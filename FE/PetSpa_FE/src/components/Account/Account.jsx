/* eslint-disable no-unused-vars */
import { Link } from "react-router-dom";
import { useState, useEffect } from "react";
import { Card, Alert, Checkbox } from "antd";
import {
  Form,
  Input,
  Button,
  message,
  Typography,
  Select,
  Row,
  Col,
} from "antd";
import axios from "axios";

const { Title } = Typography;

function Account() {
  const [form] = Form.useForm();

  const [loading, setLoading] = useState(false);
  const [loadingData, setLoadingData] = useState(true);
  const [accountDeactivated, setAccountDeactivated] = useState(false);
  const [accountActivationChecked, setAccountActivationChecked] =
    useState(false);
  const [fullName, setFullName] = useState();
  const [Role, setRole] = useState();

  useEffect(() => {
    const fetchUserInfo = async () => {
      const userInfoString = localStorage.getItem("user-info");
      if (!userInfoString) {
        message.error("No user information found");
        return;
      }
      const userInfo = JSON.parse(userInfoString);
      const token = userInfo?.data.token;

      if (!token) {
        message.error("You must be logged in");
        window.location.href = "/login"; // Redirect to login page
        return;
      }
      console.log();

      try {
        const response = await axios.get(
          `https://localhost:7150/api/Customer/${userInfo.data.user.id}`,
          {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (response.status === 200) {
          const userData = response.data;
          const fullName = userData.data.fullName;
          const user = JSON.parse(localStorage.getItem("user-info"));
          const Role = user.data.user.role;
          setFullName(fullName);
          setRole(Role);
          // Tách fullName thành firstName và lastName
          const nameParts = fullName.split(" ");
          const lastName = nameParts.pop();
          const firstName = nameParts.join(" ");
          form.setFieldsValue({
            firstName: firstName,
            lastName: lastName,
            email: userData.data.user.email,
            phoneNumber: userData.data.phoneNumber,
            gender: userData.data.gender,
          });
        } else {
          message.error("Failed to fetch user information");
        }
      } catch (error) {
        if (error.response && error.response.status === 401) {
          message.error("Session expired. Please log in again.");
          localStorage.removeItem("user-info"); // Clear expired token
          window.location.href = "/login"; // Redirect to login page
        } else {
          console.error("API error:", error);
          message.error("API error");
        }
      } finally {
        setLoadingData(false);
      }
    };

    fetchUserInfo();
  }, [form]);

  const handleUpdateProfile = async (values) => {
    setLoading(true);
    try {
      const userInfoString = localStorage.getItem("user-info");
      const userInfo = JSON.parse(userInfoString);
      const token = userInfo?.data.token;

      if (!token) {
        message.error("You must be logged in");
        window.location.href = "/login"; // Redirect to login page
        return;
      }

      const apiUrl = `https://localhost:7150/api/Customer/UpdateByUser/${userInfo.data.user.id}`;
      const data = {
        fullName: values.firstName + " " + values.lastName,
        ...values,
      };

      const response = await axios({
        method: "PUT",
        url: apiUrl,
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        data: data,
      });

      if (response.status === 200) {
        message.success("Profile updated successfully");
      } else {
        message.error(response.data.message || "Error updating profile");
      }
    } catch (error) {
      if (error.response && error.response.status === 401) {
        message.error("Session expired. Please log in again.");
        localStorage.removeItem("user-info"); // Clear expired token
        window.location.href = "/login"; // Redirect to login page
      } else {
        console.log(error.response);
        message.error(error.response.data.msg);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <meta charSet="utf-8" />
      <meta
        name="viewport"
        content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0"
      />
      <title>
        Account settings - Account | Vuexy - Bootstrap Admin Template
      </title>
      <meta
        name="description"
        content="Start your development with a Dashboard for Bootstrap 5"
      />
      <meta
        name="keywords"
        content="dashboard, bootstrap 5 dashboard, bootstrap 5 design, bootstrap 5"
      />
      {/* Canonical SEO */}
      {/* ? PROD Only: Google Tag Manager (Default ThemeSelection: GTM-5DDHKGP, PixInvent: GTM-5J3LMKC) */}
      {/* End Google Tag Manager */}
      {/* Favicon */}
      <link
        rel="icon"
        type="image/x-icon"
        href="src/assets/images/favicon/favicon.ico"
      />
      {/* Fonts */}
      <link rel="preconnect" href="https://fonts.googleapis.com" />
      <link rel="preconnect" href="https://fonts.gstatic.com" crossOrigin />
      <link
        href="https://fonts.googleapis.com/css2?family=Public+Sans:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,500;1,600;1,700&ampdisplay=swap"
        rel="stylesheet"
      />
      {/* Icons */}
      <link rel="stylesheet" href="src/assets/vendor/fonts/fontawesome.css" />
      <link rel="stylesheet" href="src/assets/vendor/fonts/tabler-icons.css" />
      <link rel="stylesheet" href="src/assets/vendor/fonts/flag-icons.css" />
      {/* Core CSS */}
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
      <link rel="stylesheet" href="src/assets/css/demo.css" />
      {/* Vendors CSS */}
      <link
        rel="stylesheet"
        href="src/assets/vendor/libs/node-waves/node-waves.css"
      />
      <link
        rel="stylesheet"
        href="src/assets/vendor/libs/perfect-scrollbar/perfect-scrollbar.css"
      />
      <link
        rel="stylesheet"
        href="src/assets/vendor/libs/typeahead-js/typeahead.css"
      />
      <link
        rel="stylesheet"
        href="src/assets/vendor/libs/select2/select2.css"
      />
      <link
        rel="stylesheet"
        href="src/assets/vendor/libs/@form-validation/form-validation.css"
      />
      <link
        rel="stylesheet"
        href="src/assets/vendor/libs/animate-css/animate.css"
      />
      <link
        rel="stylesheet"
        href="src/assets/vendor/libs/sweetalert2/sweetalert2.css"
      />
      <noscript>
        <iframe
          src="https://www.googletagmanager.com/ns.html?id=GTM-5DDHKGP"
          height="0"
          width="0"
          style={{ display: "none", visibility: "hidden" }}
          title="Google Tag Manager"
        ></iframe>
      </noscript>
      {/* End Google Tag Manager (noscript) */}
      {/* Layout wrapper */}
      <div className="layout-wrapper layout-content-navbar  ">
        <div className="layout-container">
          {/* / Menu */}
          {/* Layout container */}
          <div className="layout-page">
            {/* Navbar */}
            <nav
              className="layout-navbar container-xxl navbar navbar-expand-xl navbar-detached align-items-center bg-navbar-theme"
              id="layout-navbar"
            >
              <div className="layout-menu-toggle navbar-nav align-items-xl-center me-3 me-xl-0   d-xl-none ">
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
                    <li>
                      <div className="d-flex">
                        <div className="flex-shrink-0 me-3">
                          <div className="avatar avatar-online">
                            <img
                              src="src/assets/images/avatars/avt.png"
                              alt="User avatar"
                              className="h-auto rounded-circle"
                            />
                          </div>
                        </div>
                        <div className="flex-grow-1">
                          <span className="fw-medium d-block">{fullName}</span>
                          <small className="text-muted">{Role}</small>
                        </div>
                      </div>
                    </li>
                  </li>
                </ul>
              </div>
              {/* Search Small Screens */}
              <div className="navbar-search-wrapper search-input-wrapper  d-none">
                <input
                  type="text"
                  className="form-control search-input container-xxl border-0"
                  placeholder="Search..."
                  aria-label="Search..."
                />
                <i className="ti ti-x ti-sm search-toggler cursor-pointer" />
              </div>
            </nav>
            {/* / Navbar */}
            {/* Content wrapper */}
            <div className="content-wrapper">
              {/* Content */}
              <div className="container-xxl flex-grow-1 container-p-y">
                <h4 className="py-3 mb-4">
                  <span className="text-muted fw-light">
                    Account Settings /
                  </span>{" "}
                  Account
                </h4>
                <div className="row">
                  <div className="col-md-12">
                    <ul className="nav nav-pills flex-column flex-md-row mb-4">
                      <li className="nav-item">
                        <a
                          className="nav-link active"
                          href="javascript:void(0);"
                        >
                          <i className="ti-xs ti ti-users me-1" /> Account
                        </a>
                      </li>
                      <li className="nav-item">
                        <Link className="nav-link" to="/AccountSetting">
                          <i className="ti-xs ti ti-lock me-1" /> Security
                        </Link>
                      </li>
                    </ul>
                    <div className="card mb-4">
                      <hr className="my-0" />
                      <div className="card-body">
                        <Col span={12} style={{ maxWidth: "100%" }}>
                          <Title level={2}>Account Information</Title>
                          {/* {loadingData ? (
    <p>Loading...</p>
  ) : ( */}
                          <Form
                            form={form}
                            onFinish={handleUpdateProfile}
                            layout="vertical"
                          >
                            <Row gutter={16}>
                              <Col span={12}>
                                <Form.Item
                                  label="First Name"
                                  name="firstName"
                                  rules={[
                                    {
                                      required: true,
                                      message: "Please input your first name!",
                                    },
                                  ]}
                                >
                                  <Input />
                                </Form.Item>
                              </Col>
                              <Col span={12}>
                                <Form.Item
                                  label="Last Name"
                                  name="lastName"
                                  rules={[
                                    {
                                      required: true,
                                      message: "Please input your last name!",
                                    },
                                  ]}
                                >
                                  <Input />
                                </Form.Item>
                              </Col>
                              <Col span={12}>
                                <Form.Item
                                  label="Phone Number"
                                  name="phoneNumber"
                                  rules={[
                                    {
                                      required: true,
                                      message:
                                        "Please input your phone number!",
                                    },
                                  ]}
                                >
                                  <Input />
                                </Form.Item>
                              </Col>
                              <Col span={12}>
                                <Form.Item
                                  label="Gender"
                                  name="gender"
                                  rules={[
                                    {
                                      required: true,
                                      message: "Please select your gender!",
                                    },
                                  ]}
                                >
                                  <Select placeholder="Select">
                                    <Select.Option value="male">
                                      Male
                                    </Select.Option>
                                    <Select.Option value="female">
                                      Female
                                    </Select.Option>
                                    <Select.Option value="other">
                                      Other
                                    </Select.Option>
                                  </Select>
                                </Form.Item>
                              </Col>
                            </Row>
                            <Form.Item>
                              <Button
                                type="primary"
                                htmlType="submit"
                                loading={loading}
                              >
                                Save changes
                              </Button>
                              <Button
                                type="default"
                                htmlType="reset"
                                style={{ marginLeft: 8 }}
                              >
                                Cancel
                              </Button>
                            </Form.Item>
                          </Form>
                          {/* )} */}
                        </Col>
                      </div>
                      {/* /Account */}
                    </div>
                  </div>
                </div>
              </div>
              {/* / Content */}
              {/* Footer */}

              {/* / Footer */}
              <div className="content-backdrop fade" />
            </div>
            {/* Content wrapper */}
          </div>
          {/* / Layout page */}
        </div>
        {/* Overlay */}
        <div className="layout-overlay layout-menu-toggle" />
        {/* Drag Target Area To SlideIn Menu On Small Screens */}
        <div className="drag-target" />
      </div>
    </div>
  );
}
export default Account;

const cardStyles = {
  deleteAccountCard: {
    borderRadius: "8px",
    boxShadow: "0 4px 8px rgba(0, 0, 0, 0.1)",
    backgroundColor: "#ffffff",
  },
  cardHeader: {
    fontSize: "1.25rem",
    fontWeight: "bold",
    padding: "16px",
    borderBottom: "1px solid #f0f0f0",
  },
  cardBody: {
    padding: "16px",
  },
  alertContainer: {
    marginBottom: "16px",
  },
  confirmationCheckbox: {
    display: "block",
    marginBottom: "16px",
  },
  deactivateAccountBtn: {
    width: "100%",
  },
};
