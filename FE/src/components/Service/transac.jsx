import { useState, useEffect } from "react";
import "../../assets/css/trans.css";
import {
  Form,
  DatePicker,
  Space,
  Button,
  Select,
  message,
  Col,
  Row,
  Modal,
  Input,
  Pagination,
} from "antd";
import axios from "axios";
import dayjs from "dayjs";
import { useNavigate } from "react-router-dom";
import moment from "moment";

const { Option } = Select;

const Transac = () => {
  const [selectedProducts, setSelectedProducts] = useState([]);
  const [selectedStaffId, setSelectedStaffId] = useState(null);
  const [form] = Form.useForm();
  const [error, setError] = useState("");
  const navigate = useNavigate();
  const [isOpen, setIsOpen] = useState(false);
  const [isFeedbackModalOpen, setIsFeedbackModalOpen] = useState(false);
  const [isRefundModalOpen, setIsRefundModalOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [newDate, setNewDate] = useState(null);
  const [feedbackText, setFeedbackText] = useState("");
  const [bankName, setBankName] = useState("");
  const [cardNumber, setCardNumber] = useState("");
  const [dataSource, setDataSource] = useState([]);
  const [staffList, setStaffList] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);

  useEffect(() => {
    fetchStaff();
    fetchBookings();
  }, []);

  const handleOk = () => {
    form.submit();
  };

  const fetchStaff = async () => {
    try {
      const response = await axios.get("https://localhost:7150/api/Staff");
      setStaffList(response.data.data);
    } catch (error) {
      message.error("Failed to fetch staff members");
    }
  };

  const handleHideModal = () => {
    setIsOpen(false);
    setIsFeedbackModalOpen(false);
    setIsRefundModalOpen(false);
    setSelectedProduct(null);
    setNewDate(null);
    setBankName("");
    setCardNumber("");
    form.resetFields();
  };

  const fetchBookings = async () => {
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);

    if (userInfo) {
      try {
        const response = await axios.get(
          `https://localhost:7150/api/Payments/history-customerId?CustomerId=${userInfo.data.user.id}`,
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
        if (!Array.isArray(result)) {
          throw new Error("Invalid API response format");
        }

        const extractedData = await Promise.all(
          result.map(async (transaction) => {
            const bookingDetails = await Promise.all(
              transaction.bookingDetails.map(async (detail) => {
                const petName = await fetchPetName(detail.petId);
                const staffName = detail.staffId
                  ? await fetchStaffName(detail.staffId)
                  : null;
                const serviceName = await fetchServiceName(detail.serviceId);
                let comboName = null;
                if (detail.comboId) {
                  comboName = await fetchComboName(detail.comboId);
                }

                return {
                  ...detail,
                  petName,
                  staffName,
                  serviceName,
                  comboName,
                };
              })
            );

            return {
              ...transaction,
              bookingDetails,
            };
          })
        );

        setDataSource(extractedData);
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
  };

  const fetchPetName = async (petId) => {
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
  };
  const fetchComboName = async (comboId) => {
    try {
      const response = await axios.get(
        `https://localhost:7150/api/Combo/${comboId}`
      );

      return response.data.data.comboType;
    } catch (error) {
      console.error(`Error fetching combo name for comboId ${comboId}:`, error);
      return null;
    }
  };

  const fetchStaffName = async (staffId) => {
    try {
      const response = await axios.get(
        `https://localhost:7150/api/Staff/${staffId}`
      );
      return response.data.data.fullName;
    } catch (error) {
      console.error(`Error fetching staff name for staffId ${staffId}:`, error);
      return null;
    }
  };

  const fetchServiceName = async (serviceId) => {
    try {
      const response = await axios.get(
        `https://localhost:7150/api/Service/${serviceId}`
      );
      return response.data.data.serviceName;
    } catch (error) {
      console.error(
        `Error fetching service name for serviceId ${serviceId}:`,
        error
      );
      return null;
    }
  };

  const handleSelectProduct = (event) => {
    const productId = event.target.value;
    if (event.target.checked) {
      setSelectedProducts([...selectedProducts, productId]);
    } else {
      setSelectedProducts(selectedProducts.filter((id) => id !== productId));
    }
  };

  const handlePrint = () => {
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

    selectedProducts.forEach((id) => {
      const printContents = document.getElementById(id).innerHTML;
      newWindow.document.write(printContents);
      newWindow.document.write("<hr>"); // Add a separator between products
    });

    newWindow.document.write("</body></html>");

    newWindow.document.close();
    newWindow.focus(); // Necessary for IE >= 10
    newWindow.print();
  };

  const handleShowModal = (bookingId) => {
    const selectedTransaction = dataSource.find((transaction) =>
      transaction.bookingDetails.some(
        (bookingDetail) => bookingDetail.bookingId === bookingId
      )
    );
    setNewDate(moment(selectedTransaction.scheduleDate, "YYYY-MM-DD HH:mm:ss"));
    const selectedProduct = selectedTransaction.bookingDetails.find(
      (bookingDetail) => bookingDetail.bookingId === bookingId
    );

    setSelectedProduct(selectedProduct);

    // Check if the booking is in progress or completed
    if (selectedProduct.status === 0 || selectedProduct.status === 1) {
      message.error(
        "Cannot update booking. Booking is either in progress or completed."
      );
      return;
    }

    // Check if the booking is less than 24 hours from now
    const now = moment();
    const originalBookingTime = moment(
      selectedProduct.scheduleDate,
      "YYYY-MM-DD HH:mm:ss"
    );
    if (originalBookingTime.diff(now, "hours") < 24) {
      message.error(
        "Booking time is less than 24 hours from now, therefore it cannot be changed."
      );
      return;
    }

    setSelectedStaffId(selectedProduct.staffId || null);

    // Reset form and set new values
    form.resetFields();
    setNewDate(originalBookingTime); // Set the newDate with the original booking time
    form.setFieldsValue({
      staff: selectedProduct.staffId || null,
      date: originalBookingTime,
    });

    setIsOpen(true);
  };

  const handleUpdateTime = async () => {
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);
    const token = userInfo?.data?.token;
    setError("");
    setIsLoading(true);

    // Validate that new date is not empty
    if (!newDate || !newDate.isValid()) {
      message.error("New schedule time cannot be empty.");
      setError(null);
      setIsLoading(false);
      return;
    }

    // Validate that new date is at least 24 hours from now
    const now = moment();
    if (newDate.diff(now, "hours") < 24) {
      message.error("New schedule time must be at least 24 hours from now.");
      setError(null);
      setIsLoading(false);
      return;
    }

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
        setIsLoading(false);
        return;
      }

      if (response.status === 200) {
        const bookingId = selectedProduct.bookingId;

        try {
          const updateResponse = await axios.post(
            `https://localhost:7150/api/Booking/update-time-booking`,
            {
              bookingId,
              newBookingSchedule: newDate.format("YYYY-MM-DDTHH:mm:ss"),
              newStaffId: selectedStaffId || null,
            },
            {
              headers: {
                Authorization: `Bearer ${token}`,
              },
            }
          );

          if (updateResponse.status === 200) {
            console.log("Booking time updated successfully.");
            message.success("Booking time updated successfully.");
            const staffName = selectedStaffId
              ? await fetchStaffName(selectedStaffId)
              : null;
            // Update the booking time in dataSource
            const updatedDataSource = dataSource.map((transaction) => {
              return {
                ...transaction,
                bookingDetails: transaction.bookingDetails.map((item) =>
                  item.bookingId === selectedProduct.bookingId
                    ? {
                        ...item,
                        scheduleDate: newDate.format("YYYY-MM-DD HH:mm:ss"),
                        staffName: staffName,
                      }
                    : item
                ),
              };
            });

            setDataSource(updatedDataSource);

            handleHideModal();
            setError("");
          } else {
            throw new Error("Failed to update booking.");
          }
        } catch (updateError) {
          console.error("Error updating booking time:", updateError);
          message.error(
            updateError.response?.data?.errorMessage ||
              "Failed to update booking time."
          );
          setError(
            updateError.response?.data?.errorMessage ||
              "Failed to update booking time."
          );
        }
      }
    } catch (error) {
      if (error.response) {
        if (error.response.status === 401) {
          localStorage.removeItem("user-info");
          console.log("Token expired. Please log in again.");
          message.error("Token expired. Please log in again.");
          setError("Token expired. Please log in again.");
          navigate("/login");
        } else {
          console.error("Error response:", error.response.data);
          message.error(
            error.response.data || "An error occurred."
          );
          //setError(error.response.data|| "An error occurred.");
        }
      } else {
        console.error("Error:", error);
        message.error("An unexpected error occurred.");
       // setError("An unexpected error occurred.");
      }
    }
    setIsLoading(false);
  };

  const handleFeedbackSubmit = async () => {
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);
    const token = userInfo?.data?.token;

    try {
      const response = await axios.post(
        `https://localhost:7150/api/Booking/update-feedback`,
        {
          bookingId: selectedProduct.bookingId,
          feedback: feedbackText,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (response.status === 200) {
        message.success("Feedback submitted successfully.");

        // Update the feedback in dataSource
        const updatedDataSource = dataSource.map((transaction) => {
          return {
            ...transaction,
            bookingDetails: transaction.bookingDetails.map((item) =>
              item.bookingId === selectedProduct.bookingId
                ? { ...item, feedback: feedbackText }
                : item
            ),
          };
        });
        setDataSource(updatedDataSource);

        setIsFeedbackModalOpen(false);
        setFeedbackText("");
      } else {
        throw new Error("Failed to submit feedback.");
      }
    } catch (error) {
      console.error("Error submitting feedback:", error);
      message.error(
        error.response?.data || "Failed to submit feedback."
      );
    }
  };

  const handleRefund = async () => {
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);
    const token = userInfo?.data?.token;

    if (!bankName || !cardNumber) {
      message.error("Bank name and card number are required.");
      return;
    }

    try {
      const response = await axios.post(
        `https://localhost:7150/api/Booking/cancel-booking`,
        {
          bookingId: selectedProduct.bookingId,
          bankName,
          cardNumber,
        },
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (response.status === 200) {
        message.success(
          "Refund request submitted successfully. 70% of the amount will be refunded within 24 hours."
        );

        // Update the booking status in dataSource
        const updatedDataSource = dataSource.map((transaction) => {
          return {
            ...transaction,
            bookingDetails: transaction.bookingDetails.map((item) =>
              item.bookingId === selectedProduct.bookingId
                ? { ...item, status: 2 }
                : item
            ),
          }});
        setDataSource(updatedDataSource);

        handleHideModal();
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

  const formatPrice = (price) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(price);
  };

  const getStatusLabel = (status) => {
    switch (status) {
      case -1:
        return "Not Started";
      case 0:
        return "In Progress";
      case 1:
        return "Completed";
      case 2:
        return "Cancelled";
      default:
        return "Unknown";
    }
  };

  const getCheckAcceptLabel = (checkAccept) => {
    return checkAccept ? "Accepted" : "Loading";
  };

  const styles = {
    container: {
      display: "flex",
      alignItems: "center",
    },
    title: {
      fontWeight: "bold",
      color: "#2c2e33",
      marginRight: "0.5rem",
    },
    badge: {
      display: "flex",
      alignItems: "center",
      backgroundColor: "#e6f7e9",
      color: "#28a745",
      fontWeight: "bold",
      padding: "0.2rem 0.75rem",
      borderRadius: "0.25rem",
      marginLeft: "1rem",
    },
    indicator: {
      display: "inline-block",
      width: "0.75rem",
      height: "0.75rem",
      borderRadius: "50%",
      backgroundColor: "#28a745",
      marginRight: "0.5rem",
    },
    date: {
      marginLeft: "0.5rem",
    },
    cardHeader: {
      backgroundColor: "#f7f7f7",
      borderBottom: "1px solid #e6e6e6",
    },
    cardTitle: {
      fontSize: "1.25rem",
      fontWeight: "600",
    },
    media: {
      display: "flex",
      alignItems: "center",
      marginBottom: "20px",
    },
    avatar: {
      borderRadius: "8px",
      overflow: "hidden",
    },
    mediaBody: {
      flex: "1",
      padding: "10px",
    },
    productTitle: {
      fontSize: "1.125rem",
      fontWeight: "600",
      marginBottom: "10px",
    },
    productPrice: {
      fontSize: "1rem",
      fontWeight: "600",
      color: "#28a745",
    },
    buttonPrimary: {
      backgroundColor: "#007bff",
      borderColor: "#007bff",
      fontSize: "0.75rem", // Smaller font size
      padding: "0.25rem 0.5rem", // Smaller padding
      borderRadius: "0.25rem", // Smaller border radius
      width: "80px",
    },
    buttonSuccess: {
      backgroundColor: "#009900",
      borderColor: "#009900",
      fontSize: "0.75rem", // Smaller font size
      padding: "0.25rem 0.5rem", // Smaller padding
      borderRadius: "0.25rem", // Smaller border radius
      width: "100px",
    },
    buttonDanger: {
      backgroundColor: "#dc3545",
      borderColor: "#dc3545",
      fontSize: "0.75rem", // Smaller font size
      padding: "0.25rem 0.5rem", // Smaller padding
      borderRadius: "0.25rem", // Smaller border radius
      width: "80px",
    },
    buttonPrint: {
      backgroundColor: "#003399",
      borderColor: "#003399",
      display: "flex",
      alignItems: "center",
      justifyContent: "center",
      fontSize: "1.5rem", // Adjust font size if needed
      borderRadius: "0.25rem",
      color: "white",
      width: "100%",
    },
  };

  const handlePageChange = (page, pageSize) => {
    setCurrentPage(page);
    setPageSize(pageSize);
  };

  const getCurrentPageData = () => {
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    return dataSource.slice(startIndex, endIndex);
  };

  return (
    <main id="content" role="main" className="main">
      <div className="content container-fluid">
        <div className="page-header d-print-none">
          <div className="row align-items-center">
            <div className="col-sm mb-2 mb-sm-0">
              <nav aria-label="breadcrumb">
                <ol className="breadcrumb breadcrumb-no-gutter"></ol>
              </nav>

              <div style={styles.container}>
                <h1
                  style={{
                    ...styles.title,
                    color: "#6a0dad",
                    textAlign: "center",
                    fontSize: "2rem",
                    margin: "2rem",
                  }}
                >
                  History Payment
                </h1>
              </div>
            </div>
          </div>
        </div>

        <div className="container">
          {getCurrentPageData().map((transaction, transactionIndex) => (
            <div key={transactionIndex} className="card mb-3 mb-lg-5">
              <div className="card-header" style={styles.cardHeader}>
                <h4 className="card-header-title" style={styles.cardTitle}>
                  Payment detail
                  <span className="badge badge-soft-dark rounded-circle ml-1">
                    {transaction.bookingDetails.length}
                  </span>
                </h4>
                <div className="font-size-sm text-body">
                  <span>Customer Name: </span>
                  <span className="font-weight-bold">
                    {transaction.customerName}
                  </span>
                </div>
                <div className="font-size-sm text-body">
                  <span>Payment Method: </span>
                  <span className="font-weight-bold">
                    {transaction.paymentMethod}
                  </span>
                </div>
                <div className="font-size-sm text-body">
                  <span>Total Amount: </span>
                  <span className="font-weight-bold">
                    {formatPrice(transaction.totalAmount)}
                  </span>
                </div>
              </div>

              <div className="card-body">
                {transaction.bookingDetails.map((product, productIndex) => (
                  <div
                    key={productIndex}
                    id={`product-${productIndex}`}
                    className="media-1 mb-3"
                    style={styles.media}
                  >
                    <input
                      type="checkbox"
                      value={`product-${productIndex}`}
                      onChange={handleSelectProduct}
                      className="mr-2 d-print-none"
                    />
                    <div
                      className="avatar avatar-xl mr-3"
                      style={styles.avatar}
                    >
                      <img
                        className="img-fluid"
                        src="src/assets/images/icon/icon_pet_walking.svg"
                        alt="product"
                      />
                    </div>
                    <div className="media-body" style={styles.mediaBody}>
                      <div className="row">
                        <div className="col-md-8 mb-3 mb-md-0">
                          <a
                            className="h5 d-block"
                            href="javascript:void(0)"
                            style={styles.productTitle}
                          >
                            {product.serviceName}
                          </a>
                          {product.comboName && (
                            <div className="font-size-sm text-body">
                              <span>Combo: </span>
                              <span className="font-weight-bold">
                                {product.comboName}
                              </span>
                            </div>
                          )}
                          <div className="font-size-sm text-body">
                            <span>PetName: </span>
                            <span className="font-weight-bold">
                              {product.petName}
                            </span>
                          </div>
                          <Form>
                            <Form.Item label="Date" className="w-1/2">
                              <Space direction="vertical" className="w-full">
                                <div className="w-full">
                                  {product.scheduleDate
                                    ? dayjs(product.scheduleDate).format(
                                        "YYYY-MM-DD HH:mm:ss"
                                      )
                                    : "N/A"}
                                </div>
                              </Space>
                            </Form.Item>
                          </Form>
                          {product.staffId && (
                            <div className="font-size-sm text-body">
                              <span>Staff: </span>
                              <span className="font-weight-bold">
                                {product.staffName || product.staffId}
                              </span>
                            </div>
                          )}
                          <div className="font-size-sm text-body">
                            <span>Status: </span>
                            <span className="font-weight-bold">
                              {getStatusLabel(product.status)}
                            </span>
                          </div>
                          {product.status !== 2 &&(
                            <div className="font-size-sm text-body">
                            <span>Acceptance: </span>
                            <span className="font-weight-bold">
                              {getCheckAcceptLabel(product.checkAccept)}
                            </span>
                          </div>
                          )}                        
                          {product.status === 1 && !product.feedback && (
                            <Button
                              type="button"
                              size="small"
                              onClick={() => {
                                setSelectedProduct(product);
                                setIsFeedbackModalOpen(true);
                              }}
                              className="btn btn-primary btn-pinned mt-3 d-print-none"
                              style={styles.buttonSuccess}
                            >
                              Feedback
                            </Button>
                          )}
                          {product.status === 1 && product.feedback && (
                            <div className="font-size-sm text-body mt-3">
                              Feedbacked
                            </div>
                          )}
                        </div>
                        <div className="col-md-4 align-self-center text-right">
                          <h5 className="mb-0" style={styles.productPrice}>
                            {formatPrice(product.servicePrice)}
                          </h5>
                          {product.status === -1 && (
                            <Button
                              type="button"
                              size="small"
                              onClick={() => handleShowModal(product.bookingId)}
                              className="btn btn-primary btn-pinned mt-3 d-print-none"
                              style={styles.buttonPrimary}
                            >
                              Update
                            </Button>
                          )}
                          {product.status === -1 &&
                            moment(product.bookingSchedule).isAfter(
                              moment().subtract(5, "hours")
                            ) && (
                              <Button
                                type="button"
                                size="small"
                                onClick={() => {
                                  setSelectedProduct(product);
                                  setIsRefundModalOpen(true);
                                }}
                                className="btn btn-danger btn-pinned mt-3 d-print-none"
                                style={styles.buttonDanger}
                              >
                                Refund
                              </Button>
                            )}
                        </div>
                      </div>
                    </div>
                  </div>
                ))}

                <Button
                  className="btn mt-3"
                  onClick={handlePrint}
                  style={styles.buttonPrint}
                  size="small"
                >
                  Print Selected Products
                </Button>
                <hr />
              </div>
            </div>
          ))}

          <Pagination
            current={currentPage}
            pageSize={pageSize}
            total={dataSource.length}
            onChange={handlePageChange}
            className="d-print-none"
          />
        </div>

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

        <Modal
          title={
            <div
              style={{
                textAlign: "center",
                fontSize: "24px",
                fontWeight: "bold",
              }}
            >
              Submit Feedback
            </div>
          }
          open={isFeedbackModalOpen}
          onCancel={() => setIsFeedbackModalOpen(false)}
          onOk={handleFeedbackSubmit}
        >
          <Input.TextArea
            placeholder="Enter your feedback"
            value={feedbackText}
            onChange={(e) => setFeedbackText(e.target.value)}
          />
        </Modal>

        <Modal
          title={
            <div
              style={{
                textAlign: "center",
                fontSize: "24px",
                fontWeight: "bold",
              }}
            >
              Refund Booking
            </div>
          }
          open={isRefundModalOpen}
          onCancel={() => setIsRefundModalOpen(false)}
          onOk={handleRefund}
        >
          <Form layout="vertical">
            <Form.Item label="Bank Name" required>
              <Input
                value={bankName}
                onChange={(e) => setBankName(e.target.value)}
              />
            </Form.Item>
            <Form.Item label="Card Number" required>
              <Input
                value={cardNumber}
                onChange={(e) => setCardNumber(e.target.value)}
              />
            </Form.Item>
          </Form>
        </Modal>
      </div>
    </main>
  );
};

export default Transac;
