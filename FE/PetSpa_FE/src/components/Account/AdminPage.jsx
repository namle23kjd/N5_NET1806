import React, { useState, useEffect } from "react";
import axios from "axios";
import {
  Table,
  Space,
  Input,
  Select,
  Form,
  Button,
  Typography,
  Layout,
  Row,
  Modal,
  message,
  Popconfirm,
  Card,
  Col,
  DatePicker,
} from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  PlusOutlined,
  PieChartOutlined,
  CloseCircleOutlined,
} from "@ant-design/icons";
import { PieChart, Pie, Cell, Tooltip, Legend } from "recharts";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faWallet,
  faUsers,
  faTasks,
  faExclamationTriangle,
  faServer,
} from "@fortawesome/free-solid-svg-icons";
import moment from "moment";

const { Option } = Select;
const { Title } = Typography;
const { Content } = Layout;
const { RangePicker } = DatePicker;

const COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042", "#AF19FF"];

const AdminPage = () => {
  const [accounts, setAccounts] = useState([]);
  const [originalAccounts, setOriginalAccounts] = useState([]);
  const [formData, setFormData] = useState({
    userName: "",
    email: "",
    phoneNumber: "",
    role: "",
    fullName: "",
    gender: "Male",
  });
  const [isEditing, setIsEditing] = useState(false);
  const [editId, setEditId] = useState(null);
  const [showForm, setShowForm] = useState(false);
  const [error, setError] = useState("");
  const [filteredInfo, setFilteredInfo] = useState({});
  const [sortedInfo, setSortedInfo] = useState({});
  const [searchPhone, setSearchPhone] = useState("");
  const [searchRole, setSearchRole] = useState("");
  const [searchMonth, setSearchMonth] = useState("");
  const [invoiceSearchMonth, setInvoiceSearchMonth] = useState("");
  const [totalAccounts, setTotalAccounts] = useState(0);
  const [rolePercentages, setRolePercentages] = useState({});
  const [showDashboard, setShowDashboard] = useState(false);
  const [showInvoiceScreen, setShowInvoiceScreen] = useState(false);
  const [dashboardData, setDashboardData] = useState({
    totalRevenue: 0,
    totalUsers: 0,
    newUsers: 0,
    serverUptime: 0,
    toDoList: 0,
    issues: 0,
  });

  const [invoiceData, setInvoiceData] = useState([]);
  const [originalInvoiceData, setOriginalInvoiceData] = useState([]);
  const [totalInvoiceAmount, setTotalInvoiceAmount] = useState(0);

  const fetchDeniedBookings = async () => {
    try {
      const response = await axios.get(
        "https://localhost:7150/api/Booking/bookings/deny"
      );
      const { count } = response.data.data;
      setDashboardData((prevData) => ({
        ...prevData,
        deniedBookings: count,
      }));
    } catch (error) {
      console.error("Error fetching denied bookings:", error);
    }
  };

  const fetchBookingData = async () => {
    try {
      const response = await axios.get(
        "https://localhost:7150/api/Booking/bookings/status-2"
      );
      const { count } = response.data.data;
      setDashboardData((prevData) => ({
        ...prevData,
        issues: count,
      }));
    } catch (error) {
      console.error("Error fetching booking data:", error);
    }
  };

  const fetchTotalRevenue = async () => {
    try {
      const response = await axios.get(
        "https://localhost:7150/api/Booking/total-revenue/from-start"
      );
      const { totalAmount } = response.data.data;
      setDashboardData((prevData) => ({
        ...prevData,
        totalRevenue: totalAmount,
      }));
    } catch (error) {
      console.error("Error fetching total revenue:", error);
    }
  };

  const fetchCompletedBookings = async () => {
    try {
      const response = await axios.get(
        "https://localhost:7150/api/Booking/completed"
      );
      const { count } = response.data.data;
      setDashboardData((prevData) => ({
        ...prevData,
        completedBookings: count,
      }));
    } catch (error) {
      console.error("Error fetching completed bookings:", error);
    }
  };

  const fetchInvoiceData = async () => {
    try {
      const response = await axios.get(
        "https://localhost:7150/api/Payments/get-all-payments"
      );
      const payments = response.data;

      const formattedPayments = payments.map((payment) => {
        const bookingsWithStatus = payment.bookings.map((booking) => {
          const status =
            booking.status === 2
              ? "Refund"
              : booking.checkAccept === -1
              ? "Canceled"
              : "Paid";

          const serviceNames = booking.bookingDetails.map((detail) =>
            detail.comboName
              ? `${detail.comboName} - ${status}`
              : `${detail.serviceName} - ${status}`
          );

          return {
            ...booking,
            serviceNames: serviceNames.join("\n"),
            status: status,
          };
        });

        const serviceName = bookingsWithStatus.map((booking) => booking.serviceNames).join("\n");

        return {
          client: payment.customerName,
          amount: payment.totalPayment,
          status: serviceName,
          due: payment.createdDate,
        };
      });

      setInvoiceData(formattedPayments);
      setOriginalInvoiceData(formattedPayments);
      calculateTotalAmount(formattedPayments); // Calculate total amount
    } catch (error) {
      console.error("Error fetching invoice data:", error);
    }

  };

  useEffect(() => {
    fetchAccounts();
    fetchBookingData();
    fetchDashboardData();
    fetchInactiveUsers();
    fetchTotalRevenue();
    fetchCompletedBookings();
    fetchDeniedBookings();
    fetchInvoiceData();
  }, []);

  useEffect(() => {
    fetchDashboardData();
    fetchInactiveUsers();
  }, []);

  const fetchAccounts = async () => {
    try {
      const response = await axios.get("https://localhost:7150/api/Account");
      const accountsData = response.data;

      const filteredAccounts = accountsData.filter(
        (account) =>
          !account.roles.some((role) => role.toLowerCase() === "admin")
      );
      setAccounts(filteredAccounts);
      setOriginalAccounts(filteredAccounts);
      calculateStatistics(filteredAccounts);

      setTotalAccounts(accountsData.length);
    } catch (error) {
      console.error(error);
    }
  };

  const fetchDashboardData = async () => {
    try {
      const [startDate, endDate] = dateRange;
      const response = await axios.get("https://localhost:7150/api/Dashboard", {
        params: {
          startDate: startDate.format("YYYY-MM-DD"),
          endDate: endDate.format("YYYY-MM-DD"),
        },
      });
      setDashboardData(response.data);
    } catch (error) {
      console.error(error);
    }
  };

  const fetchInactiveUsers = async () => {
    try {
      const response = await axios.get(
        "https://localhost:7150/api/Account/count-inactive-users"
      );
      const { count } = response.data;
      setDashboardData((prevData) => ({
        ...prevData,
        inactiveUsers: count,
      }));
    } catch (error) {
      console.error("Error fetching inactive users:", error);
    }
  };

  const calculateStatistics = (accountsData) => {
    const total = accountsData.length;
    setTotalAccounts(total);

    const roleCounts = accountsData.reduce((acc, account) => {
      account.roles.forEach((role) => {
        acc[role] = (acc[role] || 0) + 1;
      });
      return acc;
    }, {});

    const percentages = Object.keys(roleCounts).reduce((acc, role) => {
      acc[role] = ((roleCounts[role] / total) * 100).toFixed(2);
      return acc;
    }, {});

    setRolePercentages(percentages);
  };

  const calculateTotalAmount = (invoices) => {
    const total = invoices.reduce((sum, invoice) => sum + invoice.amount, 0);
    setTotalInvoiceAmount(total);
  };

  const handleInputChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };
  const [form] = Form.useForm();

  const validateForm = () => {
    const duplicateEmail = accounts.some(
      (account) => account.email === formData.email && account.id !== editId
    );
    const duplicateUserName = accounts.some(
      (account) =>
        account.userName === formData.userName && account.id !== editId
    );
    const duplicatePhoneNumber = accounts.some(
      (account) =>
        account.phoneNumber === formData.phoneNumber && account.id !== editId
    );

    if (duplicateEmail) {
      setError("Email is already in use.");
      return false;
    }

    if (duplicateUserName) {
      setError("Username is already in use.");
      return false;
    }

    if (duplicatePhoneNumber) {
      setError("Phone number is already in use.");
      return false;
    }

    setError("");
    return true;
  };

  const handleSubmit = async () => {
    form.validateFields().then(async () => {
      if (!validateForm()) {
        return;
      }

      try {
        if (isEditing) {
          const updateData = {
            id: editId,
            userName: formData.userName,
            email: formData.email,
            phoneNumber: formData.phoneNumber,
            role: formData.role,
          };
          await axios.put(
            "https://localhost:7150/api/Account/update-user",
            updateData
          );
          setIsEditing(false);
          setEditId(null);
        } else {
          const submitData = { ...formData, password: "Pet123@@" };
          await axios.post(
            "https://localhost:7150/api/Account/register",
            submitData
          );
        }
        message.success(
          isEditing
            ? "Account updated successfully"
            : "Account added successfully"
        );
        setFormData({
          userName: "",
          email: "",
          phoneNumber: "",
          role: "",
          fullName: "",
          gender: "Male",
        });
        setShowForm(false);
        fetchAccounts();
      } catch (error) {
        message.error("An error occurred. Please try again.");
        console.error(error);
      }
    });
  };

  const handleEdit = (account) => {
    setIsEditing(true);
    setEditId(account.id);
    setFormData({
      userName: account.userName,
      email: account.email,
      phoneNumber: account.phoneNumber,
      role: account.roles.join(", "),
      fullName: account.fullName,
      gender: account.gender,
    });
    form.setFieldsValue({
      userName: account.userName,
      email: account.email,
      phoneNumber: account.phoneNumber,
      role: account.roles.join(", "),
      fullName: account.fullName,
      gender: account.gender,
    });
    setShowForm(true);
  };

  const handleDelete = async (id) => {
    try {
      await axios.delete(
        `https://localhost:7150/api/Account/delete-account/${id}`
      );
      message.success("Account deleted successfully");
      fetchAccounts();
    } catch (error) {
      message.error("An error occurred. Please try again.");
      console.error(error);
    }
  };

  const handleAddNew = () => {
    setIsEditing(false);
    setEditId(null);
    setFormData({
      userName: "",
      email: "",
      phoneNumber: "",
      role: "",
      fullName: "",
      gender: "Male",
    });
    form.resetFields();
    setShowForm(true);
  };

  const handleCancel = () => {
    setShowForm(false);
  };

  const handleChange = (pagination, filters, sorter) => {
    setFilteredInfo(filters);
    setSortedInfo(sorter);
  };

  const handleSearchPhone = (e) => {
    const { value } = e.target;
    setSearchPhone(value);
    if (value === "") {
      setAccounts(originalAccounts);
    } else {
      const filteredData = originalAccounts.filter((account) =>
        account.phoneNumber.includes(value)
      );
      setAccounts(filteredData);
    }
  };

  const handleInvoiceSearchMonth = (value) => {
    setInvoiceSearchMonth(value);
    if (!value) {
      setInvoiceData(originalInvoiceData);
    } else {
      const filteredData = originalInvoiceData.filter((invoice) => {
        const dueDate = moment(invoice.due);
        return dueDate.isValid() && dueDate.format("MM-YYYY") === moment(value).format("MM-YYYY");
      });
      setInvoiceData(filteredData);
      calculateTotalAmount(filteredData); // Recalculate total amount
    }
  };
  

  const columns = [
    {
      title: "Username",
      dataIndex: "userName",
      key: "userName",
      sorter: (a, b) => a.userName.length - b.userName.length,
      sortOrder: sortedInfo.columnKey === "userName" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Email",
      dataIndex: "email",
      key: "email",
      sorter: (a, b) => a.email.length - b.email.length,
      sortOrder: sortedInfo.columnKey === "email" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Phone Number",
      dataIndex: "phoneNumber",
      key: "phoneNumber",
      sorter: (a, b) => a.phoneNumber.localeCompare(b.phoneNumber),
      sortOrder:
        sortedInfo.columnKey === "phoneNumber" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Role",
      dataIndex: "roles",
      key: "role",
      render: (roles) => roles.join(", "),
      filters: Array.from(
        new Set(accounts.flatMap((account) => account.roles))
      ).map((role) => ({
        text: role,
        value: role,
      })),
      filteredValue: filteredInfo.role || null,
      onFilter: (value, record) => record.roles.includes(value),
      sorter: (a, b) => a.roles.join(", ").localeCompare(b.roles.join(", ")),
      sortOrder: sortedInfo.columnKey === "role" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Actions",
      key: "actions",
      render: (text, record) => (
        <div style={{ display: "flex", gap: "4px" }}>
          <Button icon={<EditOutlined />} onClick={() => handleEdit(record)} />
          <Popconfirm
            title="Are you sure delete this account?"
            onConfirm={() => handleDelete(record.id)}
            okText="Yes"
            cancelText="No"
          >
            <Button icon={<DeleteOutlined />} />
          </Popconfirm>
        </div>
      ),
    },
  ];

  const emailValidator = (_, value) => {
    if (!value || value.endsWith("@gmail.com")) {
      return Promise.resolve();
    }
    return Promise.reject(new Error("Email must end with @gmail.com"));
  };

  const data = Object.keys(rolePercentages).map((role, index) => ({
    name: role,
    value: parseFloat(rolePercentages[role]),
  }));

  return (
    <Layout>
      <Content style={{ padding: "50px" }}>
        <Title level={2} align="center">
          Admin Page
        </Title>
        <Row justify="end" style={{ marginBottom: "20px" }}>
          <Button
            type="primary"
            icon={<PlusOutlined />}
            onClick={handleAddNew}
            style={{ marginRight: "10px" }}
          >
            Add New
          </Button>
          <Button
            type="primary"
            icon={<PieChartOutlined />}
            onClick={() => setShowDashboard(!showDashboard)}
            style={{ marginRight: "10px" }}
          >
            {showDashboard ? "Hide Dashboard" : "Show Dashboard"}
          </Button>
          <Button
            type="primary"
            icon={<PieChartOutlined />}
            onClick={() => setShowInvoiceScreen(!showInvoiceScreen)}
          >
            {showInvoiceScreen ? "Hide Invoice Screen" : "Show Invoice Screen"}
          </Button>
        </Row>
        {showForm && (
          <Modal
            title={isEditing ? "Edit Account" : "Add Account"}
            visible={showForm}
            onCancel={handleCancel}
            footer={[
              <Button
                key="cancel"
                onClick={handleCancel}
                icon={<CloseCircleOutlined />}
              >
                Cancel
              </Button>,
              <Button key="submit" type="primary" onClick={form.submit}>
                {isEditing ? "Update" : "Add"}
              </Button>,
            ]}
          >
            <Form
              layout="vertical"
              initialValues={formData}
              form={form}
              onFinish={handleSubmit}
            >
              <Form.Item
                label="Username"
                name="userName"
                rules={[{ required: true, message: "Please enter username" }]}
              >
                <Input
                  name="userName"
                  value={formData.userName}
                  onChange={handleInputChange}
                />
              </Form.Item>
              <Form.Item
                label="Email"
                name="email"
                rules={[
                  { required: true, message: "Please enter email" },
                  { type: "email", message: "Please enter a valid email" },
                  { validator: emailValidator },
                ]}
              >
                <Input
                  name="email"
                  value={formData.email}
                  onChange={handleInputChange}
                />
              </Form.Item>
              <Form.Item
                label="Phone Number"
                name="phoneNumber"
                rules={[
                  { required: true, message: "Please enter phone number" },
                ]}
              >
                <Input
                  name="phoneNumber"
                  value={formData.phoneNumber}
                  onChange={handleInputChange}
                />
              </Form.Item>
              <Form.Item
                label="Role"
                name="role"
                rules={[{ required: true, message: "Please select a role" }]}
              >
                <Select
                  value={formData.role}
                  onChange={(value) =>
                    handleInputChange({ target: { name: "role", value } })
                  }
                >
                  <Option value="customer">Customer</Option>
                  <Option value="manager">Manager</Option>
                  <Option value="staff">Staff</Option>
                  {/* <Option value="admin">Admin</Option> */}
                </Select>
              </Form.Item>

              {!isEditing && (
                <>
                  <Form.Item
                    label="Full Name"
                    name="fullName"
                    rules={[
                      { required: true, message: "Please enter full name" },
                    ]}
                  >
                    <Input
                      name="fullName"
                      value={formData.fullName}
                      onChange={handleInputChange}
                    />
                  </Form.Item>
                  <Form.Item
                    label="Gender"
                    name="gender"
                    rules={[{ required: true, message: "Please enter gender" }]}
                  >
                    <Select
                      defaultValue={formData.gender}
                      onChange={(value) =>
                        handleInputChange({ target: { name: "gender", value } })
                      }
                    >
                      <Option value="Male">Male</Option>
                      <Option value="Female">Female</Option>
                      <Option value="Other">Other</Option>
                    </Select>
                  </Form.Item>
                </>
              )}
            </Form>
            {error && <p style={{ color: "red" }}>{error}</p>}
          </Modal>
        )}
        {showDashboard && (
          <div style={{ marginBottom: "20px" }}>
            <Title level={4}>Dashboard</Title>
            <Row gutter={16}>
              <Col span={12}>
                <div>
                  <p>Total Accounts (including admin): {totalAccounts}</p>
                  <PieChart width={500} height={400}>
                    <Pie
                      data={data}
                      cx={250}
                      cy={200}
                      labelLine={false}
                      label={({ name, percent }) =>
                        `${name}: ${(percent * 100).toFixed(2)}%`
                      }
                      outerRadius={150}
                      fill="#8884d8"
                      dataKey="value"
                    >
                      {data.map((entry, index) => (
                        <Cell
                          key={`cell-${index}`}
                          fill={COLORS[index % COLORS.length]}
                        />
                      ))}
                    </Pie>
                    <Tooltip />
                    <Legend />
                  </PieChart>
                </div>
              </Col>
              <Col span={12}>
                <Row gutter={16}>
                  <Col span={12}>
                    <Card
                      bordered={false}
                      style={{
                        backgroundColor: "#D9FEE7",
                        textAlign: "center",
                        marginBottom: "20px",
                      }}
                    >
                      <FontAwesomeIcon
                        icon={faWallet}
                        size="2x"
                        style={{ color: "#4CAF50" }}
                      />
                      <div style={{ fontSize: "24px", fontWeight: "bold" }}>
                        {dashboardData.totalRevenue.toLocaleString("vi-VN", {
                          style: "currency",
                          currency: "VND",
                        })}
                      </div>
                      <div>Total Revenue</div>
                    </Card>
                  </Col>
                  <Col span={12}>
                    <Card
                      bordered={false}
                      style={{
                        backgroundColor: "#FCE4EC",
                        textAlign: "center",
                        marginBottom: "20px",
                      }}
                    >
                      <FontAwesomeIcon
                        icon={faUsers}
                        size="2x"
                        style={{ color: "#F06292" }}
                      />
                      <div style={{ fontSize: "24px", fontWeight: "bold" }}>
                        {totalAccounts}
                      </div>
                      <div>Total Users (including admin)</div>
                    </Card>
                  </Col>
                </Row>
                <Row gutter={16}>
                  <Col span={12}>
                    <Card
                      bordered={false}
                      style={{
                        backgroundColor: "#FCE4EC",
                        textAlign: "center",
                        marginBottom: "20px",
                      }}
                    >
                      <FontAwesomeIcon
                        icon={faUsers}
                        size="2x"
                        style={{ color: "#F06292" }}
                      />
                      <div style={{ fontSize: "24px", fontWeight: "bold" }}>
                        {dashboardData.inactiveUsers}
                      </div>
                      <div>Banned Accounts</div>
                    </Card>
                  </Col>
                  <Col span={12}>
                    <Card
                      bordered={false}
                      style={{
                        backgroundColor: "#EDE7F6",
                        textAlign: "center",
                        marginBottom: "20px",
                      }}
                    >
                      <FontAwesomeIcon
                        icon={faTasks}
                        size="2x"
                        style={{ color: "#7E57C2" }}
                      />
                      <div style={{ fontSize: "24px", fontWeight: "bold" }}>
                        {dashboardData.completedBookings} tasks
                      </div>
                      <div>Completed Bookings</div>
                    </Card>
                  </Col>
                </Row>
                <Row gutter={16}>
                  <Col span={12}>
                    <Card
                      bordered={false}
                      style={{
                        backgroundColor: "#E3F2FD",
                        textAlign: "center",
                        marginBottom: "20px",
                      }}
                    >
                      <FontAwesomeIcon
                        icon={faServer}
                        size="2x"
                        style={{ color: "#42A5F5" }}
                      />
                      <div style={{ fontSize: "24px", fontWeight: "bold" }}>
                        {dashboardData.issues}
                      </div>
                      <div>Deny bookings by Managers</div>
                    </Card>
                  </Col>
                  <Col span={12}>
                    <Card
                      bordered={false}
                      style={{
                        backgroundColor: "#FFEBEE",
                        textAlign: "center",
                        marginBottom: "20px",
                      }}
                    >
                      <FontAwesomeIcon
                        icon={faExclamationTriangle}
                        size="2x"
                        style={{ color: "#EF5350" }}
                      />
                      <div style={{ fontSize: "24px", fontWeight: "bold" }}>
                        {dashboardData.deniedBookings}
                      </div>
                      <div>Cancel bookings by Customers</div>
                    </Card>
                  </Col>
                </Row>
              </Col>
            </Row>
          </div>
        )}
        {showInvoiceScreen && (
          <div style={{ marginBottom: "20px" }}>
            <Title level={4}>Invoice Screen</Title>
            <Row gutter={16}>
              <Col span={24}>
                <Space style={{ marginBottom: 16 }}>
                  <DatePicker
                    picker="month"
                    placeholder="Search by Due Month"
                    onChange={(date, dateString) =>
                      handleInvoiceSearchMonth(dateString)
                    }
                    style={{ width: 200, marginRight: 16 }}
                  />
                  <Button
                    onClick={() => {
                      setInvoiceSearchMonth("");
                      setInvoiceData(originalInvoiceData);
                    }}
                  >
                    Clear filters
                  </Button>
                </Space>
                <Card bordered={false}>
                  <div className="invoice-screen">
                    <Table
                      dataSource={invoiceData}
                      columns={[
                        {
                          title: "Customer Name",
                          dataIndex: "client",
                          key: "client",
                        },
                        {
                          title: "Amount",
                          dataIndex: "amount",
                          key: "amount",
                          render: (amount) =>
                            amount.toLocaleString("vi-VN", {
                              style: "currency",
                              currency: "VND",
                            }),
                        },
                        {
                          title: "Status",
                          dataIndex: "status",
                          key: "status",
                          render: (status) =>
                            status.split("\n").map((line, index) => {
                              let color;
                              if (line.includes("Paid")) {
                                color = "green";
                              } else if (line.includes("Refund")) {
                                color = "red";
                              } else if (line.includes("Canceled")) {
                                color = "yellow";
                              }
                              return (
                                <div key={index} style={{ color }}>
                                  {line}
                                </div>
                              );
                            }),
                        },
                        {
                          title: "Due",
                          dataIndex: "due",
                          key: "due",
                          sorter: (a, b) =>
                            moment(a.due, "YYYY-MM-DD HH:mm:ss").unix() -
                            moment(b.due, "YYYY-MM-DD HH:mm:ss").unix(),
                          sortOrder:
                            sortedInfo.columnKey === "due"
                              ? sortedInfo.order
                              : null,
                        },
                      ]}
                    />
                    <div
                      style={{
                        textAlign: "right",
                        marginTop: "20px",
                        fontWeight: "bold",
                      }}
                    >
                      Total Amount: {totalInvoiceAmount.toLocaleString("vi-VN", {
                        style: "currency",
                        currency: "VND",
                      })}
                    </div>
                  </div>
                </Card>
              </Col>
            </Row>
          </div>
        )}
        <Space style={{ marginBottom: 16 }}>
          <Input
            placeholder="Search by Phone Number"
            value={searchPhone}
            onChange={handleSearchPhone}
            style={{ width: 200, marginRight: 16 }}
          />
          <Button
            onClick={() => {
              setFilteredInfo({});
              setSortedInfo({});
              setAccounts(originalAccounts);
              setSearchPhone("");
              setSearchRole("");
              setSearchMonth("");
            }}
          >
            Clear filters and sorters
          </Button>
        </Space>
        <Table
          columns={columns}
          dataSource={accounts}
          onChange={handleChange}
          rowKey="id"
          pagination={{ pageSize: 10 }}
          bordered
        />
      </Content>
    </Layout>
  );
};

export default AdminPage;
