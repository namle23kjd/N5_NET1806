import React, { useState } from 'react';
import { Link } from "react-router-dom";
import { Form, Input, Button, Row, Col, Card, Typography, message } from "antd";
import { EyeInvisibleOutlined, EyeTwoTone } from "@ant-design/icons";
import axios from "axios";

const { Title } = Typography;

function AccountSetting() {
  const [form] = Form.useForm();
  const [loading, setLoading] = useState(false);

  const handleChangePassword = async (values) => {
    setLoading(true);
    try {
      const userInfoString = localStorage.getItem("user-info");
      const userInfo = JSON.parse(userInfoString);
      const token = userInfo?.data?.token;

      if (!token) {
        message.error("You must be logged in");
        window.location.href = "/login"; // Redirect to login page
        return;
      }

      const apiUrl = `https://localhost:7150/api/Auth/change-password`;
      const data = {
        email: userInfo.data.user.email,
        currentPassword: values.currentPassword,
        newPassword: values.newPassword,
      };

      const response = await axios({
        method: "POST",
        url: apiUrl,
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        data: data,
      });

      if (response.status === 200) {
        message.success("Password changed successfully");
        form.resetFields([
          "currentPassword",
          "newPassword",
          "confirmPassword",
        ]);
      } else {
        message.error(response.data.message || "Error changing password");
      }
    } catch (error) {
      if (error.response && error.response.status === 401) {
        message.error("Session expired. Please log in again.");
        localStorage.removeItem("user-info"); // Clear expired token
        window.location.href = "/login"; // Redirect to login page
      } else {
        message.error(error.response?.data?.message || "Current password wrong to change");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <link
        rel="stylesheet"
        href="src/assets/vendor/css/rtl/core.css"
        className="template-customizer-core-css"
      />
      <link
        rel="stylesheet"
        href="src/assets/vendor/css/rtl/theme-default.css"
        className="template-customizer-theme-css"
      />
      <link
        rel="stylesheet"
        href="src/assets/vendor/css/pages/page-account-settings.css"
      />

      <div className="layout-wrapper layout-content-navbar">
        <div className="layout-container">
          <div className="layout-page">
            <nav
              className="layout-navbar container-xxl navbar navbar-expand-xl navbar-detached align-items-center bg-navbar-theme"
              id="layout-navbar"
            >
              <div className="layout-menu-toggle navbar-nav align-items-xl-center me-3 me-xl-0 d-xl-none">
                <a
                  className="nav-item nav-link px-0 me-xl-4"
                  href="javascript:void(0)"
                >
                  <i className="ti ti-menu-2 ti-sm" />
                </a>
              </div>
              <div
                className="navbar-nav-right d-flex align-items-center"
                id="navbar-collapse"
              >
                <ul className="navbar-nav flex-row align-items-center ms-auto">
                  <li className="nav-item navbar-dropdown dropdown-user dropdown">
                    <a
                      className="nav-link dropdown-toggle hide-arrow"
                      href="javascript:void(0);"
                      data-bs-toggle="dropdown"
                    >
                      <div className="avatar avatar-online">
                        <img
                          src="src/assets/images/avatars/1.png"
                          alt="User avatar"
                          className="h-auto rounded-circle"
                        />
                      </div>
                    </a>
                    <ul className="dropdown-menu dropdown-menu-end">
                      <li>
                        <Link className="dropdown-item" to="/account">
                          <div className="d-flex">
                            <div className="flex-shrink-0 me-3">
                              <div className="avatar avatar-online">
                                <img
                                  src="src/assets/images/avatars/1.png"
                                  alt="User avatar"
                                  className="h-auto rounded-circle"
                                />
                              </div>
                            </div>
                            <div className="flex-grow-1">
                              <span className="fw-medium d-block">
                                John Doe
                              </span>
                              <small className="text-muted">Admin</small>
                            </div>
                          </div>
                        </Link>
                      </li>
                      <li>
                        <div className="dropdown-divider" />
                      </li>
                      <li>
                        <Link className="dropdown-item" to="/Pet">
                          <i className="ti ti-user-check me-2 ti-sm" />
                          <span className="align-middle">My Pet</span>
                        </Link>
                      </li>
                      <li>
                        <Link className="dropdown-item" to="/Cart">
                          <i className="ti ti-settings me-2 ti-sm" />
                          <span className="align-middle">Cart</span>
                        </Link>
                      </li>
                      <li>
                        <a
                          className="dropdown-item"
                          href="auth-login-cover.html"
                          target="_blank"
                        >
                          <i className="ti ti-logout me-2 ti-sm" />
                          <span className="align-middle">Log Out</span>
                        </a>
                      </li>
                    </ul>
                  </li>
                </ul>
              </div>
            </nav>

            <div className="content-wrapper">
              <div className="container-xxl flex-grow-1 container-p-y">
                <h4 className="py-3 mb-4">
                  <span className="ti-xs ti ti-lock me-1">
                    <Link to="/account">Account Settings</Link> /
                  </span>{" "}
                  Security
                </h4>
                <div className="row">
                  <div className="col-md-12">
                    <ul className="nav nav-pills flex-column flex-md-row mb-4">
                      <li className="nav-item">
                        <Link className="nav-link" to="/account">
                          <i className="ti-xs ti ti-users me-1" /> Account
                        </Link>
                      </li>
                      <li className="nav-item">
                        <a
                          className="nav-link active"
                          href="javascript:void(0);"
                        >
                          <i className="ti-xs ti ti-lock me-1" /> Security
                        </a>
                      </li>
                    </ul>

                    <Card className="mb-4">
                      <Title level={5} className="card-header">
                        Change Password
                      </Title>
                      <div className="card-body">
                        <Form
                          form={form}
                          onFinish={handleChangePassword}
                          layout="vertical"
                        >
                          <Row gutter={16}>
                            <Col span={12}>
                              <Form.Item
                                label="Current Password"
                                name="currentPassword"
                                rules={[
                                  {
                                    required: true,
                                    message: "Please input your current password!",
                                  },
                                ]}
                                className="form-password-toggle"
                              >
                                <Input.Password
                                  data-testid="currentPassword"
                                  placeholder="············"
                                  iconRender={(visible) =>
                                    visible ? (
                                      <EyeTwoTone />
                                    ) : (
                                      <EyeInvisibleOutlined />
                                    )
                                  }
                                />
                              </Form.Item>
                            </Col>
                          </Row>
                          <Row gutter={16}>
                            <Col span={12}>
                              <Form.Item
                                label="New Password"
                                name="newPassword"
                                rules={[
                                  {
                                    required: true,
                                    message: "Please input your new password!",
                                  },
                                  {
                                    min: 6,
                                    message: "Password must be at least 6 characters long!",
                                  },
                                  {
                                    max: 30,
                                    message: "Password must not exceed 30 characters!",
                                  },
                                ]}
                                className="form-password-toggle"
                              >
                                <Input.Password
                                  data-testid="newPassword"
                                  placeholder="············"
                                  iconRender={(visible) =>
                                    visible ? (
                                      <EyeTwoTone />
                                    ) : (
                                      <EyeInvisibleOutlined />
                                    )
                                  }
                                />
                              </Form.Item>
                            </Col>
                            <Col span={12}>
                              <Form.Item
                                label="Confirm New Password"
                                name="confirmPassword"
                                dependencies={['newPassword']}
                                rules={[
                                  {
                                    required: true,
                                    message: "Please confirm your new password!",
                                  },
                                  ({ getFieldValue }) => ({
                                    validator(_, value) {
                                      if (!value || getFieldValue('newPassword') === value) {
                                        return Promise.resolve();
                                      }
                                      return Promise.reject(new Error('The two passwords do not match!'));
                                    },
                                  }),
                                ]}
                                className="form-password-toggle"
                              >
                                <Input.Password
                                  data-testid="confirmPassword"
                                  placeholder="············"
                                  iconRender={(visible) =>
                                    visible ? (
                                      <EyeTwoTone />
                                    ) : (
                                      <EyeInvisibleOutlined />
                                    )
                                  }
                                />
                              </Form.Item>
                            </Col>
                          </Row>
                          <div className="col-12 mb-4">
                            <Title level={6}>Password Requirements:</Title>
                            <ul className="ps-3 mb-0">
                              <li className="mb-1">
                                Minimum 6 characters long - the more, the better
                              </li>
                              <li className="mb-1">
                                At least one lowercase character
                              </li>
                              <li>
                                At least one number, symbol, or whitespace character
                              </li>
                            </ul>
                          </div>
                          <Form.Item>
                            <Button
                              type="primary"
                              htmlType="submit"
                              loading={loading}
                              className="me-2"
                            >
                              Save changes
                            </Button>
                            <Button type="default" htmlType="reset">
                              Cancel
                            </Button>
                          </Form.Item>
                        </Form>
                      </div>
                    </Card>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="layout-overlay layout-menu-toggle" />
        <div className="drag-target" />
      </div>
    </div>
  );
}

export default AccountSetting;
