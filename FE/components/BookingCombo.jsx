/* eslint-disable no-unused-vars */
/* eslint-disable react/prop-types */
import {
  Button,
  DatePicker,
  Form,
  Input,
  Modal,
  Select,
  Space,
  Table,
  Upload,
  message,
} from "antd";
import { useForm } from "antd/es/form/Form";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";

import AddingPet from "./AddingPet";

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
    return new Intl.NumberFormat("en-US", {
      minimumFractionDigits: 3,
      maximumFractionDigits: 3,
    }).format(price);
  };

  const fetchCombo = async () => {
    try {
      const response = await axios.get("https://localhost:7150/api/Combo");
      setcomboList(response.data);
      console.log(response.data);
      const combo = response.data.find((x) => x.comboId == comboId);
      setPriceCombo(combo.price);
      setPriceCurrent(combo.price);
    } catch (error) {
      message.error("Failed to fetch combo details");
    }
  };

  useEffect(() => {
    fetchCombo();
    fetchMovies();
    fetchStaff();
  }, [dataSource]);
  const [priceCurrent, setPriceCurrent] = useState();
  const [period, setPeriod] = useState(1);

  const handlePrice = (value) => {
    let newPriceCombo = priceCurrent;
    switch (value) {
      case 3:
        newPriceCombo = priceCurrent * 3 * 0.97; // 3 months, 3% discount
        break;
      case 6:
        newPriceCombo = priceCurrent * 6 * 0.94; // 6 months, 6% discount
        break;
      case 9:
        newPriceCombo = priceCurrent * 9 * 0.92; // 9 months, 8% discount
        break;
      default:
        newPriceCombo = priceCurrent; // Default to the initial price if value doesn't match
    }

    console.log("New Price Combo:", newPriceCombo); // Debugging log
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
      render: (image) => (
        <div className="flex justify-center">
          <img
            src={image ? image : null}
            width={150}
            height={150}
            className="rounded-full"
            alt="Pet"
          />
        </div>
      ),
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
    console.log(token);
    const isServiceAlreadyInCart = cart.some((service) =>
      cart.some(
        (item) =>
          item.petId === selectedPetId &&
          item.comboId === service.serviceId &&
          item.date === date.format("YYYY-MM-DDTHH:mm:ss")
      )
    );

    if (isServiceAlreadyInCart) {
      message.warning(
        "This pet has already used this combo at the selected time."
      );
      setLoading(false);
      return;
    }

    setLoading(true); // Start loading

    try {
      // Sending POST request to the backend
      let url = `https://localhost:7150/api/Booking/available?startTime=${date.format(
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
        console.log("Please log in again.");
        setError("Please log in again.");
        setLoading(false); // Stop loading
        return;
      }

      const selectedPet = dataSource.find((pet) => pet.petId === selectedPetId);
      const selectedService = comboList.find(
        (service) => service.comboId === comboId
      );

      console.log(period);
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
          console.log("Please log in again.");
          message.error(error.response.data);
          setTimeout(() => {
            navigate("/");
          }, 1000);
          setLoading(false);
        } else {
          console.error("Error response:", error.response.data);
          message.error(error.response.data || "An error occurred.");
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
                <Option value={3}>3 months (3%)</Option>
                <Option value={6}>6 months (6%)</Option>
                <Option value={9}>9 months (8%)</Option>
              </Select>
            </Form.Item>
            <div className="w-1/2 text-right">
              <p className="text-2xl font-bold">${formatPrice(priceCombo)}</p>
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
