import React, { useState, useEffect } from "react";
import axios from "axios";
import {
  TextField,
  Button,
  Container,
  Typography,
  Paper,
  Box,
  IconButton,
} from "@mui/material";
import { Edit, Delete, Add, Cancel } from "@mui/icons-material";
import { styled } from "@mui/material/styles";
import { Table, Space, Input } from "antd";
import { PieChart, Pie, Cell, Tooltip, Legend } from "recharts";

const COLORS = ["#0088FE", "#00C49F", "#FFBB28", "#FF8042", "#AF19FF"];

const FormContainer = styled("form")(({ theme }) => ({
  display: "flex",
  flexDirection: "column",
  gap: theme.spacing(2),
  marginBottom: theme.spacing(4),
}));

const FormField = ({
  label,
  name,
  type = "text",
  value,
  onChange,
  required,
}) => (
  <TextField
    label={label}
    name={name}
    type={type}
    value={value}
    onChange={onChange}
    required={required}
    variant="outlined"
    fullWidth
  />
);

const TableCellActions = styled("div")({
  display: "flex",
  justifyContent: "left",
});

const ActionButton = styled(IconButton)(({ theme }) => ({
  marginRight: theme.spacing(0),
}));

const AdminPage = () => {
  const [accounts, setAccounts] = useState([]);
  const [originalAccounts, setOriginalAccounts] = useState([]);
  const [formData, setFormData] = useState({
    userName: "",
    email: "",
    phoneNumber: "",
    role: "",
    fullName: "",
    gender: "",
  });
  const [isEditing, setIsEditing] = useState(false);
  const [editId, setEditId] = useState(null);
  const [showForm, setShowForm] = useState(false);
  const [error, setError] = useState("");
  const [filteredInfo, setFilteredInfo] = useState({});
  const [sortedInfo, setSortedInfo] = useState({});
  const [searchPhone, setSearchPhone] = useState("");
  const [searchRole, setSearchRole] = useState("");
  const [totalAccounts, setTotalAccounts] = useState(0);
  const [rolePercentages, setRolePercentages] = useState({});
  const [showDashboard, setShowDashboard] = useState(false);

  useEffect(() => {
    fetchAccounts();
  }, []);

  const fetchAccounts = async () => {
    try {
      const response = await axios.get("https://localhost:7150/api/Account");
      const accountsData = response.data;
      setAccounts(accountsData);
      setOriginalAccounts(accountsData);
      calculateStatistics(accountsData);
    } catch (error) {
      console.error(error);
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

  const handleInputChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

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

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    const submitData = { ...formData, password: "Pet123@@" };

    if (isEditing) {
      await axios.put(`https://localhost:7150/api/Account/update-user/${editId}`, submitData);
      setIsEditing(false);
      setEditId(null);
    } else {
      await axios.post("https://localhost:7150/api/Account/register", submitData);
    }
    setFormData({
      userName: "",
      email: "",
      phoneNumber: "",
      role: "",
      fullName: "",
      gender: "",
    });
    setShowForm(false);
    fetchAccounts();
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
    setShowForm(true);
  };

  const handleDelete = async (id) => {
    await axios.delete(`https://localhost:7150/api/Account/delete-account/${id}`);
    fetchAccounts();
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
      gender: "",
    });
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

  const handleSearchRole = (e) => {
    const { value } = e.target;
    setSearchRole(value);
    if (value === "") {
      setAccounts(originalAccounts);
    } else {
      const filteredData = originalAccounts.filter((account) =>
        account.roles.some((role) => role.toLowerCase().includes(value.toLowerCase()))
      );
      setAccounts(filteredData);
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
      sortOrder: sortedInfo.columnKey === "phoneNumber" ? sortedInfo.order : null,
      ellipsis: true,
    },
    {
      title: "Role",
      dataIndex: "roles",
      key: "role",
      render: (roles) => roles.join(", "),
      filters: Array.from(new Set(accounts.flatMap((account) => account.roles))).map((role) => ({
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
        <TableCellActions>
          <ActionButton onClick={() => handleEdit(record)} color="primary">
            <Edit />
          </ActionButton>
          <IconButton onClick={() => handleDelete(record.id)} color="secondary">
            <Delete />
          </IconButton>
        </TableCellActions>
      ),
    },
  ];

  const data = Object.keys(rolePercentages).map((role, index) => ({
    name: role,
    value: parseFloat(rolePercentages[role]),
  }));

  return (
    <Container>
      {showForm && (
        <Container maxWidth="sm" style={{ marginTop: "40px" }}>
          <Paper elevation={3} style={{ padding: "20px" }}>
            <Typography variant="h4" gutterBottom align="center">
              Admin Page
            </Typography>
            {error && (
              <Typography variant="body2" color="error" align="center">
                {error}
              </Typography>
            )}
            <FormContainer onSubmit={handleSubmit}>
              <FormField
                label="Username"
                name="userName"
                value={formData.userName}
                onChange={handleInputChange}
                required
              />
              <FormField
                label="Email"
                name="email"
                type="email"
                value={formData.email}
                onChange={handleInputChange}
                required
              />
              <FormField
                label="Phone Number"
                name="phoneNumber"
                value={formData.phoneNumber}
                onChange={handleInputChange}
                required
              />
              <FormField
                label="Role"
                name="role"
                value={formData.role}
                onChange={handleInputChange}
                required
              />
              <Box mt={2} display="flex" justifyContent="space-between">
                <Button type="submit" variant="contained" color="primary">
                  {isEditing ? "Update" : "Add"}
                </Button>
                <Button
                  variant="outlined"
                  color="secondary"
                  startIcon={<Cancel />}
                  onClick={handleCancel}
                >
                  Cancel
                </Button>
              </Box>
            </FormContainer>
          </Paper>
        </Container>
      )}
      <Box display="flex" justifyContent="flex-end" mb={2}>
        <Button
          variant="contained"
          color="primary"
          startIcon={<Add />}
          onClick={handleAddNew}
        >
          Add New
        </Button>
        <Button
          variant="contained"
          color="secondary"
          style={{ marginLeft: "16px" }}
          onClick={() => setShowDashboard(!showDashboard)}
        >
          {showDashboard ? "Hide Dashboard" : "Show Dashboard"}
        </Button>
      </Box>
      {showDashboard && (
        <Box mb={4}>
          <Typography variant="h6" gutterBottom>
            Dashboard
          </Typography>
          <Typography variant="body1">Total Accounts: {totalAccounts}</Typography>
          <PieChart width={1000} height={400}>
            <Pie
              data={data}
              cx={500}
              cy={200}
              labelLine={false}
              label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(2)}%`}
              outerRadius={150}
              fill="#8884d8"
              dataKey="value"
            >
              {data.map((entry, index) => (
                <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
              ))}
            </Pie>
            <Tooltip />
            <Legend />
          </PieChart>
        </Box>
      )}
      <Space
        style={{
          marginBottom: 16,
        }}
      >
        <Input
          placeholder="Search by Phone Number"
          value={searchPhone}
          onChange={handleSearchPhone}
          style={{ width: 200, marginRight: 16 }}
        />
        <Button onClick={() => { setFilteredInfo({}); setSortedInfo({}); setAccounts(originalAccounts); setSearchPhone(""); setSearchRole(""); }}>
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
        style={{ marginBottom: "20px", backgroundColor: "#fff", borderRadius: "10px" }}
      />
    </Container>
  );
};

export default AdminPage;
