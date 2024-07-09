import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowLeft, faArrowUp } from "@fortawesome/free-solid-svg-icons";
import Slider from "react-slick";
import { faStar } from "@fortawesome/free-solid-svg-icons";
import { faQuoteRight } from "@fortawesome/free-solid-svg-icons";
import { faArrowRight } from "@fortawesome/free-solid-svg-icons";
import "../../assets/js/email-decode.min.js";
import "../../assets/js/jquery.min.js";
import "../../assets/js/popper.min.js";
import "../../assets/js/bootstrap.min.js";
import "../../assets/js/bootstrap-dropdown-ml-hack.js";
import "../../assets/js/slick.min.js";
import "../../assets/js/magnific-popup.min.js";
import "../../assets/js/nice-select.min.js";
import "../../assets/js/jquery-ui.js";
import "../../assets/js/vanilla-calendar.min.js";
import "../../assets/js/countdown.js";
import "../../assets/js/main.js";
import { Link } from "react-router-dom";
import { useEffect, useState } from "react";
import axios from "axios";
import PropTypes from "prop-types";
// import {
//   Button,
//   DatePicker,
//   Form,
//   Modal,
//   Select,
//   Space,
//   Table,
//   message,
// } from "antd";
import { useNavigate } from "react-router-dom";
import Search from "antd/es/input/Search.js";
// import { Option } from "antd/es/mentions/index.js";
// import { useForm } from "antd/es/form/Form.js";
import { message } from "antd";
import BookingCard from "../BookingCard.jsx";
import ProtectedRoute from "../ProtectedRoute.jsx";

