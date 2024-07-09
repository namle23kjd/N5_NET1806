import React, { useState, useEffect } from "react";
import {
  Button,
  Space,
  Table,
  Input,
  Modal,
  Form,
  message,
  Select,
  Layout,
  Menu,
  Typography,
} from "antd";
import {
  PlusOutlined,
  FilterOutlined,
  SearchOutlined,
} from "@ant-design/icons";
import "../../assets/css/managerPage.css";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import moment from "moment";

const { Header, Content, Sider } = Layout;
const { Title } = Typography;

const ManagerPage = () => {
  const initialCheckaccepts = [];

  const [services, setServices] = useState([]);
  const [checkaccepts, setCheckaccepts] = useState(initialCheckaccepts);
  const [filteredInfo, setFilteredInfo] = useState({});
  const [sortedInfo, setSortedInfo] = useState({});
  const [searchText, setSearchText] = useState("");
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [editingRecord, setEditingRecord] = useState(null);
  const [activeTab, setActiveTab] = useState("service");

  const [form] = Form.useForm();

  useEffect(() => {
    fetch("https://localhost:7150/api/Service")
      .then((response) => response.json())
      .then((data) => {
        const formattedData = data.data.data.map((service, index) => ({
          key: index + 1,
          serviceId: service.serviceId,
          serviceName: service.serviceName,
          serviceDescription: service.serviceDescription,
          duration: service.duration,
          price: service.price.toLocaleString("vi-VN", {
            style: "currency",
            currency: "VND",
          }),
          status: service.status,
        }));
        setServices(formattedData);
      });
  }, []);

  const onFinish = (values) => {
    console.log("Form values:", values);
  };

  const [staffList, setStaffList] = useState([]);
  useEffect(() => {
    fetch("https://localhost:7150/api/Booking/bookings/not-accepted")
      .then((response) => response.json())
      .then((data) => {
        const filteredData = data.data.filter(booking => booking.status !== 2);
        const formattedData = filteredData.map((booking, index) => ({
          key: index + 1,
          bookingId: booking.bookingId,
          customerName: booking.customerName,
          serviceId: booking.serviceId,
          serviceName: booking.serviceName,
          petName: booking.petName,
          startDate: booking.startDate,
          endDate: booking.endDate,
          staffName: booking.staffName === "Unknown" ? null : booking.staffName,
          staffId: booking.staffId,
          checkAccept: booking.checkAccept,
        }));
        console.log(formattedData);
        setCheckaccepts(formattedData);
      })
      .catch((error) => {
        console.error("Error fetching checkaccept data:", error);
        message.error("Failed to fetch checkaccept data");
      });
  }, []);

  const fetchStaffBookingsSummary = async (date) => {
    try {
      const response = await axios.get(
        `https://localhost:7150/api/Staff/bookings-summary?date=${date}`
      );
      if (response.status === 200) {
        const summaryData = response.data.data;
        const staffBookingsMap = summaryData.reduce((map, staff) => {
          map[staff.staffId] = staff.totalBooking;
          return map;
        }, {});

        const formattedStaffList = staffList.map((staff) => ({
          staffId: staff.staffId,
          fullName: `${staff.fullName} (${
            staffBookingsMap[staff.staffId] || 0
          } bookings)`,
        }));

        setStaffList(formattedStaffList);
      }
    } catch (error) {
      console.error("Error fetching staff bookings summary:", error);
      message.error("Failed to fetch staff bookings summary");
    }
  };

  const handleStaffSelect = async (value, record) => {
    const selectedStaff = staffList.find((staff) => staff.staffId === value);
    if (selectedStaff) {
      const updatedCheckaccepts = checkaccepts.map((checkaccept) =>
        checkaccept.key === record.key
          ? {
              ...checkaccept,
              staffId: selectedStaff.staffId,
              staffName: selectedStaff.fullName.split(" (")[0],
            }
          : checkaccept
      );
      setCheckaccepts(updatedCheckaccepts);
      message.success("Staff assigned successfully");
    }
  };

  useEffect(() => {
    fetch("https://localhost:7150/api/Staff")
      .then((response) => response.json())
      .then((data) => {
        const formattedStaffList = data.data.map((staff) => {
          const names = staff.fullName.split(" ");
          const lastName = names[names.length - 1];

          return {
            staffId: staff.staffId,
            fullName: lastName,
          };
        });

        setStaffList(formattedStaffList);
      })
      .catch((error) => {
        console.error("Error fetching staff data:", error);
        message.error("Failed to fetch staff data");
      });
  }, []);

  const handleChange = (pagination, filters, sorter) => {
    setFilteredInfo(filters);
    setSortedInfo(sorter);
  };

  const clearAll = () => {
    setFilteredInfo({});
    setSortedInfo({});
    setSearchText("");
  };

  const handleSearch = (e) => {
    setSearchText(e.target.value);
  };

  const handleAdd = () => {
    setEditingRecord(null);
    setIsModalVisible(true);
  };

  const handleEdit = (record) => {
    setEditingRecord(record);
    setIsModalVisible(true);
    form.setFieldsValue(record);
  };

  const navigate = useNavigate();

  const handleDelete = async (key) => {
    const userInfoString = localStorage.getItem("user-info");
    if (!userInfoString) {
      navigate("/login");
      return;
    }

    try {
      if (activeTab === "service") {
        const service = services.find((service) => service.key === key);

        if (service) {
          const newStatus = !service.status;
          service.status = newStatus;
          setServices([...services]);

          const result = await axios.delete(
            `https://localhost:7150/api/Service/${service.serviceId}`
          );
          if (result.status === 200) {
            message.success("Service status updated successfully");
          }
        } else {
          message.error("Service not found");
        }
      }
    } catch (error) {
      console.error("Error deleting service:", error);
      message.error("Failed to delete service");
    }
  };

  const handleModalOk = () => {
    form
      .validateFields()
      .then(async (values) => {
        if (editingRecord) {
          if (activeTab === "service") {
            if (!editingRecord.status) {
              message.error("Cannot edit a service that is not active.");
              return;
            }
            try {
              const serviceId = editingRecord.serviceId;
              const updateData = {
                serviceName: values.serviceName,
                status: editingRecord.status,
                serviceDescription: values.serviceDescription,
                serviceImage: values.serviceImage || "",
                duration: values.duration,
                price: parseFloat(values.price),
                comboId: values.comboId || null,
              };

              const result = await axios.put(
                `https://localhost:7150/api/Service/${serviceId}`,
                updateData
              );

              if (result.status === 200) {
                setServices(
                  services.map((service) =>
                    service.key === editingRecord.key
                      ? {
                          ...updateData,
                          key: service.key,
                          serviceId: serviceId,
                        }
                      : service
                  )
                );
                message.success("Service updated successfully");
              } else {
                message.error("Failed to update service");
              }
            } catch (error) {
              console.error("Error updating service:", error);
              message.error("Failed to update service");
            }
          }
        } else {
          if (activeTab === "service") {
            console.log(values);
            try {
              const newService = {
                serviceName: values.serviceName,
                serviceDescription: values.serviceDescription,
                duration: values.duration,
                price: parseFloat(values.price),
                status: true, // Assuming new services are active by default
                comboId: values.comboId || null,
              };

              const result = await axios.post(
                "https://localhost:7150/api/Service",
                newService
              );
              if (result.status === 200) {
                const addedService = {
                  ...newService,
                  key: services.length + 1,
                  serviceId: result.data.data.serviceId,
                  price: newService.price.toLocaleString("vi-VN", {
                    style: "currency",
                    currency: "VND",
                  }),
                };
                setServices([...services, addedService]);
                message.success("Service added successfully");
              } else {
                message.error("Failed to add service");
              }
            } catch (error) {
              console.error("Error adding service:", error);
              message.error("Failed to add service");
            }
          }
        }
        setIsModalVisible(false);
        form.resetFields();
      })
      .catch((info) => {
        console.log("Validate Failed:", info);
      });
  };

  const handleModalCancel = () => {
    setIsModalVisible(false);
    form.resetFields();
  };
 

  const handleAccept = async (key) => {
    const booking = checkaccepts.find((checkaccept) => checkaccept.key === key);

    if (
      !booking ||
      booking.staffId === "00000000-0000-0000-0000-000000000000"
    ) {
      message.error(
        "Please select a staff member before accepting the booking."
      );
      return;
    }

    try {
      const response = await axios.put(
        "https://localhost:7150/api/Booking/accept-booking",
        {
          bookingId: booking.bookingId,
          staffId: booking.staffId,
        }
      );

      if (response.status === 200) {
        message.success(
          "Accept successfully"
        );
        setCheckaccepts((prevCheckaccepts) =>
          prevCheckaccepts.filter((checkaccept) => checkaccept.key !== key)
        );
        message.success("Booking accepted successfully");
      } else {
        message.error("Failed to accept booking");
      }
    } catch (error) {
      message.error(error.response.data);
      const updatedCheckaccepts = checkaccepts.map((checkaccept) =>
        checkaccept.key === key
          ? {
              ...checkaccept,
              staffId: "00000000-0000-0000-0000-000000000000",
              staffName: null,
            }
          : checkaccept
      );

      setCheckaccepts(updatedCheckaccepts);
    }
  };
  const handleDeny = async (key) => {
    const booking = checkaccepts.find((checkaccept) => checkaccept.key === key);
    try {
      const response = await axios.post(
        `https://localhost:7150/api/Booking/cancel-booking`,
        {
          bookingId: booking.bookingId
        }
      );

      if (response.status === 200) {
        message.success(
          "Deny successfully"
        );
        setCheckaccepts((prevCheckaccepts) =>
          prevCheckaccepts.filter((checkaccept) => checkaccept.key !== key)
        );
        // Update the booking status in dataSource
      
      } else {
        throw new Error("Failed to submit refund request.");
      }
    } catch (error) {
      console.error("Error submitting refund request:", error);
      message.error(
        error.response?.data || "Failed to submit refund request."
      );
    }
  };

 

  const serviceColumns = [
    {
      title: "No.",
      dataIndex: "key",
      key: "key",
      width: "5%",
      align: "center",
      sorter: (a, b) => a.key - b.key,
      sortOrder: sortedInfo.columnKey === "key" ? sortedInfo.order : null,
    },
    {
      title: "Service Name",
      dataIndex: "serviceName",
      key: "serviceName",
      width: "20%",
      align: "center",
      sorter: (a, b) => a.serviceName.length - b.serviceName.length,
      sortOrder:
        sortedInfo.columnKey === "serviceName" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Description",
      dataIndex: "serviceDescription",
      key: "serviceDescription",
      width: "30%",
      align: "center",
      sorter: (a, b) =>
        a.serviceDescription.length - b.serviceDescription.length,
      sortOrder:
        sortedInfo.columnKey === "serviceDescription" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Duration",
      dataIndex: "duration",
      key: "duration",
      width: "15%",
      align: "center",
      sorter: (a, b) => a.duration.length - b.duration.length,
      sortOrder: sortedInfo.columnKey === "duration" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Price (VND)",
      dataIndex: "price",
      key: "price",
      width: "15%",
      align: "center",
      sorter: (a, b) =>
        parseFloat(a.price.replace(/[\₫,]/g, "")) -
        parseFloat(b.price.replace(/[\₫,]/g, "")),
      sortOrder: sortedInfo.columnKey === "price" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Action",
      key: "action",
      width: "15%",
      align: "center",
      render: (text, record) => (
        <Space size="middle">
          <Button type="link" onClick={() => handleEdit(record)}>
            Edit
          </Button>
          <Button type="link" danger onClick={() => handleDelete(record.key)}>
            {record.status === false ? "Activate" : "Delete"}
          </Button>
        </Space>
      ),
    },
  ];

  const checkacceptColumns = [
    {
      title: "Customer Name",
      dataIndex: "customerName",
      key: "customerName",
      sorter: (a, b) => a.customerName.length - b.customerName.length,
      sortOrder:
        sortedInfo.columnKey === "customerName" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Service Name",
      dataIndex: "serviceName",
      key: "serviceName",
      sorter: (a, b) => a.serviceName.length - b.serviceName.length,
      sortOrder:
        sortedInfo.columnKey === "serviceName" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Pet Name",
      dataIndex: "petName",
      key: "petName",
      sorter: (a, b) => a.petName.length - b.petName.length,
      sortOrder: sortedInfo.columnKey === "petName" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Start Date",
      dataIndex: "startDate",
      key: "startDate",
      sorter: (a, b) => new Date(a.startDate) - new Date(b.startDate),
      sortOrder: sortedInfo.columnKey === "startDate" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "End Date",
      dataIndex: "endDate",
      key: "endDate",
      sorter: (a, b) => new Date(a.endDate) - new Date(b.endDate),
      sortOrder: sortedInfo.columnKey === "endDate" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Staff Name",
      dataIndex: "staffName",
      key: "staffName",
      sorter: (a, b) => a.staffName?.length - b.staffName?.length,
      sortOrder: sortedInfo.columnKey === "staffName" ? sortedInfo.order : null,
      ellipsis: true,
      render: (text, record) =>
        record.staffName === null ? (
          <Select
            placeholder="Select Staff"
            onFocus={() =>
              fetchStaffBookingsSummary(record.startDate.split(" ")[0])
            }
            onChange={(value) => handleStaffSelect(value, record)}
            style={{ width: "100%" }}
          >
            {staffList.map((staff) => (
              <Select.Option key={staff.staffId} value={staff.staffId}>
                {staff.fullName}
              </Select.Option>
            ))}
          </Select>
        ) : (
          text
        ),
    },
    {
      title: "Action",
      key: "action",
      render: (text, record) => (
        <Space size="middle">
          <Button
            type="primary"
            onClick={() => handleAccept(record.key)}
            disabled={record.checkAccept}
          >
            Accept
          </Button>
        </Space>
      ),
    },
    {
      title: "Action",
      key: "action",
      render: (text, record) => (
        <Space size="middle">
          <Button
            type="primary"
            onClick={() => handleDeny(record.key)}
            disabled={record.checkAccept}
          >
            Deny
          </Button>
        </Space>
      ),
    },
  ];

  const filteredData =
    activeTab === "service"
      ? services.filter((service) =>
          service.serviceName.toLowerCase().includes(searchText.toLowerCase())
        )
      : checkaccepts.filter((checkaccept) =>
          checkaccept.customerName
            ?.toLowerCase()
            .includes(searchText.toLowerCase())
        );

  return (
    <Layout>
      <Header className="header">
        <Title level={3} style={{ color: "#fff", margin: 0 }}>
          Manager Page
        </Title>
      </Header>
      <Layout>
        <Sider width={200} className="site-layout-background">
          <Menu
            mode="inline"
            selectedKeys={[activeTab]}
            style={{ height: "100%", borderRight: 0 }}
          >
            <Menu.Item key="service" onClick={() => setActiveTab("service")}>
              Service Manager
            </Menu.Item>
            <Menu.Item
              key="checkaccept"
              onClick={() => setActiveTab("checkaccept")}
            >
              Checkaccept Manager
            </Menu.Item>
          </Menu>
        </Sider>
        <Layout style={{ padding: "0 24px 24px" }}>
          <Content
            style={{
              padding: 24,
              margin: 0,
              minHeight: 280,
            }}
          >
            <Space
              style={{
                marginBottom: 16,
                display: "flex",
                justifyContent: "space-between",
              }}
            >
              <Space>
                <Button icon={<FilterOutlined />} onClick={clearAll}>
                  Clear filters and sorters
                </Button>
                <Input
                  placeholder="Search by name"
                  value={searchText}
                  onChange={handleSearch}
                  prefix={<SearchOutlined />}
                />
              </Space>
              {activeTab !== "checkaccept" && (
                <Button
                  type="primary"
                  icon={<PlusOutlined />}
                  onClick={handleAdd}
                >
                  Add Service
                </Button>
              )}
            </Space>
            <Table
              columns={
                activeTab === "service" ? serviceColumns : checkacceptColumns
              }
              dataSource={filteredData}
              onChange={handleChange}
              pagination={{ pageSize: 10 }}
              rowKey={(record) => record.key}
            />
          </Content>
        </Layout>
      </Layout>
      <Modal
        title={editingRecord ? `Edit Service` : `Add Service`}
        visible={isModalVisible}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
        okText={editingRecord ? "Update" : "Add"}
      >
        <Form
          form={form}
          layout="vertical"
          name="recordForm"
          onFinish={onFinish}
        >
          <Form.Item
            name="serviceName"
            label="Service Name"
            rules={[
              { required: true, message: "Please input the service name!" },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="serviceDescription"
            label="Description"
            rules={[
              { required: true, message: "Please input the description!" },
            ]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="duration"
            label="Duration"
            rules={[{ required: true, message: "Please input the duration!" }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="price"
            label="Price (VND)"
            rules={[{ required: true, message: "Please input the price!" }]}
          >
            <Input />
          </Form.Item>
        </Form>
      </Modal>
    </Layout>
  );
};

export default ManagerPage;
