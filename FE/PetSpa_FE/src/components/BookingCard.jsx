import {
  Avatar,
  Button,
  DatePicker,
  Form,
  Modal,
  Select,
  Space,
  Table,
  message,
} from "antd";
import { useForm } from "antd/es/form/Form";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import AddingPet from "./AddingPet";
import moment from "moment";

const { Option } = Select;

const BookingCard = ({ isOpen, handleHideModal, serviceId }) => {
  const [selectStaffId, setSelectStaffId] = useState();
  const [error, setError] = useState(null);
  const [dataSource, setDataSource] = useState([]);
  const [selectedServiceId, setSelectedServiceId] = useState(serviceId);
  const [date, setDate] = useState(null);
  const [staffList, setStaffList] = useState([]);
  const [selectedPetId, setSelectedPetId] = useState(null);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const [form] = useForm();
  const [cart, setCart] = useState(() => {
    const savedCart = localStorage.getItem("cart");
    return savedCart ? JSON.parse(savedCart) : [];
  });
  const [priceCombo, setPriceCombo] = useState();
  const [isPetOpen, setIsPetOpen] = useState(false);
  const [services, setServices] = useState([]);
  const [period, setPeriod] = useState(1);
  const [priceCurrent, setPriceCurrent] = useState();
  useEffect(() => {
    fetchPets();
    fetchStaff();
    fetchServices();
  }, []);
  useEffect(() => {
    fetchPets();
  }, [dataSource]);
  const disablePastDates = (current) => {
    return current && current < moment().startOf("day");
  };
  const handlePrice = (value) => {
    // Kiểm tra giá trị của priceCurrent
    if (
      !priceCurrent ||
      typeof priceCurrent !== "number" ||
      priceCurrent <= 0
    ) {
      console.error("Invalid priceCurrent value:", priceCurrent);
      return;
    }

    let newPriceCombo = priceCurrent;

    switch (value) {
      case 3:
        newPriceCombo = priceCurrent * 3 * 0.98; // 3 tháng, giảm giá 3%
        break;
      case 6:
        newPriceCombo = priceCurrent * 6 * 0.96; // 6 tháng, giảm giá 6%
        break;
      case 9:
        newPriceCombo = priceCurrent * 9 * 0.94; // 9 tháng, giảm giá 8%
        break;
      default:
        newPriceCombo = priceCurrent; // Mặc định trở về giá ban đầu nếu giá trị không khớp
    }

    // Đảm bảo newPriceCombo không bị gán giá trị undefined hoặc null
    if (isNaN(newPriceCombo) || newPriceCombo <= 0) {
      console.error("Invalid newPriceCombo value:", newPriceCombo);
      newPriceCombo = priceCurrent; // Trở lại giá trị ban đầu nếu có lỗi
    }

    // Cập nhật giá trị
    setPriceCombo(newPriceCombo);
    setPeriod(value);

    console.log("Updated price combo:", newPriceCombo, "for period:", value);
  };

  const formatPrice = (price) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(price);
  };

  const handlePetModalOpen = () => {
    setIsPetOpen(true);
  };

  const handlePetModalClose = () => {
    setIsPetOpen(false);
  };

  const handleOk = () => {
    form.submit();
  };

  async function fetchPets() {
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);

    if (userInfo != null) {
      if (userInfo.data.user.id != null) {
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
            message.warning("Please login in.");
            navigate("/login");
            return;
          }

          const result = response.data;

          localStorage.setItem("pets", JSON.stringify(result));
          setDataSource(result.data.pets.filter((x) => x.status !== false));
        } catch (error) {
          if (error.response && error.response.status === 401) {
            localStorage.removeItem("user-info");
            message.error("Token expired. Please log in again.");
            navigate("/login");
          } else {
            message.error("An error occurred. Please try again.");
          }
        }
      } else {
        navigate("/login");
      }
    } else {
      navigate("/login");
    }
  }

  const fetchStaff = async () => {
    try {
      const response = await axios.get("https://localhost:7150/api/Staff");
      setStaffList(response.data.data);
    } catch (error) {
      message.error("Failed to fetch staff members");
    }
  };

  const columns = [
    {
      title: (
        <span className="text-lg text-blue-500 font-semibold">Poster</span>
      ),
      dataIndex: "image",
      key: "image",
      align: "center",
      width: "30%",
      render: (image) => <Avatar size={56} src={image} />,
    },
    {
      title: <span className="text-lg text-blue-500 font-semibold">Name</span>,
      dataIndex: "petName",
      key: "petName",
      align: "center",
      width: "27%",
      render: (petName) => (
        <span className="text-base text-black font-medium">{petName}</span>
      ),
    },
    {
      title: (
        <span className="text-lg text-blue-500 font-semibold">Action</span>
      ),
      dataIndex: "petId",
      key: "petId",
      align: "center",
      width: "28%",
      render: (petId) => (
        <Button
          onClick={() => setSelectedPetId(petId)}
          className={`border-2 rounded-md px-4 py-2 transition-colors ${
            petId === selectedPetId
              ? "bg-blue-400 text-white border-blue-500"
              : "bg-white text-blue-600 border-blue-700 hover:bg-blue-600 hover:text-white"
          }`}
        >
          Choice
        </Button>
      ),
    },
  ];

  const fetchServices = async () => {
    try {
      const response = await axios.get("https://localhost:7150/api/Service");
      const result = response.data;
      setServices(result.data.data);
      localStorage.setItem("service", JSON.stringify(result));
      if (selectedServiceId) {
        const service = result.data.data.find(
          (x) => x.serviceId === selectedServiceId
        );
        if (service) {
          setPriceCombo(service.price);
          setPriceCurrent(service.price);
        }
      }
    } catch (error) {
      console.error("Error fetching services:", error);
    }
  };

  const handleChoice = async () => {
    const savedCart = localStorage.getItem("cart");
    const cart = savedCart ? JSON.parse(savedCart) : [];
    if (!selectedPetId || !date) {
      setError("Please select a pet and a date.");
      return;
    }
  
    setError("");
  
    // Retrieve user information from localStorage
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = userInfoString ? JSON.parse(userInfoString) : null;
    const token = userInfo?.data?.token;
  
    // Format the date for comparison
    const formattedDate = date.format("YYYY-MM-DDTHH:mm:ss");
    const selectedStartTime = moment(formattedDate);
    const selectedEndTime = selectedStartTime.clone().add(30, 'minutes'); // Assuming a fixed duration of 30 minutes
  
    // Check if the booking time is at least one hour from now
    const now = moment();
    if (date.diff(now, "hours") < 1) {
      message.warning("The booking time must be at least one hour from now.");
      return;
    }

  
    // Define validation functions for each condition
    const isOverlap = (item) => {
      const itemStartTime = moment(item.date);
      const itemEndTime = itemStartTime.clone().add(30, 'minutes'); // Assuming a fixed duration of 30 minutes
      return (
        selectedStartTime.isBetween(itemStartTime, itemEndTime, null, '[)') ||
        selectedEndTime.isBetween(itemStartTime, itemEndTime, null, '(]') ||
        itemStartTime.isBetween(selectedStartTime, selectedEndTime, null, '[)') ||
        itemEndTime.isBetween(selectedStartTime, selectedEndTime, null, '(]')
      );
    };
  
    const isOverlap1 = (itemStartTime, itemEndTime, startTime, endTime) => {
      return (
        startTime.isBetween(itemStartTime, itemEndTime, null, '[)') ||
        endTime.isBetween(itemStartTime, itemEndTime, null, '(]') ||
        itemStartTime.isBetween(startTime, endTime, null, '[)') ||
        itemEndTime.isBetween(startTime, endTime, null, '(]')
      );
    };
    const isCondition1 = cart.some(
      (item) =>
        item.staffId === selectStaffId &&
        item.serviceId === selectedServiceId &&
        item.petId === selectedPetId &&
        isOverlap(item)
    );
  
    const isCondition2 = cart.some(
      (item) =>
        item.staffId === selectStaffId &&
        item.serviceId === selectedServiceId &&
        isOverlap(item)
    );
  
    const isCondition3 = cart.some(
      (item) =>
        item.petId === selectedPetId &&
        item.serviceId === selectedServiceId &&
        isOverlap(item)
    );
  
  
    const isCondition5 = cart.some(
      (item) =>
        item.petId === selectedPetId &&
        isOverlap(item)
    );
  
    const isCondition6 = cart.some(
      (item) =>
        item.staffId === selectStaffId &&
        isOverlap(item)
    );
    
    const isBookingConflict = cart.some((item) => {
      if (item.petId !== selectedPetId) return false;
      const itemPeriod = item.period || 1;
      const selectedPeriod = period || 1;
      for (let i = 1; i <= itemPeriod; i++) {
        const itemMonth = moment(item.date).add(i-1, 'months');
        const itemStartTime = itemMonth.clone();
        const itemEndTime = itemStartTime.clone().add(30, 'minutes');
  
        for (let j = 1; j <= selectedPeriod; j++) {
          const selectedMonth = selectedStartTime.clone().add(j-1, 'months');
          const selectedStartTimeRecurring = selectedMonth.clone();
          const selectedEndTimeRecurring = selectedStartTimeRecurring.clone().add(30, 'minutes');
  
          // Check for overlapping times
          if (isOverlap1(itemStartTime, itemEndTime, selectedStartTimeRecurring, selectedEndTimeRecurring)) {
            return true;
          }
        }
      }
      return false;
    });
  
    if (isBookingConflict) {
      message.warning("The selected period conflicts with an existing booking or recurring booking time.");
      return;
    }
  
  
    if (isBookingConflict) {
      message.warning("The selected period conflicts with an existing booking or recurring booking time.");
      return;
    }
    
    if (isCondition1) {
      message.warning("The same staff, service, pet, and time are already booked within the duration.");
      return;
    }
  
    if (isCondition2) {
      message.warning("The same staff and service are already booked at the same time within the duration.");
      return;
    }
  
    if (isCondition3) {
      message.warning("The same pet and service are already booked at the same time within the duration.");
      return;
    }

  
    if (isCondition5) {
      message.warning("The same pet is already booked at the same time within the duration.");
      return;
    }
  
    if (isCondition6) {
      message.warning("The same staff is already booked at the same time within the duration.");
      return;
    }
  
  
    // Proceed with booking logic
    setLoading(true); // Start loading

    try {
      // Sending POST request to the backend
      let url = `https://localhost:7150/api/Booking/available?startTime=${date.format(
        "YYYY-MM-DDTHH:mm:ss"
      )}&serviceCode=${selectedServiceId}`;

      if (selectStaffId) {
        url += `&staffId=${selectStaffId}`;
      }
      if (period) {
        url += `&periodMonths=${period}`;
      }
      const response = await axios.get(url, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.status === 401) {
        console.log("Token expired. Please log in again.");
        setError("Token expired. Please log in again.");
        setLoading(false); // Stop loading
        return;
      }

      const selectedPet = dataSource.find((pet) => pet.petId === selectedPetId);
      const selectedService = services.find(
        (service) => service.serviceId === selectedServiceId
      );

      const newItem = {
        serviceId: selectedService.serviceId,
        date: date.format("YYYY-MM-DDTHH:mm:ss"),
        serviceName: selectedService.serviceName,
        servicePrice: priceCombo, // Use the updated priceCombo
        petId: selectedPet.petId,
        petName: selectedPet.petName,
        period: period,
        staffId: selectStaffId ? selectStaffId : "",
      };

      // Proceed with payment process
      const paymentResponse = await processPayment(newItem);

      if (paymentResponse.success) {
        setCart((prevCart) => [...prevCart, newItem]);
        message.success("Service booked successfully");
        // Save cart to localStorage
        localStorage.setItem("cart", JSON.stringify([...cart, newItem]));
        setSelectedPetId(null);
        setDate(null);
        setSelectStaffId(null);
        setSelectedServiceId(null);
        form.resetFields();
        handleHideModal();
      } else {
        message.error("Payment failed. Please try again.");
      }

      setLoading(false); // Reset Ant Design form fields
    } catch (error) {
      if (error.response) {
        console.log(error.response.data);
        if (error.response.status === 401) {
          console.log("Token expired. Please log in again.");
          message.warning("Please log in again.");
          localStorage.removeItem("user-info");
          setTimeout(() => {
            navigate("/login");
          }, 1000);

          setLoading(false);
        } else {
          console.error("Error response:", error.response.data);
          message.error(error.response.data);
          setLoading(false);
          return;
        }
      } else {
        console.error("Error:", error);
        message.error("An unexpected error occurred.");
        setLoading(false);
        return;
      }
    }
  };

  // Mock function for processing payment
  const processPayment = async (item) => {
    // Implement your payment processing logic here
    // For example, call your payment API and return the response
    return {
      success: true, // Change this based on actual payment response
      data: item,
    };
  };

  return (
    <>
      <Modal open={isOpen} footer={null} onCancel={handleHideModal} width={800}>
        <Button type="primary" onClick={handlePetModalOpen}>
          +
        </Button>
        <Table
          columns={columns}
          dataSource={dataSource}
          rowClassName={(record) =>
            record.petId === selectedPetId ? "bg-cyan-100" : ""
          }
          rowKey="petId"
          pagination={false} // Remove if you need pagination
          scroll={{ x: 800 }} // Enables horizontal scrolling
        />
        <Form layout="vertical" className="mt-5" form={form}>
          <div className="flex space-x-4">
            <Form.Item label="Date" className="w-1/2">
              <Space direction="vertical" className="w-full">
                <DatePicker
                  showTime
                  value={date}
                  onChange={(date) => setDate(date)}
                  className="w-full"
                  disabledDate={disablePastDates} // Add the disabledDate prop
                />
              </Space>
            </Form.Item>
            <Form.Item label="Select Staff" className="w-1/2">
              <Select
                showSearch
                placeholder="Select a staff"
                optionFilterProp="children"
                onChange={(value) => setSelectStaffId(value)}
                className="w-full"
              >
                {Array.isArray(staffList) &&
                  staffList.map((staff) => (
                    <Option key={staff.staffId} value={staff.staffId}>
                      {staff.fullName}
                    </Option>
                  ))}
              </Select>
            </Form.Item>
          </div>
          <div className="flex space-x-4 mt-4 items-center">
            <Form.Item label="Periodic Option" className="w-1/2">
              <Select
                placeholder="Select a period"
                onChange={(value) => handlePrice(value)}
                className="w-full"
              >
                <Option value={1}>1 time</Option>
                <Option value={3}>3 months (2%)</Option>
                <Option value={6}>6 months (4%)</Option>
                <Option value={9}>9 months (6%)</Option>
              </Select>
            </Form.Item>
            <div className="w-1/2 text-right">
              <p className="text-2xl font-bold">{formatPrice(priceCombo)}</p>
            </div>
          </div>
          <p className="text-red-500">{error}</p>
        </Form>
        <div className="flex justify-end mt-2">
          <Button type="primary" onClick={handleChoice} loading={loading}>
            Book
          </Button>
        </div>
      </Modal>

      <AddingPet
        isOpen={isPetOpen}
        handleHideModal={handlePetModalClose}
        setDataSource={setDataSource}
      />
    </>
  );
};

export default BookingCard;
