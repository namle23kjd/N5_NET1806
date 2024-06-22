import {
  Button,
  Form,
  Image,
  Input,
  Modal,
  Select,
  Upload,
  message,
  Table,
} from "antd";
import axios from "axios";
import { useEffect, useState } from "react";
import { PlusOutlined } from "@ant-design/icons";
import { useForm } from "antd/es/form/Form";
import { DatePicker, Space } from "antd";
import { useNavigate } from "react-router-dom";
import moment from "moment";
import uploadFile from "@/utils/upload";

import "../../components/Petmanagement/Petmanagement.css"; // Create a separate CSS file for custom styles

function Petmanagement() {
  const navigate = useNavigate();
  const regex30KyTu = /^.{1,30}$/; // Ensures the name is between 1 and 30 characters
  const [form] = useForm();
  const [dataSource, setDataSource] = useState([]);
  const [isOpen, setIsOpen] = useState(false);
  const [birthday, setBirthday] = useState(null);
  const [isUpdate, setIsUpdate] = useState(false);
  const [petId, setPetId] = useState(null);
  const [error, setError] = useState("");
  const [previewOpen, setPreviewOpen] = useState(false);
  const [previewImage, setPreviewImage] = useState("");
  const [fileList, setFileList] = useState([]);

  const handleDeleteMovie = async (id) => {
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);

    if (userInfo != null) {
      try {
        const response = await axios.delete(
          `https://localhost:7150/api/Pet/${id}`,
          {
            headers: {
              Authorization: `Bearer ${userInfo.data.token}`,
            },
          }
        );

        if (response.status === 401) {
          localStorage.removeItem("user-info");
          navigate("/login");
          return;
        }

        if (response.status === 200) {
          const listAfterDelete = dataSource.filter((pet) => pet.petId !== id);
          setDataSource(listAfterDelete);
        } else {
          console.error(
            "Failed to delete pet:",
            response.status,
            response.statusText
          );
        }
      } catch (error) {
        console.error("Error deleting pet:", error);
      }
    } else {
      navigate("/login");
    }
  };

  const handleUpdate = async (id) => {
    setIsUpdate(true);
    setPetId(id);
    const pet = dataSource.find((x) => x.petId === id);
    form.setFieldsValue({
      name: pet.petName,
      height: pet.petHeight,
      weight: pet.petWeight,
      category: pet.petType,
      poster_path: pet.image,
    });
    setBirthday(pet.petBirthday ? moment(pet.petBirthday, "YYYY-MM-DD") : null);
    setFileList(pet.image ? [{ url: pet.image }] : []);
    setIsOpen(true);
  };

  const handleChange = ({ fileList: newFileList }) => setFileList(newFileList);
  const uploadButton = (
    <Button icon={<PlusOutlined />} type="dashed">
      Upload
    </Button>
  );

  async function fetchMovies() {
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);

    if (userInfo != null) {
      try {
        const response = await axios.get(
          `https://localhost:7150/api/Customer/${userInfo.data.user.id}`,
          {
            headers: {
              Authorization: `Bearer ${userInfo.data.token}`,
            },
          }
        );

        if (response.status === 401) {
          localStorage.removeItem("user-info");
          navigate("/login");
          return;
        }

        const result = response.data;
        localStorage.setItem("pets", JSON.stringify(result));

        setDataSource(result.data.pets.filter((x) => x.status !== false));
      } catch (error) {
        if (error.response && error.response.status === 401) {
          localStorage.removeItem("user-info");
          message.error("Please,Login in again");
          navigate("/login");
        } else {
          message.error("Có lỗi xảy ra. Vui lòng thử lại.");
        }
      }
    } else {
      navigate("/");
    }
  }

  function handleShowModal() {
    setIsOpen(true);
  }

  function handleHideModal() {
    setIsOpen(false);
    form.resetFields();
    setIsUpdate(false);
    setPetId(null);
  }

  function handleOk() {
    form.submit();
  }

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
      const apiUrl = isUpdate
        ? `https://localhost:7150/api/Pet/${petId}`
        : `https://localhost:7150/api/Pet`;
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
        const response = await axios({
          method: isUpdate ? "PUT" : "POST",
          url: apiUrl,
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          data: petData,
        });

        if (response.status === 200 || response.status === 201) {
          message.success(`Pet ${isUpdate ? "updated" : "added"} successfully`);
          setError("");
          if (isUpdate) {
            setDataSource((prevDataSource) =>
              prevDataSource.map((pet) =>
                pet.petId === petId ? { ...pet, ...petData } : pet
              )
            );
          } else {
            setDataSource((prevDataSource) => [...prevDataSource, petData]);
          }
        } else {
          setError(
            response.data.message ||
              `Error ${isUpdate ? "updating" : "adding"} pet`
          );
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

  useEffect(() => {
    fetchMovies();
  }, []);

  const columns = [
    {
      title: "Pet Name",
      dataIndex: "petName",
      key: "petName",
    },
    {
      title: "Category",
      dataIndex: "petType",
      key: "petType",
    },
    {
      title: "Weight",
      dataIndex: "petWeight",
      key: "petWeight",
    },
    {
      title: "Height",
      dataIndex: "petHeight",
      key: "petHeight",
    },
    {
      title: "Birthday",
      dataIndex: "petBirthday",
      key: "petBirthday",
    },
    {
      title: "Image",
      dataIndex: "image",
      key: "image",
      render: (text) => <Image width={50} src={text} />,
    },
    {
      title: "Action",
      key: "action",
      render: (_, record) => (
        <Space size="middle">
          <Button type="link" onClick={() => handleUpdate(record.petId)}>
            Update
          </Button>
          <Button type="link" danger onClick={() => handleDeleteMovie(record.petId)}>
            Delete
          </Button>
        </Space>
      ),
    },
  ];

  return (
    <div className="pet-management-container">
      <Button type="primary" onClick={handleShowModal} className="add-pet-button">
        Add new pet
      </Button>
      <Table columns={columns} dataSource={dataSource} rowKey="petId" />

      <Modal
        title={isUpdate ? "Update Pet" : "Add New Pet"}
        open={isOpen}
        onCancel={handleHideModal}
        onOk={handleOk}
      >
        <Form
          labelCol={{
            span: 24,
          }}
          form={form}
          onFinish={handleSubmit}
        >
          <Form.Item
            label="Name"
            name="name"
            rules={[{ required: true, message: "Please input the pet name!" }]}
          >
            <Input />
          </Form.Item>
          <Form.Item label="Height" name="height">
            <Input />
          </Form.Item>
          <Form.Item label="Weight" name="weight">
            <Input />
          </Form.Item>
          <Form.Item
            label="Category"
            name="category"
            rules={[{ required: true, message: "Please select a category!" }]}
          >
            <Select
              options={[
                { value: "Cat", label: <span>Cat</span> },
                { value: "Dog", label: <span>Dog</span> },
                { value: "Other", label: <span>Other</span> },
              ]}
            />
          </Form.Item>
          <Form.Item label="Birthday">
            <Space direction="vertical">
              <DatePicker
                value={birthday}
                onChange={(date) => setBirthday(date)}
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
              {fileList.length >= 1 ? null : uploadButton}
            </Upload>
          </Form.Item>
          {error && <p className="error-message">{error}</p>}
        </Form>
      </Modal>

      {previewImage && (
        <Image
          wrapperStyle={{
            display: "none",
          }}
          preview={{
            visible: previewOpen,
            onVisibleChange: (visible) => setPreviewOpen(visible),
            afterOpenChange: (visible) => !visible && setPreviewImage(""),
          }}
          src={previewImage}
        />
      )}
    </div>
  );
}

export default Petmanagement;
