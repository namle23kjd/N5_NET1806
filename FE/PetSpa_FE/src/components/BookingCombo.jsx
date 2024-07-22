import {
  Avatar,
  Button,
  DatePicker,
  Form,
  message,
  Modal,
  Select,
  Space,
  Table,
} from "antd";
import { useForm } from "antd/es/form/Form";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";

import AddingPet from "./AddingPet";
import moment from "moment";

const { Option } = Select;

const BookingCombo = ({ isOpen, handleHideModal, comboId }) => {
  const [selectStaffId, setSelectStaffId] = useState();
  const [error, setError] = useState(null);
  const [dataSource, setDataSource] = useState([]);
  const [selectedComboId, setSelectedComboId] = useState(comboId);
  const [date, setDate] = useState(null);
  const [staffList, setStaffList] = useState([]);
  const [selectedPetId, setSelectedPetId] = useState(null);
  const [loading, setLoading] = useState(false);
  const [comboList, setcomboList] = useState([]);
  const [priceCombo, setPriceCombo] = useState();
  const [isPetOpen, setIsPetOpen] = useState(false);

  const navigate = useNavigate();
  const [form] = useForm();
  const [cart, setCart] = useState(() => {
    const savedCart = localStorage.getItem("cart");
    return savedCart ? JSON.parse(savedCart) : [];
  });
  const formatPrice = (price) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(price);
  };

  const fetchCombo = async () => {
    try {
      const response = await axios.get("https://localhost:7150/api/Combo");
      setcomboList(response.data);

      const combo = response.data.find((x) => x.comboId == comboId);
      setPriceCombo(combo.price);
      setPriceCurrent(combo.price);
    } catch (error) {
      message.error("Failed to fetch combo details");
    }
  };

  useEffect(() => {
    fetchCombo();
    fetchStaff();
    fetchMovies();
  }, []);
  useEffect(() => {
    fetchMovies();
  }, [dataSource]);
  const [priceCurrent, setPriceCurrent] = useState();
  const [period, setPeriod] = useState(1);
  const [selectPeriod, setSelectPeriod] = useState();
  const handlePrice = (value) => {
    setSelectPeriod(value);
    let newPriceCombo = priceCurrent;
    switch (value) {
      case 3:
        newPriceCombo = priceCurrent * 3 * 0.98; // 3 months, 3% discount
        break;
      case 6:
        newPriceCombo = priceCurrent * 6 * 0.96; // 6 months, 6% discount
        break;
      case 9:
        newPriceCombo = priceCurrent * 9 * 0.94; // 9 months, 8% discount
        break;
      default:
        newPriceCombo = priceCurrent; // Default to the initial price if value doesn't match
    }

    setPriceCombo(newPriceCombo);
    setPeriod(value);
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

  async function fetchMovies() {
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

  const handleChoice = async () => {
    const savedCart = localStorage.getItem("cart");
    const cart = savedCart ? JSON.parse(savedCart) : [];
    if (selectedPetId == null || date == null) {
      setError("Please select a pet and a date.");
      return;
    }
    setError("");
    const userInfoString = localStorage.getItem("user-info");
    const userInfo = JSON.parse(userInfoString);
    const token = userInfo?.data?.token;

    const formattedDate = date.format("YYYY-MM-DDTHH:mm:ss");
    const selectedStartTime = moment(formattedDate);
    const selectedEndTime = selectedStartTime.clone().add(91, "minutes"); // Assuming a fixed duration of 30 minutes

    const now = moment();
    if (date.diff(now, "hours") < 1) {
      message.warning("The booking time must be at least one hour from now.");
      return;
    }

    // Define validation functions for each condition
    const isOverlap = (item) => {
      const itemStartTime = moment(item.date);
      const itemEndTime = itemStartTime.clone().add(91, "minutes"); // Assuming a fixed duration of 30 minutes
      return (
        selectedStartTime.isBetween(itemStartTime, itemEndTime, null, "[)") ||
        selectedEndTime.isBetween(itemStartTime, itemEndTime, null, "(]") ||
        itemStartTime.isBetween(
          selectedStartTime,
          selectedEndTime,
          null,
          "[)"
        ) ||
        itemEndTime.isBetween(selectedStartTime, selectedEndTime, null, "(]")
      );
    };

    const isOverlap1 = (itemStartTime, itemEndTime, startTime, endTime) => {
      return (
        startTime.isBetween(itemStartTime, itemEndTime, null, "[)") ||
        endTime.isBetween(itemStartTime, itemEndTime, null, "(]") ||
        itemStartTime.isBetween(startTime, endTime, null, "[)") ||
        itemEndTime.isBetween(startTime, endTime, null, "(]")
      );
    };
    const isCondition1 = cart.some(
      (item) =>
        item.staffId === selectStaffId &&
        item.serviceId === selectedComboId &&
        item.petId === selectedPetId &&
        isOverlap(item)
    );

    const isCondition2 = cart.some(
      (item) =>
        item.staffId === selectStaffId &&
        item.serviceId === selectedComboId &&
        isOverlap(item)
    );

    const isCondition3 = cart.some(
      (item) =>
        item.petId === selectedPetId &&
        item.serviceId === selectedComboId &&
        isOverlap(item)
    );

    const isCondition5 = cart.some(
      (item) => item.petId === selectedPetId && isOverlap(item)
    );

    const isCondition6 = cart.some(
      (item) => item.staffId === selectStaffId && isOverlap(item)
    );

    const isBookingConflict = cart.some((item) => {
      if (item.petId !== selectedPetId) return false;
      const itemPeriod = item.period || 1;
      const selectedPeriod = period || 1;
      for (let i = 1; i <= itemPeriod; i++) {
        const itemMonth = moment(item.date).add(i - 1, "months");
        const itemStartTime = itemMonth.clone();
        const itemEndTime = itemStartTime.clone().add(91, "minutes");

        for (let j = 1; j <= selectedPeriod; j++) {
          const selectedMonth = selectedStartTime.clone().add(j - 1, "months");
          const selectedStartTimeRecurring = selectedMonth.clone();
          const selectedEndTimeRecurring = selectedStartTimeRecurring
            .clone()
            .add(91, "minutes");

          // Check for overlapping times
          if (
            isOverlap1(
              itemStartTime,
              itemEndTime,
              selectedStartTimeRecurring,
              selectedEndTimeRecurring
            )
          ) {
            return true;
          }
        }
      }
      return false;
    });

    if (isBookingConflict) {
      message.warning(
        "The selected period conflicts with an existing booking or recurring booking time."
      );
      return;
    }

    if (isCondition1) {
      message.warning(
        "The same staff, service, pet, and time are already booked within the duration."
      );
      return;
    }

    if (isCondition2) {
      message.warning(
        "The same staff and service are already booked at the same time within the duration."
      );
      return;
    }

    if (isCondition3) {
      message.warning(
        "The same pet and service are already booked at the same time within the duration."
      );
      return;
    }

    if (isCondition5) {
      message.warning(
        "The same pet is already booked at the same time within the duration."
      );
      return;
    }

    if (isCondition6) {
      message.warning(
        "The same staff is already booked at the same time within the duration."
      );
      return;
    }

    setLoading(true); // Start loading

    try {
      // Sending POST request to the backend
      let url = `https://localhost:7150/api/Booking/availableForPeriod?startTime=${date.format(
        "YYYY-MM-DDTHH:mm:ss"
      )}&serviceCode=${selectedComboId}`;

      if (selectStaffId) {
        url += `&staffId=${selectStaffId}`;
      }
      const response = await axios.get(url, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      if (response.status === 401) {
        setError("Please log in again.");
        setLoading(false); // Stop loading
        return;
      }

      const selectedPet = dataSource.find((pet) => pet.petId === selectedPetId);
      const selectedService = comboList.find(
        (service) => service.comboId === comboId
      );

      const newItem = {
        date: date.format("YYYY-MM-DDTHH:mm:ss"),
        serviceName: selectedService.comboType,
        servicePrice: priceCombo,
        petId: selectedPet.petId,
        petName: selectedPet.petName,
        comboDetails: selectedService.services,
        period: period,
        staffId: selectStaffId ? selectStaffId : "",
      };
      setCart((prevCart) => [...prevCart, newItem]);
      message.success("Booking for pet successfully");
      // Lưu giỏ hàng vào localStorage
      localStorage.setItem("cart", JSON.stringify([...cart, newItem]));
      setSelectedPetId(null);
      setDate(null);
      setSelectStaffId(null);
      form.resetFields();
      setLoading(false); // Reset Ant Design form fields
      handleHideModal();
    } catch (error) {
      if (error.response) {
        if (error.response.status === 401) {
          message.error(error.response.data);
          localStorage.removeItem("user-info");
          setTimeout(() => {
            navigate("/login");
          }, 1000);
          setLoading(false);
        } else {
          console.error("Error response:", error.response.data);
          message.warning(error.response.data || "An error occurred.");
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

  return (
    <div>
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
    </div>
  );
};

export default BookingCombo;
