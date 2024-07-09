import {
  faCartShopping,
  faCircleCheck,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  Checkbox,
  Col,
  DatePicker,
  Form,
  Modal,
  Row,
  Select,
  Space,
  message,
} from "antd";
import axios from "axios";
import dayjs from "dayjs";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import customParseFormat from "dayjs/plugin/customParseFormat";
import { useForm } from "antd/es/form/Form";

import { Option } from "antd/es/mentions";
dayjs.extend(customParseFormat);

function Cart() {
  const [currentStep, setCurrentStep] = useState(1);
  const [products, setProducts] = useState([]);
  const [error, setError] = useState();
  const navigate = useNavigate();

  const [rank, setRank] = useState(null);
  const [discount, setDiscount] = useState(0);

  const [form] = useForm();
  const [isOpen, setIsOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [newDate, setNewDate] = useState(null);

  const [staffList, setStaffList] = useState([]);

  const [selectedStaffId, setSelectedStaffId] = useState([]);

  const fetchStaff = async () => {
    try {
      const response = await axios.get("https://localhost:7150/api/Staff");
      setStaffList(response.data.data);
    } catch (error) {
      message.error("Failed to fetch staff members");
    }
  };

  const handleDeleteBooking = async (index) => {
    const cartString = localStorage.getItem("cart");
    let cart = JSON.parse(cartString);

    if (!cart || !Array.isArray(cart)) {
      message.error("Cart is empty or invalid.");
      return;
    }

    cart.splice(index, 1);
    setProducts(cart);
    localStorage.setItem("cart", JSON.stringify(cart));
    message.success("Booking removed successfully.");
  };

  const handleCheckboxChange = (productId, petId, date) => {
    setProducts((prevStoredProducts) => {
      return prevStoredProducts.map((product) => {
        if (
          product.serviceId === productId &&
          product.petId === petId &&
          product.date === date
        ) {
          return { ...product, selected: !product.selected };
        }
        return product;
      });
    });
  };

  const formatPrice = (price) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(price);
  };

  const calculateSubtotal = () => {
    const subtotal = products
      .filter((product) => product.selected)
      .reduce((total, product) => total + product.servicePrice, 0);
    const discountedSubtotal = subtotal * (1 - discount);
    return discountedSubtotal;
  };

  const fetchCustomerRankAndDiscount = async () => {
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);

    if (userInfo != null) {
      const response = await axios.get(
        `https://localhost:7150/api/Customer/${userInfo.data.user.id}`,
        {
          headers: {
            Authorization: `Bearer ${userInfo.data.token}`,
          },
        }
      );

      const rank = response.data.data.cusRank;
      setRank(rank);

      if (rank === "Silver") {
        setDiscount(0.05);
      } else if (rank === "Gold") {
        setDiscount(0.1);
      }
    }
  };

  const handleHideModal = () => {
    setIsOpen(false);
    setSelectedProduct(null);
    setNewDate(null);
  };

  const handleOk = () => {
    form.submit();
  };

  const handleUpdateTime = async () => {
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);
    const token = userInfo?.data?.token;
    setError("");

    try {
      let url = `https://localhost:7150/api/Booking/available?startTime=${newDate.format(
        "YYYY-MM-DDTHH:mm:ss"
      )}&serviceCode=${selectedProduct.serviceId}`;

      if (selectedStaffId) {
        url += `&staffId=${selectedStaffId}`;
      }

      const response = await axios.get(url, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.status === 401) {
        console.log("Token expired. Please log in again.");
        setError("Token expired. Please log in again.");
        localStorage.removeItem("user-info");

        return;
      }

      if (response.status === 200) {
        const bookingId = selectedProduct.bookingId;

        try {
          const updateResponse = await axios.put(
            `https://localhost:7150/api/Booking/${bookingId}`,
            { newDateTime: newDate.format("YYYY-MM-DDTHH:mm:ss") },
            {
              headers: {
                Authorization: `Bearer ${token}`,
              },
            }
          );

          if (updateResponse.status === 200) {
            console.log("Booking time updated successfully.");
            message.success("Booking time updated successfully.");
            setError("");
          } else {
            throw new Error("Failed to update booking.");
          }
        } catch (updateError) {
          console.error("Error updating booking time:", updateError);
          message.error(
            updateError.response?.data || "Failed to update booking time."
          );
          setError(
            updateError.response?.data || "Failed to update booking time."
          );
        }
      }
    } catch (error) {
      if (error.response) {
        if (error.response.status === 401) {
          localStorage.removeItem("user-info");
          console.log("Token expired. Please log in again.");
          message.error("Token expired. Please log in again.");
          navigate("/login");
        } else {
          console.error("Error response:", error.response.data);
          message.error(error.response.data || "An error occurred.");
          setError(error.response.data || "An error occurred.");
        }
      } else {
        console.error("Error:", error);
        message.error("An unexpected error occurred.");
        setError("An unexpected error occurred.");
      }
    }
  };

  async function fetchComboType(comboId) {
    try {
      const response = await axios.get(
        `https://localhost:7150/api/Combo/${comboId}`
      );
      return response.data.data.comboType;
    } catch (error) {
      console.error(`Error fetching combo type for comboId ${comboId}:`, error);
      return null;
    }
  }

  async function fetchStaffName(staffId) {
    try {
      const response = await axios.get(
        `https://localhost:7150/api/Staff/${staffId}`
      );
      const staff = response.data;

      if (staff && staff.data.fullName) {
        return staff.data.fullName;
      } else {
        return null;
      }
    } catch (error) {
      console.error(`Error fetching staff name for staffId ${staffId}:`, error);
      return null;
    }
  }

  async function fetchPetName(petId) {
    try {
      const petsString = localStorage.getItem("pets");
      if (!petsString) {
        console.error("No pets data found in localStorage");
        return null;
      }

      const pets = JSON.parse(petsString);
      const pet = pets.data.pets.find((pet) => pet.petId === petId);

      return pet ? pet.petName : null;
    } catch (error) {
      console.error(`Error fetching pet name for petId ${petId}:`, error);
      return null;
    }
  }

  async function fetchBookings() {
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);

    if (userInfo != null) {
      try {
        const response = await axios.get(
          `https://localhost:7150/api/Customer/${userInfo.data.user.id}/bookings`,
          {
            headers: {
              Authorization: `Bearer ${userInfo.data.token}`,
            },
          }
        );

        if (response.status === 401) {
          message.error("Token has expired. Please log in again.");
          localStorage.removeItem("user-info");
          navigate("/login");
          return;
        }

        const result = response.data;
        const bookings = result.data.bookings;

        const extractedDataPromises = bookings.map(async (booking) => {
          const bookingDetailsPromises = booking.bookingDetails.map(
            async (detail) => {
              const petName = await fetchPetName(detail.petId);
              const staffName = detail.staffId
                ? await fetchStaffName(detail.staffId)
                : null;

              if (
                booking.bookingDetails.length >= 2 &&
                detail.service.comboId
              ) {
                const comboType = await fetchComboType(detail.service.comboId);
                return {
                  bookingId: detail.bookingId,
                  petId: detail.petId,
                  petName: petName,
                  scheduleDate: booking.bookingSchedule,
                  comboId: detail.service.comboId,
                  comboType: comboType,
                  serviceName: null,
                  serviceId: null,
                  staffId: detail.staffId,
                  staffName: staffName,
                  servicePrice: detail.service.price,
                  checkAccept: booking.checkAccept,
                };
              } else {
                return {
                  bookingId: detail.bookingId,
                  petId: detail.petId,
                  petName: petName,
                  scheduleDate: booking.bookingSchedule,
                  comboId: null,
                  comboType: null,
                  serviceName: detail.service.serviceName,
                  serviceId: detail.service.serviceId,
                  staffId: detail.staffId,
                  staffName: staffName,
                  servicePrice: detail.service.price,
                  checkAccept: booking.checkAccept,
                };
              }
            }
          );

          return Promise.all(bookingDetailsPromises);
        });

        (await Promise.all(extractedDataPromises)).flat();
      } catch (error) {
        if (error.response && error.response.status === 401) {
          localStorage.removeItem("user-info");
          message.error("Token has expired. Please log in again.");
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
  }

  async function handleBooking() {
    let userInfo;
    try {
      const userInfoString = localStorage.getItem("user-info");
      if (!userInfoString) {
        throw new Error("User info not found in localStorage.");
      }
      userInfo = JSON.parse(userInfoString);
    } catch (error) {
      console.error("Error parsing user info:", error);

      message.error("Invalid user information. Please log in again.");
      return;
    }

    const token = userInfo.data?.token;
    const userId = userInfo.data?.user?.id;

    if (!token || !userId) {
      message.error("User information is incomplete.");
      return;
    }

    if (products.every((item) => !item.selected)) {
      message.error("Cart list is empty");
      return;
    }

    const cart = products.filter((item) => item.selected);

    if (cart.length === 0) {
      setError("Your cart is empty.");
      return;
    }

    const bookingPromises = [];

    for (let item of cart) {
      if (item.selected) {
        if (item.comboDetails && item.period === 1) {
          bookingPromises.push({
            cusId: userId,
            bookingSchedule: item.date,
            bookingDetails: item.comboDetails.map((detail) => ({
              petId: item.petId,
              serviceId: detail.serviceId,
              comboId: detail.comboId,
              staffId: detail.staffId || null,
              status: true,
              comboType: "string",
            })),
          });
        } else if (item.comboDetails && item.period > 1) {
          for (let i = 0; i < item.period; i++) {
            const bookingDate = dayjs(item.date)
              .add(i, "months")
              .format("YYYY-MM-DDTHH:mm:ss");

            bookingPromises.push({
              cusId: userId,
              bookingSchedule: bookingDate,
              bookingDetails: item.comboDetails.map((detail) => ({
                petId: item.petId,
                serviceId: detail.serviceId,
                comboId: detail.comboId,
                staffId: detail.staffId || null,
                status: true,
                comboType: "string",
              })),
            });
          }
        } else if (item.period > 1) {
          for (let i = 0; i < item.period; i++) {
            const bookingDate = dayjs(item.date)
              .add(i, "months")
              .format("YYYY-MM-DDTHH:mm:ss");

            bookingPromises.push({
              cusId: userId,
              bookingSchedule: bookingDate,
              bookingDetails: [
                {
                  petId: item.petId,
                  serviceId: item.serviceId,
                  comboId: null,
                  staffId: item.staffId || null,
                  status: true,
                  comboType: "string",
                },
              ],
            });
          }
        } else {
          bookingPromises.push({
            cusId: userId,
            bookingSchedule: item.date,
            bookingDetails: [
              {
                petId: item.petId,
                serviceId: item.serviceId,
                comboId: null,
                staffId: item.staffId || null,
                status: true,
                comboType: "string",
              },
            ],
          });
        }
      }
    }

    console.log(bookingPromises);
    try {
      const responses = await Promise.all(
        bookingPromises.map((requestData) =>
          axios.post(`https://localhost:7150/api/Booking`, requestData, {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          })
        )
      );

      const bookingCodes = responses
        .filter((response) => response)
        .map((response) => response.data.data.bookingId);

      if (bookingCodes.length > 0) {
        const paymentRequest = {
          cusId: userInfo?.data?.user?.id,
          bookingIds: bookingCodes,
          orderType: "string",
          amount: calculateSubtotal(),
          orderDescription: "string",
          name: "string",
          returnUrl: "http://localhost:5173/Cart",
        };

        try {
          const paymentResponse = await axios.post(
            `https://localhost:7150/api/Payments/create-payment`,
            paymentRequest,
            {
              headers: {
                Authorization: `Bearer ${token}`,
              },
            }
          );

          if (paymentResponse.status === 200 && paymentResponse.data) {
            const existingBookings =
              JSON.parse(localStorage.getItem("checkBookings")) || [];
            const newBookings = bookingCodes.map((code) => ({
              code,
              expiry: new Date().getTime() + 24 * 60 * 60 * 1000,
            }));
            const updatedBookings = [...existingBookings, ...newBookings];
            localStorage.setItem(
              "checkBookings",
              JSON.stringify(updatedBookings)
            );
            localStorage.setItem("selectedProducts", JSON.stringify(cart));

            window.location.href = paymentResponse.data.paymentUrl;
          } else {
            message.error("Failed to create payment link.");
          }
        } catch (error) {
          console.error("Payment error:", error);
          message.error("An error occurred while creating the payment link.");
        }
      } else {
        message.error("Booking failed.");
      }
    } catch (error) {
      if (error.response) {
        if (error.response.status === 401) {
          console.log("Token expired. Please log in again.");
          message.error("Token expired. Please log in again.");
          navigate("/login");
        } else {
          console.error("Error response:", error.response.data);
          message.error(error.response.data || "An error occurred.");
          setError(error.response.data || "An error occurred.");
        }
      } else {
        console.error("Error:", error);
        message.error("An unexpected error occurred.");
        setError("An unexpected error occurred.");
      }
    }
  }

  // Clear cart at midnight
  const clearCartAtMidnight = () => {
    const now = new Date();
    const midnight = new Date(
      now.getFullYear(),
      now.getMonth(),
      now.getDate() + 1,
      0,
      0,
      0
    );
    const timeToMidnight = midnight.getTime() - now.getTime();
    setTimeout(() => {
      localStorage.removeItem("cart");
      setProducts([]);
    }, timeToMidnight);
  };

  useEffect(() => {
    const fetchStaffAndBookings = async () => {
      await fetchStaff();
      await fetchBookings();
    };

    fetchStaffAndBookings();
    fetchCustomerRankAndDiscount();

    const urlParams = new URLSearchParams(window.location.search);
    const responseCode = urlParams.get("vnp_ResponseCode");
    const vnpTxnRef = urlParams.get("vnp_TxnRef");

    const fetchAllStaffNames = async (products) => {
      const productsWithStaffNames = await Promise.all(
        products.map(async (product) => {
          const staffName = product.staffId
            ? await fetchStaffName(product.staffId)
            : null;
          return {
            ...product,
            ...(staffName && { staffName }), // Only include staffName if it exists
            selected: true,
          };
        })
      );
      return productsWithStaffNames;
    };

    const handleProductsUpdate = async (products) => {
      const productsWithStaffNames = await fetchAllStaffNames(products);
      setProducts(productsWithStaffNames);
      console.log(productsWithStaffNames);
    };

    if (responseCode != null) {
      if (responseCode === "00") {
        const selectedProducts =
          JSON.parse(localStorage.getItem("selectedProducts")) || [];
        localStorage.removeItem("selectedProducts");
        const cartData = JSON.parse(localStorage.getItem("cart")) || [];
        const updatedProducts = cartData.filter(
          (product) =>
            !selectedProducts.some(
              (selected) =>
                selected.serviceName === product.serviceName &&
                selected.petId === product.petId &&
                selected.date === product.date &&
                selected.staffId === product.staffId
            )
        );
        handleProductsUpdate(updatedProducts);
        localStorage.setItem("cart", JSON.stringify(updatedProducts));
        navigate("/Cart");
        setCurrentStep(2);
        message.success("Payment successful and items removed from cart.");
      } else if (responseCode !== "00" && vnpTxnRef) {
        const products = JSON.parse(localStorage.getItem("cart")) || [];
        localStorage.removeItem("selectedProducts");

        handleProductsUpdate(products);
        axios
          .delete(
            `https://localhost:7150/api/Payments?transactionId=${vnpTxnRef}`
          )
          .then((response) => {
            console.log("Payment failure data:", response.data);
            message.error("Payment failed. Please try again.");
            navigate("/Cart");
          })
          .catch((error) => {
            console.error("Error fetching payment failure data:", error);
            message.error(
              "Payment failed and we encountered an error while fetching the failure details."
            );
            navigate("/Cart");
          });
      }
    } else {
      const storedProducts = JSON.parse(localStorage.getItem("cart")) || [];
      if (storedProducts.length > 0) {
        handleProductsUpdate(storedProducts);
      }
    }

    // Set up the cart clear at midnight
    clearCartAtMidnight();
  }, []);

  return (
    <div>
      <Modal
        title={
          <div
            style={{
              textAlign: "center",
              fontSize: "24px",
              fontWeight: "bold",
            }}
          >
            Change time for booking
          </div>
        }
        open={isOpen}
        onCancel={handleHideModal}
        onOk={handleOk}
      >
        <Form
          labelCol={{
            span: 24,
          }}
          form={form}
          onFinish={handleUpdateTime}
        >
          <Row gutter={16}>
            <Col span={12}>
              <Form.Item label="New Date">
                <Space direction="vertical">
                  <DatePicker
                    showTime
                    format="YYYY-MM-DD HH:mm:ss"
                    value={newDate}
                    onChange={(date) => setNewDate(date)}
                    className="w-full"
                  />
                </Space>
              </Form.Item>
            </Col>
            <Col span={12}>
              <Form.Item label="Select Staff" name="staff">
                <Select
                  showSearch
                  placeholder="Select a staff"
                  optionFilterProp="children"
                  onChange={(value) => setSelectedStaffId(value)}
                  className="w-full"
                  value={selectedStaffId}
                >
                  {Array.isArray(staffList) &&
                    staffList.map((staff) => (
                      <Option key={staff.staffId} value={staff.staffId}>
                        {staff.fullName}
                      </Option>
                    ))}
                </Select>
              </Form.Item>
            </Col>
          </Row>
          {error && <p className="error-message">{error}</p>}
        </Form>
      </Modal>

      <head>
        <meta charSet="utf-8" />
        <meta
          name="viewport"
          content="width=device-width, initial-scale=1.0, user-scalable=no, minimum-scale=1.0, maximum-scale=1.0"
        />
        <title>
          Checkout Card - Front Pages | Vuexy - Bootstrap Admin Template
        </title>
        <meta
          name="description"
          content="Start your development with a Dashboard for Bootstrap 5"
        />
        <meta
          name="keywords"
          content="dashboard, bootstrap 5 dashboard, bootstrap 5 design, bootstrap 5"
        />
        <link rel="canonical" href="https://1.envato.market/vuexy_admin" />
        <link
          rel="icon"
          type="image/x-icon"
          href="src/assets/images/favicon/favicon.ico"
        />
        <link rel="preconnect" href="https://fonts.googleapis.com" />
        <link rel="preconnect" href="https://fonts.gstatic.com" crossOrigin />
        <link
          href="https://fonts.googleapis.com/css2?family=Public+Sans:ital,wght@0,300;0,400;0,500;0,600;0,700;1,300;1,400;1,500;1,600;1,700&ampdisplay=swap"
          rel="stylesheet"
        />
        <link
          rel="stylesheet"
          href="src/assets/vendor/fonts/tabler-icons.css"
        />
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
        <link
          rel="stylesheet"
          href="src/assets/vendor/css/pages/front-page.css"
        />
        <link
          rel="stylesheet"
          href="src/assets/vendor/libs/node-waves/node-waves.css"
        />
        <link
          rel="stylesheet"
          href="src/assets/vendor/libs/select2/select2.css"
        />
        <link
          rel="stylesheet"
          href="src/assets/vendor/libs/bs-stepper/bs-stepper.css"
        />
        <link
          rel="stylesheet"
          href="src/assets/vendor/libs/rateyo/rateyo.css"
        />
        <link
          rel="stylesheet"
          href="src/assets/vendor/libs/@form-validation/form-validation.css"
        />
        <link
          rel="stylesheet"
          href="src/assets/vendor/css/pages/wizard-ex-checkout.css"
        />

        <script src="src/assets/vendor/js/helpers.js"></script>
        <script src="src/assets/vendor/js/template-customizer.js"></script>
        <script src="src/assets/js/front-config.js"></script>
      </head>

      <body>
        <noscript>
          <iframe
            src="https://www.googletagmanager.com/ns.html?id=GTM-5DDHKGP"
            height="0"
            width="0"
            style="display: none; visibility: hidden"
          ></iframe>
        </noscript>
        <script src="src/assets/vendor/js/dropdown-hover.js"></script>
        <script src="src/assets/vendor/js/mega-dropdown.js"></script>

        <nav className="layout-navbar shadow-none py-0">
          <div className="container"></div>
        </nav>

        <section className="section-py bg-body first-section-pt">
          <div className="container">
            <div
              id="wizard-checkout"
              className="bs-stepper wizard-icons wizard-icons-example mb-5"
            >
              <div className="bs-stepper-header m-auto border-0 py-4">
                <div className="step" data-target="#checkout-cart">
                  <button
                    type="button"
                    className="step-trigger"
                    onClick={() => setCurrentStep(1)}
                  >
                    <span className="bs-stepper-icon">
                      <svg viewBox="0 0 58 54">
                        <FontAwesomeIcon icon={faCartShopping} />{" "}
                      </svg>
                    </span>
                    <span className="bs-stepper-label">Cart</span>
                  </button>
                </div>
                <div className="line">
                  <i className="ti ti-chevron-right"></i>
                </div>

                <div className="step" data-target="#checkout-confirmation">
                  <button
                    type="button"
                    className="step-trigger"
                    // onClick={() => setCurrentStep(2)}
                  >
                    <span className="bs-stepper-icon">
                      <svg viewBox="0 0 58 54">
                        <FontAwesomeIcon icon={faCircleCheck} />{" "}
                      </svg>
                    </span>
                    <span className="bs-stepper-label">Confirmation</span>
                  </button>
                </div>
              </div>
              <div className="bs-stepper-content border-top">
                <form
                  id="wizard-checkout-form"
                  onSubmit={(e) => e.preventDefault()}
                >
                  {currentStep === 1 && (
                    <div id="checkout-cart" className="content">
                      <div className="row">
                        <div className="col-xl-8 mb-3 mb-xl-0">
                          <div
                            className="alert alert-success mb-3"
                            role="alert"
                          >
                            <div className="d-flex gap-3">
                              <div className="flex-shrink-0">
                                <i className="ti ti-bookmarks ti-sm alert-icon alert-icon-lg"></i>
                              </div>
                              <div className="flex-grow-1">
                                <div className="fw-medium fs-5 mb-2">
                                  Available Offers
                                </div>
                                <ul className="list-unstyled mb-0">
                                  <li> - 5% Silver Rank</li>
                                  <li> - 10% Gold Rank</li>
                                </ul>
                              </div>
                            </div>
                            <button
                              type="button"
                              className="btn-close btn-pinned"
                              data-bs-dismiss="alert"
                              aria-label="Close"
                            ></button>
                          </div>

                          <h5>My Shopping Bag </h5>
                          <ul className="list-group mb-3">
                            {products.map((product, index) => (
                              <li key={index} className="list-group-item p-4">
                                <div className="d-flex gap-3">
                                  <div className="flex-shrink-0 d-flex align-items-center">
                                    <img
                                      src="src/assets/images/icon/icon_pet_walking.svg"
                                      alt="google home"
                                      className="w-px-100"
                                    />
                                  </div>
                                  <div className="flex-grow-1">
                                    <div className="row">
                                      <div className="col-md-8">
                                        <p className="me-3">
                                          <a
                                            href="javascript:void(0)"
                                            className="text-body"
                                          >
                                            {product.comboDetails
                                              ? `Combo: ${product.serviceName}`
                                              : `Service: ${product.serviceName}`}
                                          </a>
                                        </p>
                                        <div className="text-muted mb-2 d-flex flex-wrap">
                                          <span className="me-1">PetName:</span>
                                          <a
                                            href="javascript:void(0)"
                                            className="me-3"
                                          >
                                            {product.petName}
                                          </a>
                                        </div>
                                        {product.staffName && (
                                          <div className="text-muted mb-2 d-flex flex-wrap">
                                            <span className="me-1">
                                              StaffName:
                                            </span>
                                            <a
                                              href="javascript:void(0)"
                                              className="me-3"
                                            >
                                              {product.staffName}
                                            </a>
                                          </div>
                                        )}
                                        <div className="text-muted mb-2 d-flex flex-wrap">
                                          <span className="me-1">Date:</span>
                                          <a
                                            href="javascript:void(0)"
                                            className="me-3"
                                          >
                                            {new Date(
                                              product.date
                                            ).toLocaleDateString("vi-VN", {
                                              year: "numeric",
                                              month: "long",
                                              day: "numeric",
                                            })}
                                          </a>
                                        </div>
                                        <div className="text-muted mb-2 d-flex flex-wrap">
                                          <span className="me-1">Time:</span>
                                          <a
                                            href="javascript:void(0)"
                                            className="me-3"
                                          >
                                            {new Date(
                                              product.date
                                            ).toLocaleTimeString("vi-VN", {
                                              hour: "2-digit",
                                              minute: "2-digit",
                                            })}
                                          </a>
                                        </div>
                                        <div className="text-muted mb-2 d-flex flex-wrap">
                                          <span className="me-1">Period:</span>
                                          <a
                                            href="javascript:void(0)"
                                            className="me-3"
                                          >
                                            {product.period == "1"
                                              ? product.period + " time"
                                              : product.period + " months"}
                                          </a>
                                        </div>
                                      </div>
                                      <div className="col-md-4">
                                        <div className="text-md-end">
                                          <button
                                            type="button"
                                            className="btn-close btn-pinned"
                                            onClick={() =>
                                              handleDeleteBooking(index)
                                            }
                                            aria-label="Close"
                                          ></button>

                                          <div className="my-2 my-md-4 mb-md-5">
                                            <span className="text-primary">
                                              {formatPrice(
                                                product.servicePrice
                                              )}
                                            </span>
                                          </div>

                                          <Checkbox
                                            checked={product.selected}
                                            onChange={() =>
                                              handleCheckboxChange(
                                                product.serviceId,
                                                product.petId,
                                                product.date
                                              )
                                            }
                                          ></Checkbox>
                                        </div>
                                      </div>
                                    </div>
                                  </div>
                                </div>
                              </li>
                            ))}
                          </ul>

                          <div className="list-group">
                            <a
                              href="javascript:void(0)"
                              className="list-group-item d-flex justify-content-between"
                            >
                              <span>Add more products from wishlist</span>
                              <i className="ti ti-sm ti-chevron-right scaleX-n1-rtl"></i>
                            </a>
                          </div>
                        </div>

                        <div className="col-xl-4">
                          <div className="border rounded p-4 mb-3 pb-3">
                            <div>Price Details: </div>

                            <hr className="my-4" />

                            <div className="d-flex justify-content-between mb-2">
                              <span>Subtotal</span>
                              <span className="text-end">
                                {formatPrice(
                                  products
                                    .filter((product) => product.selected)
                                    .reduce(
                                      (total, product) =>
                                        total + product.servicePrice,
                                      0
                                    )
                                )}
                              </span>
                            </div>

                            <div className="d-flex justify-content-between mb-2">
                              <span>
                                Discount{" "}
                                {rank === "silver"
                                  ? "(5%)"
                                  : rank === "gold"
                                  ? "(10%)"
                                  : ""}
                              </span>
                              <span className="text-end">
                                {formatPrice(
                                  products
                                    .filter((product) => product.selected)
                                    .reduce(
                                      (total, product) =>
                                        total + product.servicePrice,
                                      0
                                    ) * discount
                                )}
                              </span>
                            </div>
                            <div className="d-flex justify-content-between mb-4">
                              <span>Rank</span>
                              <span className="text-end">{rank}</span>
                            </div>
                            <hr />
                            <div className="d-flex justify-content-between mb-4">
                              <span>Total</span>
                              <span className="fw-bold text-end">
                                {formatPrice(calculateSubtotal())}
                              </span>
                            </div>

                            <div className="d-grid">
                              <button
                                type="button"
                                className="btn btn-primary btn-next"
                                onClick={handleBooking}
                              >
                                Submit
                              </button>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                  )}
                  {currentStep === 2 && (
                    <div className="bs-stepper-content border-top">
                      <form
                        id="wizard-checkout-form"
                        onSubmit={(e) => e.preventDefault()}
                      >
                        {currentStep === 2 && (
                          <div
                            style={{
                              padding: "20px",
                              fontFamily: "Arial, sans-serif",
                            }}
                          >
                            <h1 style={{ textAlign: "center" }}>
                              Thank You! 😇
                            </h1>
                            <h2 style={{ textAlign: "center" }}>
                              Your appointment has been placed!
                            </h2>
                          </div>
                        )}
                      </form>
                    </div>
                  )}
                </form>
              </div>
            </div>
          </div>
        </section>
      </body>
    </div>
  );
}

export default Cart;