function Service() {
  const CustomNextArrow = ({ onClick }) => (
    <button type="button" className="arrow-button top-right" onClick={onClick}>
      <FontAwesomeIcon icon={faArrowRight} />
    </button>
  );

  // CustomPrevArrow component
  const CustomPrevArrow = ({ onClick }) => (
    <button type="button" className="arrow-button top-left" onClick={onClick}>
      <FontAwesomeIcon icon={faArrowLeft} />
    </button>
  );
  CustomPrevArrow.propTypes = {
    onClick: PropTypes.func.isRequired,
  };

  CustomNextArrow.propTypes = {
    onClick: PropTypes.func.isRequired,
  };
  const [bookings, setBookings] = useState([]);

  useEffect(() => {
    const fetchBookings = async () => {
      try {
        const response = await axios.get("https://localhost:7150/api/Booking");
        if (response.data.status) {
          const filteredBookings = response.data.data.filter(
            (booking) => booking.feedback != null
          );
          setBookings(filteredBookings);
          console.log(filteredBookings);
        } else {
          console.error("Failed to fetch bookings");
        }
      } catch (error) {
        console.error("Error fetching bookings:", error);
      }
    };
    
    fetchBookings();
  }, []);

  const settings = {
    dots: false,
    centerMode: true,
    slidesToShow: 3,
    slidesToScroll: 1,
    infinite: true,
    centerPadding: "0",
    prevArrow: <CustomPrevArrow />,
    nextArrow: <CustomNextArrow />,
  };
  const [error, setError] = useState();
  const [services, setServices] = useState([]);
  const [isOpen, setIsOpen] = useState(false);
  const [dataSource, setDataSource] = useState([]);
  const navigate = useNavigate();
  const [selectedServiceId, setSelectedServiceId] = useState(null);
  // const [date, setDate] = useState();
  // const [selectStaffId, setSelectStaffId] = useState();
  // const [form] = useForm();
  // const [selectedPetId, setSelectedPetId] = useState(null);
  // const [loading, setLoading] = useState(false);
  // eslint-disable-next-line no-unused-vars
  const [cart, setCart] = useState(() => {
    const savedCart = localStorage.getItem("cart");
    return savedCart ? JSON.parse(savedCart) : [];
  });

  const handleSearch = async (searchTerm) => {
    if (!searchTerm) {
      // If search term is empty, fetch all services
      try {
        const response = await axios.get("https://localhost:7150/api/Service");
        const result = response.data;
        
        setServices(result.data.data);
        setError(""); // Clear any previous errors
      } catch (error) {
        setError("An error occurred while fetching services");
      }
      return;
    }

    try {
      const response = await axios.get(
        `https://localhost:7150/api/Service/search?searchTerm=${searchTerm}`
      );
      setServices(response.data.data);
      setError("");
    } catch (err) {
      if (err.response) {
        setError(err.response.data.msg);
      } else {
        setError("An error occurred while searching");
      }
    }
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
            // toast.error("Token hết hạn. Vui lòng đăng nhập lại.");
            localStorage.removeItem("user-info");
            message.warning("Please log in.");
            navigate("/login");
            return;
          }

          const result = response.data;
          localStorage.setItem("pets", JSON.stringify(result));

          setDataSource(result.data.pets.filter((x) => x.status !== false));
        } catch (error) {
          if (error.response && error.response.status === 401) {
            localStorage.removeItem("user-info");
            message.warning("Please log in.");
            navigate("/login");
          } else {
            console.log();
          }
        }
      } else {
        message.warning("Please log in.");
        navigate("/login");
      }
    } else {
      message.warning("Please log in.");
      navigate("/login");
    }
  }

  function handleHideModal() {
    setIsOpen(false);
  }

  const handleBookNow = (serviceID) => {
    // Do something with the serviceID, such as redirecting to booking page or sending request to server
    // var petId = "1122233333";
    fetchMovies();

    setIsOpen(true);
    setSelectedServiceId(serviceID);
  };
  const fetchServices = async () => {
    try {
      const response = await axios.get("https://localhost:7150/api/Service");
      const result = response.data;
      // Filter out services with status false
      
      setServices(result.data.data);
     
    } catch (error) {
      console.error("Error fetching services:", error);
    }
  };

  useEffect(() => {
    fetchServices();
  }, [dataSource]);
  return (
    <div>
      <div>
        <meta charSet="utf-8" />
        <meta
          name="viewport"
          content="width=device-width,initial-scale=1,shrink-to-fit=no"
        />
        <meta httpEquiv="x-ua-compatible" content="ie=edge" />
        <title>Services - Petpia – Pet Care Service Template</title>
        <link
          rel="shortcut icon"
          href="src/assets/images/logo/favourite_icon.png"
        />
        <link rel="stylesheet" href="src/assets/css/all.css" />

        <script
          async
          src="https://www.googletagmanager.com/gtag/js?id=G-TSZFRP5V2X"
        ></script>
      </div>

      <div className="body_wrap">
        <div className="backtotop">
          <a href="#" className="scroll">
            <FontAwesomeIcon icon={faArrowUp} />{" "}
          </a>
        </div>
        <main>
          <section className="breadcrumb_section">
            <div className="container">
              <div className="row">
                <div className="col col-lg-4 col-md-7 col-sm-9">
                  <ul className="breadcrumb_nav">
                    <li>
                      <Link to="/Homepage">Home</Link>
                    </li>
                    <li>Service</li>
                  </ul>
                  <h1 className="page_title">Our Services</h1>
                  <p className="page_description mb-0">
                    At Pet Pia, we understand that your pets are not just
                    animals but cherished members of your family. Our Pets' Spa
                    Management services are designed to provide your furry
                    friends with the ultimate pampering and care they deserve.
                  </p>
                </div>
              </div>
            </div>
            <div className="breadcrumb_img dog_image">
              <img
                src="src/assets/images/breadcrumb/breadcrumb_img_2.png"
                alt="Pet Care Service"
              />
            </div>
          </section>
          <section
            className="service_section section_space_lg"
            style={{
              backgroundImage:
                'url("src/assets/images/overlay/shapes_overlay_6.svg")',
            }}
          >
            <div className="container">
              <div className="section_title text-center">
                <h2 className="title_text mb-0">
                  <span className="sub_title">Our Services</span> All Pet Care
                  Services
                </h2>
              </div>
              <div className="flex items-center justify-center flex-wrap bg-gray-100 p-4 rounded-lg shadow-md mb-4">
                <Search
                  placeholder="Input search text"
                  allowClear
                  onSearch={handleSearch}
                  className="w-full"
                  size="large"
                  enterButton
                />
                {error && (
                  <p className="text-red-500 mt-2 md:mt-0 md:ml-4">{error}</p>
                )}
              </div>
              <div className="row justify-content-center">
                {Array.isArray(services) &&
                  services.map((service) => {
                    const formattedPrice = new Intl.NumberFormat("vi-VN", {
                      style: "currency",
                      currency: "VND",
                    }).format(service.price);
                    const backgroundImage = service.serviceImage
                      ? `url(${service.serviceImage})`
                      : 'url("src/assets/images/shape/shape_path_1.svg")';
                    return (
                      <div key={service.serviceId} className="col col-lg-4">
                        <div
                          className="service_item"
                          style={{
                            backgroundImage: backgroundImage,
                          }}
                        >
                          <div className="title_wrap">
                            <div className="item_icon">
                              <img
                                src="src/assets/images/icon/icon_pet_walking.svg"
                                alt=""
                              />
                            </div>
                            <h3 className="item_title mb-0">
                              {service.serviceName}
                            </h3>
                          </div>
                          <p>{service.serviceDescription}</p>
                          <div className="item_price">
                            <span>{formattedPrice}</span>
                          </div>
                          <div>
      {service.status ? (
        <Link
          className="btn_unfill"
          onClick={() => handleBookNow(service.serviceId)}
        >
          <span>Schedule Your Appointment</span>{" "}
          <FontAwesomeIcon icon={faArrowRight} />
        </Link>
      ) : (
        <span className="btn_unfill">The service is currently down.</span>
      )}
    </div>
                          {isOpen &&
                            selectedServiceId === service.serviceId && (
                              <ProtectedRoute allowedRoles={["Customer"]}>
                                <BookingCard
                                  isOpen={isOpen}
                                  handleHideModal={handleHideModal}
                                  serviceId={service.serviceId}
                                />
                              </ProtectedRoute>
                            )}
                          <div className="decoration_image">
                            <img
                              src="src/assets/images/shape/shape_paws.svg"
                              alt="Pet Paws"
                            />
                          </div>
                        </div>
                      </div>
                    );
                  })}
              </div>
            </div>
          </section>
          <section
            className="feature_service decoration_wrap"
            style={{
              backgroundImage:
                'url("src/assets/images/background/shape_bg_2.png")',
            }}
          ></section>

          <section className="testimonial_section section_space_lg">
            <div className="container">
              <div className="section_title">
                <h2 className="title_text mb-0">
                  <span className="sub_title">Our Feedback</span>
                  What People Say
                </h2>
              </div>
            </div>

            <div className="testimonial_carousel">
              <Slider {...settings}>
                {bookings.map((booking) => (
                  <div className="col carousel_item" key={booking.bookingId}>
                    <div className="testimonial_item">
                      <div className="testimonial_admin">
                        <div className="admin_thumbnail">
                          <img
                            src="src/assets/images/meta/thumbnail_img_1.png"
                            alt="Pet Thumbnail Image"
                          />
                        </div>
                        <div className="admin_info">
                          <h4 className="admin_name">
                            {booking.bookingDetails[0]?.service?.serviceName ||
                              "Service Name"}
                          </h4>
                          <span className="admin_designation">
                            {booking.cus?.fullName || "Customer Name"}
                          </span>
                        </div>
                      </div>
                      <ul className="rating_star">
                        {[...Array(5)].map((star, index) => (
                          <li key={index}>
                            <FontAwesomeIcon icon={faStar} />{" "}
                          </li>
                        ))}
                      </ul>
                      <p className="mb-0">
                        {booking.feedback || "No feedback provided."}
                      </p>
                      <span className="quote_icon">
                        <FontAwesomeIcon icon={faQuoteRight} />{" "}
                      </span>
                    </div>
                  </div>
                ))}
              </Slider>
            </div>
          </section>
        </main>
      </div>
    </div>
  );
}

export default Service;
