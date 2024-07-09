import React, { useEffect, useState } from "react";
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
  InputNumber,
  Space,
  DatePicker,
  Popconfirm,
  Row,
  Col,
  Dropdown,
  Menu,
  Card,
  Tooltip,
} from "antd";
import axios from "axios";
import {
  SearchOutlined,
  MoreOutlined,
  PlusCircleOutlined,
} from "@ant-design/icons";
import { useForm } from "antd/es/form/Form";
import { useNavigate } from "react-router-dom";
import moment from "moment";
import uploadFile from "@/utils/upload";
import "./Petmanagement.css"; // Ensure correct path to the CSS file
import { PetsOutlined } from "@mui/icons-material";
import { CatIcon, DogIcon } from "lucide-react";

const { Option } = Select;

const Petmanagement = () => {
  const navigate = useNavigate();
  const regex30KyTu = /^.{1,30}$/; // Ensures the name is between 1 and 30 characters
  const [form] = useForm();
  const [dataSource, setDataSource] = useState([]);
  const [originalDataSource, setOriginalDataSource] = useState([]);
  const [isOpen, setIsOpen] = useState(false);
  const [birthday, setBirthday] = useState(null);
  const [isUpdate, setIsUpdate] = useState(false);
  const [petId, setPetId] = useState(null);
  const [error, setError] = useState("");
  const [previewOpen, setPreviewOpen] = useState(false);
  const [previewImage, setPreviewImage] = useState("");
  const [fileList, setFileList] = useState([]);
  const [searchText, setSearchText] = useState("");
  const [actionInProgress, setActionInProgress] = useState(false); // New state to track action

  const handleDeleteMovie = async (id) => {
    if (actionInProgress) return; // Prevent action if another is in progress
    setActionInProgress(true); // Set action in progress

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
          setOriginalDataSource(listAfterDelete);
          message.success("Delete successfully");
        } else {
          console.error(
            "Failed to delete pet:",
            response.status,
            response.statusText
          );
        }
      } catch (error) {
        console.error("Error deleting pet:", error);
      } finally {
        setActionInProgress(false); // Reset action in progress
      }
    } else {
      navigate("/login");
    }
  };

  const handleUpdate = async (id) => {
    if (actionInProgress) return; // Prevent action if another is in progress
    setActionInProgress(true); // Set action in progress

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

  const handlePreview = async (file) => {
    if (!file.url && !file.preview) {
      file.preview = await getBase64(file.originFileObj);
    }
    setPreviewImage(file.url || file.preview);
    setPreviewOpen(true);
  };

  const getBase64 = (file) =>
    new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result);
      reader.onerror = (error) => reject(error);
    });

  const uploadButton = (
    <Button icon={<PlusCircleOutlined />} type="dashed" style={{ borderRadius: "8px" }}>
      Upload
    </Button>
  );

  async function fetchMovies() {
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);

    if (userInfo != null) {
      try {
        const response = await axios.get(
          `https://localhost:7150/api/Customer/${userInfo.data.user.id}`
          
        );

        const result = response.data;
        localStorage.setItem("pets", JSON.stringify(result));

        const pets = result.data.pets.filter((x) => x.status !== false);
       
        const processedPets = pets.map((pet) => {
          return {
            ...pet,
            petBirthday: pet.petBirthday.split(" ")[0], // Extract the date part only
          };
        });
      

        setDataSource(processedPets);
        setOriginalDataSource(processedPets);
      } catch (error) {
        console.log(error);
      }
    } else {
      navigate("/");
    }
  }

  function handleShowModal() {
    setIsOpen(true);
  }

  const disabledDate = (current) => {
    return current && current > moment().endOf("day");
  };

  function handleHideModal() {
    setIsOpen(false);
    form.resetFields();
    setIsUpdate(false);
    setPetId(null);
    setActionInProgress(false); // Reset action in progress
  }

  function handleOk() {
    form.submit();
  }

  const handleSubmit = async (values) => {
    form.resetFields();
    setBirthday("");

    
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
            const updatedData = dataSource.map((pet) =>
              pet.petId === petId ? { ...pet, ...petData } : pet
            );
            setDataSource(updatedData);
            setOriginalDataSource(updatedData);
          } else {
            const newData = [
              ...dataSource,
              { ...petData, petId: response.data.petId },
            ];
            setDataSource(newData);
            setOriginalDataSource(newData);
          }
        } else {
          setError(
            response.data.message ||
              `Error ${isUpdate ? "updating" : "adding"} pet`
          );
        }
      } catch (error) {
        if (error.response && error.response.status === 401) {
          message.error("Token has expired. Please log in again.");
          localStorage.removeItem("user-info");
          navigate("/login");
        } else {
          console.error("API error:", error);
          setError("API error");
        }
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

  const handleSearch = (e) => {
    const value = e.target.value.toLowerCase();
    setSearchText(value);

    if (value) {
      const filteredData = originalDataSource.filter((item) =>
        item.petName.toLowerCase().includes(value)
      );
      setDataSource(filteredData);
    } else {
      setDataSource(originalDataSource);
    }
  };

  const menu = (id) => (
    <Menu>
      <Menu.Item onClick={() => handleUpdate(id)}>Update</Menu.Item>
      <Menu.Item danger>
        <Popconfirm
          title="Are you sure to delete this pet?"
          onConfirm={() => handleDeleteMovie(id)}
          okText="Yes"
          cancelText="No"
        >
          Delete
        </Popconfirm>
      </Menu.Item>
    </Menu>
  );

  const columns = [
    {
      title: "Pet Name",
      dataIndex: "petName",
      key: "petName",
      sorter: (a, b) => a.petName.localeCompare(b.petName),
      render: (text) => <a>{text}</a>,
    },
    {
      title: "Category",
      dataIndex: "petType",
      key: "petType",
      sorter: (a, b) => a.petType.localeCompare(b.petType),
      render: (text) => {
        if (text === "Dog") return <span><DogIcon/> Dog</span>;
        if (text === "Cat") return <span><CatIcon/> Cat</span>;
        return <span>{text}</span>;
      }
    },
    {
      title: "Weight",
      dataIndex: "petWeight",
      key: "petWeight",
      sorter: (a, b) => a.petWeight - b.petWeight,
    },
    {
      title: "Height",
      dataIndex: "petHeight",
      key: "petHeight",
      sorter: (a, b) => a.petHeight - b.petHeight,
    },
    {
      title: "Birthday",
      dataIndex: "petBirthday",
      key: "petBirthday",
      sorter: (a, b) =>
        moment(a.petBirthday).unix() - moment(b.petBirthday).unix(),
    },
    {
      title: "Image",
      dataIndex: "image",
      key: "image",
      render: (text) => <Image width={50} height={50} src={text} />,
    },
    {
      title: "Action",
      key: "action",
      render: (_, record) => (
        <Dropdown overlay={menu(record.petId)} trigger={['click']}>
          <Tooltip title="More actions">
            <Button shape="circle" icon={<MoreOutlined />} />
          </Tooltip>
        </Dropdown>
      ),
    },
  ];

  return (
    <div className="pet-management-container">
      <Card
        title="Pet Management"
        extra={
          <Button type="primary" onClick={handleShowModal} className="add-pet-button">
            Add New Pet
          </Button>
        }
        className="pet-management-card"
      >
        <Row gutter={[16, 16]} style={{ marginBottom: 20 }}>
          <Col span={12}>
            <Input
              prefix={<SearchOutlined />}
              placeholder="Search by pet name"
              value={searchText}
              onChange={handleSearch}
              allowClear
              style={{ borderRadius: "8px" }}
            />
          </Col>
        </Row>
        <Table columns={columns} dataSource={dataSource} rowKey="petId" />
      </Card>

      <Modal
        title={isUpdate ? "Update Pet" : "Add New Pet"}
        open={isOpen}
        onOk={handleOk}
        onCancel={handleHideModal}
        className="pet-management-modal"
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
            rules={[
              { required: true, message: "Please input the pet name!" },
              {
                pattern: /^[a-zA-ZÀ-ÿ\s]{1,30}$/,
                message:
                  "Name must be between 1 and 30 characters and cannot contain special characters!",
              },
            ]}
          >
            <Input placeholder="Enter pet name" style={{ borderRadius: "8px" }} />
          </Form.Item>
          <Form.Item
            label="Height"
            name="height"
            rules={[
              { required: true, message: "Please input the height!" },
              { type: "number", message: "Height must be a number!" },
              {
                validator: (_, value) =>
                  value >= 0
                    ? Promise.resolve()
                    : Promise.reject("Height must be a non-negative number!"),
              },
            ]}
          >
            <InputNumber style={{ width: "100%", borderRadius: "8px" }} min={0} placeholder="Enter pet height" />
          </Form.Item>
          <Form.Item
            label="Weight"
            name="weight"
            rules={[
              { required: true, message: "Please input the weight!" },
              { type: "number", message: "Weight must be a number!" },
              {
                validator: (_, value) =>
                  value >= 0
                    ? Promise.resolve()
                    : Promise.reject("Weight must be a non-negative number!"),
              },
            ]}
          >
            <InputNumber style={{ width: "100%", borderRadius: "8px" }} min={0} placeholder="Enter pet weight" />
          </Form.Item>
          <Form.Item
            label="Category"
            name="category"
            rules={[{ required: true, message: "Please select a category!" }]}
          >
            <Select placeholder="Select category" style={{ borderRadius: "8px" }}>
              <Option value="Cat">
                <CatIcon/> Cat
              </Option>
              <Option value="Dog">
                <DogIcon/> Dog
              </Option>
              <Option value="Other"><PetsOutlined />Other</Option>
            </Select>
          </Form.Item>
          <Form.Item label="Birthday">
            <DatePicker
              value={birthday}
              onChange={(date) => setBirthday(date)}
              disabledDate={disabledDate}
              format="YYYY-MM-DD"
              style={{ width: "100%", borderRadius: "8px" }}
              placeholder="Select pet's birthday"
            />
          </Form.Item>
          <Form.Item label="Poster" name="poster_path">
            <Upload
              action="https://660d2bd96ddfa2943b33731c.mockapi.io/api/upload"
              listType="picture-card"
              fileList={fileList}
              onPreview={handlePreview}
              onChange={handleChange}
              style={{ borderRadius: "8px" }}
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
};

export default Petmanagement;
