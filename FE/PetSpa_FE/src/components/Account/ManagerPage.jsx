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
import axios from "axios";
import { useNavigate } from "react-router-dom";
import moment from "moment";
import "../../assets/css/managerPage.css";

const { Header, Content, Sider } = Layout;
const { Title } = Typography;

const ManagerPage = () => {
  const [services, setServices] = useState([]);
  const [checkaccepts, setCheckaccepts] = useState([]);
  const [filteredInfo, setFilteredInfo] = useState({});
  const [sortedInfo, setSortedInfo] = useState({});
  const [searchText, setSearchText] = useState("");
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [editingRecord, setEditingRecord] = useState(null);
  const [activeTab, setActiveTab] = useState("service");
  const [activeTaskTab, setActiveTaskTab] = useState("todo");
  const [staffList, setStaffList] = useState([]);
  const [tasks, setTasks] = useState({ todo: [], inProgress: [], done: [] });
  const [error, setError] = useState("");

  const [form] = Form.useForm();
  const navigate = useNavigate();

  useEffect(() => {
    fetchServices();
    fetchBookings();
    fetchStaff();
    if (activeTab === "task") {
      fetchTasksForAllStaff();
    }
  }, [activeTab]);

  const fetchServices = async () => {
    try {
      const response = await fetch("https://localhost:7150/api/Service");
      const data = await response.json();
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
    } catch (error) {
      console.error("Error fetching services:", error);
      message.error("Failed to fetch services");
    }
  };

  const fetchBookings = async () => {
    try {
      const response = await fetch("https://localhost:7150/api/Booking/bookings/not-accepted");
      const data = await response.json();
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
      setCheckaccepts(formattedData);
    } catch (error) {
      console.error("Error fetching bookings:", error);
      message.error("Failed to fetch bookings");
    }
  };

  const fetchStaff = async () => {
    try {
      const response = await fetch("https://localhost:7150/api/Staff");
      const data = await response.json();
      const formattedStaffList = data.data.map((staff) => ({
        staffId: staff.staffId,
        fullName: staff.fullName,
      }));
      setStaffList(formattedStaffList);
    } catch (error) {
      console.error("Error fetching staff data:", error);
      message.error("Failed to fetch staff data");
    }
  };

  const fetchTasksForAllStaff = async () => {
    try {
      const userInfoString = localStorage.getItem("user-info");
      if (!userInfoString) {
        navigate("/login");
        return;
      }

      const userInfo = JSON.parse(userInfoString);
      const token = userInfo.data.token;

      const headers = {
        Authorization: `Bearer ${token}`,
      };

      const allTasks = { todo: [], inProgress: [], done: [] };

      for (const staff of staffList) {
        const { staffId } = staff;

        const todoResponse = await axios.get(
          `https://localhost:7150/api/Staff/${staffId}/pending-bookings`,
          { headers }
        );
        const inProgressResponse = await axios.get(
          `https://localhost:7150/api/Staff/${staffId}/current-booking`,
          { headers }
        );
        const doneResponse = await axios.get(
          `https://localhost:7150/api/Staff/${staffId}/completed-bookings`,
          { headers }
        );

        allTasks.todo.push(...mapApiResponse(todoResponse.data, staff.fullName));
        allTasks.inProgress.push(...mapApiResponse(inProgressResponse.data, staff.fullName));
        allTasks.done.push(...mapApiResponse(doneResponse.data, staff.fullName));
      }

      setTasks(allTasks);
    } catch (error) {
      if (error.response && error.response.status === 403) {
        setError("You are not authorized to access this resource.");
      } else {
        console.error("Error fetching data:", error);
      }
    }
  };

  const mapApiResponse = (data, staffName) => {
    return data.map((item) => ({
      id: item.bookingId,
      service: item.serviceName,
      pet: item.petName,
      owner: item.customerName,
      date: new Date(item.startDate).toLocaleDateString(),
      time: new Date(item.startDate).toLocaleTimeString(),
      staffName: staffName,
    }));
  };

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

  const handleDelete = async (key) => {
    const userInfoString = localStorage.getItem("user-info");
    if (!userInfoString) {
      navigate("/login");
      return;
    }

    try {
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
          await updateService(values);
        } else {
          await addService(values);
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

  const updateService = async (values) => {
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
  };

  const addService = async (values) => {
    try {
      const newService = {
        serviceName: values.serviceName,
        serviceDescription: values.serviceDescription,
        duration: values.duration,
        price: parseFloat(values.price),
        status: true,
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
  };

  const handleAccept = async (key) => {
    const booking = checkaccepts.find((checkaccept) => checkaccept.key === key);

    if (!booking || booking.staffId === "00000000-0000-0000-0000-000000000000") {
      message.error("Please select a staff member before accepting the booking.");
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
          bookingId: booking.bookingId,
        }
      );

      if (response.status === 200) {
        setCheckaccepts((prevCheckaccepts) =>
          prevCheckaccepts.filter((checkaccept) => checkaccept.key !== key)
        );
        message.success("Booking denied successfully");
      } else {
        throw new Error("Failed to submit refund request.");
      }
    } catch (error) {
      console.error("Error submitting refund request:", error);
      message.error(error.response?.data || "Failed to submit refund request.");
    }
  };

  const printTasks = (tasksToPrint) => {
    const newWindow = window.open("", "", "height=800,width=600");
    newWindow.document.write("<html><head><title>Print</title>");
    newWindow.document.write(
      '<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" type="text/css" />'
    );
    newWindow.document.write("<style>");
    newWindow.document.write(
      "@media print { body { -webkit-print-color-adjust: exact; margin: 20px; font-size: 18px; }"
    );
    newWindow.document.write(
      ".printable-content { width: 100%; margin: auto; }"
    );
    newWindow.document.write(
      ".card-body { padding: 20px; border: 1px solid #dee2e6; border-radius: 4px; }"
    );
    newWindow.document.write(
      ".media-1 { margin-bottom: 20px; display: flex; align-items: center; }"
    );
    newWindow.document.write(".media-body { padding: 10px; }");
    newWindow.document.write("img { max-width: 100%; height: auto; }");
    newWindow.document.write(
      ".h5, .h5 a { font-size: 1.5rem; font-weight: 500; margin-bottom: 0.5rem; }"
    );
    newWindow.document.write(".text-body { font-size: 1rem; color: #000; }");
    newWindow.document.write(".d-print-none { display: none; }"); // Hide print button
    newWindow.document.write("</style>");
    newWindow.document.write("</head><body class='printable-content'>");

    tasksToPrint.forEach((task) => {
      newWindow.document.write(
        `<div class='card-body'>${task.service} for ${task.pet} (${task.id}) at ${task.time} on ${task.date}</div>`
      );
      newWindow.document.write("<hr>"); // Add a separator between tasks
    });

    newWindow.document.close();
    newWindow.focus(); // Necessary for IE >= 10
    newWindow.print();
  };

  const TaskList = ({ tasks, printTasks }) => {
    const [selectedTasks, setSelectedTasks] = useState([]);

    const handleTaskSelect = (task) => {
      setSelectedTasks((prevSelectedTasks) =>
        prevSelectedTasks.includes(task)
          ? prevSelectedTasks.filter((t) => t !== task)
          : [...prevSelectedTasks, task]
      );
    };

    return (
      <div>
        <table>
          <thead>
            <tr>
              <th>#</th>
              <th>Service</th>
              <th>Pet</th>
              <th>Owner</th>
              <th>Date</th>
              <th>Time</th>
              <th>Staff</th>
              <th>Select</th>
            </tr>
          </thead>
          <tbody>
            {tasks.map((task, index) => (
              <tr key={task.id}>
                <td>{index + 1}</td>
                <td>{task.service}</td>
                <td>{task.pet}</td>
                <td>{task.owner}</td>
                <td>{task.date}</td>
                <td>{task.time}</td>
                <td>{task.staffName}</td>
                <td>
                  <input
                    type="checkbox"
                    checked={selectedTasks.includes(task)}
                    onChange={() => handleTaskSelect(task)}
                  />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        <button
          className="print-button"
          onClick={() => printTasks(selectedTasks)}
          disabled={selectedTasks.length === 0}
        >
          Print Selected Tasks
        </button>
      </div>
    );
  };

  const serviceColumns = [
    {
      title: "Service ID",
      dataIndex: "serviceId",
      key: "serviceId",
      sorter: (a, b) => a.serviceId - b.serviceId,
      sortOrder: sortedInfo.columnKey === "serviceId" && sortedInfo.order,
    },
    {
      title: "Service Name",
      dataIndex: "serviceName",
      key: "serviceName",
      filteredValue: filteredInfo.serviceName || null,
      onFilter: (value, record) => record.serviceName.includes(value),
      sorter: (a, b) => a.serviceName.length - b.serviceName.length,
      sortOrder: sortedInfo.columnKey === "serviceName" && sortedInfo.order,
    },
    {
      title: "Description",
      dataIndex: "serviceDescription",
      key: "serviceDescription",
    },
    {
      title: "Duration",
      dataIndex: "duration",
      key: "duration",
    },
    {
      title: "Price",
      dataIndex: "price",
      key: "price",
    },
    {
      title: "Status",
      dataIndex: "status",
      key: "status",
      render: (status) => (status ? "Active" : "Inactive"),
    },
    {
      title: "Action",
      key: "action",
      render: (text, record) => (
        <Space size="middle">
          <Button type="primary" onClick={() => handleEdit(record)}>
            Edit
          </Button>
          <Button type="danger" onClick={() => handleDelete(record.key)}>
            Delete
          </Button>
        </Space>
      ),
    },
  ];

  const checkacceptColumns = [
    {
      title: "Booking ID",
      dataIndex: "bookingId",
      key: "bookingId",
    },
    {
      title: "Customer Name",
      dataIndex: "customerName",
      key: "customerName",
    },
    {
      title: "Service Name",
      dataIndex: "serviceName",
      key: "serviceName",
    },
    {
      title: "Pet Name",
      dataIndex: "petName",
      key: "petName",
    },
    {
      title: "Start Date",
      dataIndex: "startDate",
      key: "startDate",
      render: (text) => moment(text).format("YYYY-MM-DD"),
    },
    {
      title: "End Date",
      dataIndex: "endDate",
      key: "endDate",
      render: (text) => moment(text).format("YYYY-MM-DD"),
    },
    {
      title: "Staff Name",
      dataIndex: "staffName",
      key: "staffName",
      render: (text, record) => (
        <Select
          value={record.staffId}
          onChange={(value) => handleStaffSelect(value, record)}
          style={{ width: "100%" }}
        >
          {staffList.map((staff) => (
            <Select.Option key={staff.staffId} value={staff.staffId}>
              {staff.fullName}
            </Select.Option>
          ))}
        </Select>
      ),
    },
    {
      title: "Action",
      key: "action",
      render: (text, record) => (
        <Space size="middle">
          <Button type="primary" onClick={() => handleAccept(record.key)}>
            Accept
          </Button>
          <Button type="danger" onClick={() => handleDeny(record.key)}>
            Deny
          </Button>
        </Space>
      ),
    },
  ];

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
            items={[
              { key: "service", label: "Service Manager", onClick: () => setActiveTab("service") },
              { key: "checkaccept", label: "Checkaccept Manager", onClick: () => setActiveTab("checkaccept") },
              { key: "task", label: "Task Manager", onClick: () => setActiveTab("task") }
            ]}
            style={{ height: "100%", borderRight: 0 }}
          />
        </Sider>
        <Layout style={{ padding: "0 24px 24px" }}>
          <Content
            style={{
              padding: 24,
              margin: 0,
              minHeight: 280,
            }}
          >
            {activeTab === "task" ? (
              <div className="staff-page">
                <h1>Task Manager</h1>
                {error && <div className="error-message">{error}</div>}
                <div className="tab-section">
                  <div
                    className={`tab-title ${activeTaskTab === "todo" ? "active" : ""}`}
                    onClick={() => setActiveTaskTab("todo")}
                  >
                    To Do ({tasks.todo.length})
                  </div>
                  <div
                    className={`tab-title ${activeTaskTab === "inProgress" ? "active" : ""}`}
                    onClick={() => setActiveTaskTab("inProgress")}
                  >
                    In Progress ({tasks.inProgress.length})
                  </div>
                  <div
                    className={`tab-title ${activeTaskTab === "done" ? "active" : ""}`}
                    onClick={() => setActiveTaskTab("done")}
                  >
                    Completed ({tasks.done.length})
                  </div>
                </div>
                <div className={`task-content ${activeTaskTab !== "todo" ? "hidden" : ""}`}>
                  <TaskList tasks={tasks.todo} printTasks={printTasks} />
                </div>
                <div
                  className={`task-content ${activeTaskTab !== "inProgress" ? "hidden" : ""}`}
                >
                  <TaskList tasks={tasks.inProgress} printTasks={printTasks} />
                </div>
                <div className={`task-content ${activeTaskTab !== "done" ? "hidden" : ""}`}>
                  <TaskList tasks={tasks.done} printTasks={printTasks} />
                </div>
              </div>
            ) : (
              <>
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
                  columns={activeTab === "service" ? serviceColumns : checkacceptColumns}
                  dataSource={activeTab === "service" ? services : checkaccepts}
                  onChange={handleChange}
                  pagination={{ pageSize: 10 }}
                  rowKey={(record) => record.key}
                />
              </>
            )}
          </Content>
        </Layout>
      </Layout>
      <Modal
        title={editingRecord ? `Edit Service` : `Add Service`}
        open={isModalVisible}
        onOk={handleModalOk}
        onCancel={handleModalCancel}
        okText={editingRecord ? "Update" : "Add"}
      >
        <Form form={form} layout="vertical" name="recordForm" onFinish={() => {}}>
          <Form.Item
            name="serviceName"
            label="Service Name"
            rules={[{ required: true, message: "Please input the service name!" }]}
          >
            <Input />
          </Form.Item>
          <Form.Item
            name="serviceDescription"
            label="Description"
            rules={[{ required: true, message: "Please input the description!" }]}
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
