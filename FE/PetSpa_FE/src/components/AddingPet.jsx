/* eslint-disable react/prop-types */
import { Form, Input, InputNumber, Modal, Select, Upload, message } from "antd";
import axios from "axios";
import { useState } from "react";
import { PlusOutlined } from "@ant-design/icons";
import { useForm } from "antd/es/form/Form";
import { DatePicker, Space } from "antd";
import { useNavigate } from "react-router-dom";
import uploadFile from "@/utils/upload";
import { Option } from "antd/es/mentions";

function AddingPet({ isOpen, handleHideModal, setDataSource }) {
  const navigate = useNavigate();
  const regex30KyTu = /^.{1,30}$/; // Ensures the name is between 1 and 30 characters
  const [form] = useForm();
  const [birthday, setBirthday] = useState(null);
  const [error, setError] = useState("");
  const [fileList, setFileList] = useState([]);

  const handleChange = ({ fileList: newFileList }) => setFileList(newFileList);
  const uploadButton = (
    <button
      style={{
        border: 0,
        background: "none",
      }}
      type="button"
    >
      <PlusOutlined />
      <div
        style={{
          marginTop: 8,
        }}
      >
        Upload
      </div>
    </button>
  );

  const handleSubmit = async (values) => {
    if (values.poster_path && values.poster_path.file) {
      const url = await uploadFile(values.poster_path.file.originFileObj);
      if (url) {
        values.poster_path = url;
      }
    }

    if (!regex30KyTu.test(values.name.trim())) {
      setError("Name is required");
      return;
    }

    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);
    const token = userInfo?.data?.token;

    if (userInfo?.data?.user?.id && token) {
      const apiUrl = `https://localhost:7150/api/Pet`;
      const petData = {
        cusId: userInfo.data.user.id,
        petType: values.category,
        petName: values.name,
        image: values.poster_path != null ? values.poster_path : "",
        petBirthday: birthday ? birthday.format("YYYY-MM-DD") : null,
        status: "true",
        petWeight: values.weight || "",
        petHeight: values.height || "",
      };

      try {
        const response = await axios.post(apiUrl, petData, {
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        });

        if (response.status === 200 || response.status === 201) {
          message.success("Pet added successfully");
          setError("");
          setDataSource((prevDataSource) => [...prevDataSource, petData]);
        } else {
          setError(response.data.message || "Error adding pet");
        }
      } catch (error) {
        console.error("API error:", error);
        setError("API error");
      }
    } else {
      setError("You must be logged in");
      navigate("/login");
    }

    handleHideModal();
    form.resetFields();
  };

  return (
    <Modal
      title="Add New Pet"
      open={isOpen}
      onCancel={handleHideModal}
      onOk={form.submit}
    >
      <Form labelCol={{ span: 24 }} form={form} onFinish={handleSubmit}>
        <Form.Item
          label="Name"
          name="name"
          rules={[{ required: true, message: "Please input the pet name!" }]}
        >
          <Input style={{ width: "100%" }} />
        </Form.Item>
        <Form.Item
          label="Height"
          name="height"
          rules={[
            { required: true, message: "Please input the pet height!" },
            {
              type: "number",
              min: 0,
              message: "Height must be a non-negative number!",
            },
            {
              validator: (_, value) =>
                value < 0
                  ? Promise.reject("Height must be a non-negative number!")
                  : Promise.resolve(),
            },
          ]}
        >
          <InputNumber style={{ width: "100%" }} />
        </Form.Item>
        <Form.Item
          label="Weight"
          name="weight"
          rules={[
            { required: true, message: "Please input the pet weight!" },
            {
              type: "number",
              min: 0,
              message: "Weight must be a non-negative number!",
            },
            {
              validator: (_, value) =>
                value < 0
                  ? Promise.reject("Weight must be a non-negative number!")
                  : Promise.resolve(),
            },
          ]}
        >
          <InputNumber style={{ width: "100%" }} />
        </Form.Item>
        <Form.Item
          label="Category"
          name="category"
          rules={[{ required: true, message: "Please select a category!" }]}
        >
          <Select style={{ width: "100%" }}>
            <Option value="Cat">Cat</Option>
            <Option value="Dog">Dog</Option>
            <Option value="Other">Other</Option>
          </Select>
        </Form.Item>
        <Form.Item label="Birthday">
          <Space direction="vertical" style={{ width: "100%" }}>
            <DatePicker
              showTime
              value={birthday}
              onChange={(date) => setBirthday(date)}
              style={{ width: "100%" }}
            />
          </Space>
        </Form.Item>
        <Form.Item label="Poster" name="poster_path">
          <Upload
            action="https://660d2bd96ddfa2943b33731c.mockapi.io/api/upload"
            listType="picture-card"
            fileList={fileList}
            onChange={handleChange}
          >
            {fileList.length >= 8 ? null : uploadButton}
          </Upload>
        </Form.Item>
        <p style={{ color: "red" }}>{error}</p>
      </Form>
    </Modal>
  );
}

export default AddingPet;