import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faClock,
  faEnvelope,
  faLocationDot,
  faPaw,
  faPhoneVolume,
} from "@fortawesome/free-solid-svg-icons";
import { faArrowUp } from "@fortawesome/free-solid-svg-icons";
import { faCheckCircle } from "@fortawesome/free-solid-svg-icons";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

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

import { faFacebook, faInstagram } from "@fortawesome/free-brands-svg-icons";
import { Link } from "react-router-dom";
import { useEffect, useState } from "react";
import { message } from "antd";
import axios from "axios";
import BookingCombo from "../BookingCombo.jsx";
import ComboCard from "../ComboCard/ComboCard.jsx";

function HomePage() {
  const [comboList, setComboList] = useState([]);
  const [staffList, setStaffList] = useState([]);

  const fetchCombo = async () => {
    try {
      const response = await axios.get("https://localhost:7150/api/Combo");
      setComboList(response.data);
    } catch (error) {
      message.error("Failed to fetch combos");
    }
  };

  const fetchStaff = async () => {
    try {
      const response = await axios.get("https://localhost:7150/api/Staff");
      setStaffList(response.data.data);
    } catch (error) {
      message.error("Failed to fetch staff members");
    }
  };

  useEffect(() => {
    fetchCombo();
    fetchStaff();
  }, []);

  const [isOpen, setIsOpen] = useState(false);
  const [selectComboId, setSelectComboId] = useState();
  const handleOpenModal = (comboId) => {
    setIsOpen(true);
    setSelectComboId(comboId);
  };
  const handleHideModal = () => setIsOpen(false);

  return (
    <>
      <div className="body_wrap">
        <div className="backtotop">
          <a href="#" className="scroll">
            <FontAwesomeIcon icon={faArrowUp} />{" "}
          </a>
        </div>
        <main>
          <section
            className="banner_section banner_style_1 decoration_wrap"
            style={{
              backgroundImage:
                'url("src/assets/images/background/shape_bg_1.png")',
            }}
          >
            <div
              className="section_overlay"
              style={{
                backgroundImage:
                  'url("src/assets/images/overlay/shapes_overlay_1.svg")',
              }}
            />
            <div className="container">
              <div className="banner_content">
                <h1 className="banner_title">
                  The Best Team for Pet Care Services
                </h1>
                <p className="banner_description">
                  Entrust your furry friends to the most dedicated and
                  experienced team. Our comprehensive pet care services ensure
                  your pets receive the love, attention, and professional care
                  they deserve.
                </p>
                <Link className="btn btn_primary" to="/service">
                  <FontAwesomeIcon icon={faPaw} /> Our Service
                </Link>
                <div className="banner_services_wrap row">
                  <div className="col">
                    <div className="iconbox_item iconbox_overicon">
                      <div className="item_icon">
                        <svg
                          width={31}
                          height={30}
                          viewBox="0 0 31 30"
                          fill="none"
                          xmlns="http://www.w3.org/2000/svg"
                        >
                          <path
                            d="M17.291 10.3611C16.1191 10.3611 11.0215 10.4783 7.79883 15.4001V10.3611C7.79883 7.31421 5.2207 4.73608 2.17383 4.73608C1.11914 4.73608 0.298828 5.61499 0.298828 6.61108C0.298828 7.66577 1.11914 8.48608 2.17383 8.48608C3.16992 8.48608 4.04883 9.36499 4.04883 10.3611V25.3611C4.04883 27.4705 5.68945 29.1111 7.79883 29.1111H18.1113C18.5801 29.1111 19.0488 28.7009 19.0488 28.1736V27.2361C19.0488 26.24 18.1699 25.3611 17.1738 25.3611H15.2988L22.7988 19.7361V28.1736C22.7988 28.7009 23.209 29.1111 23.7363 29.1111H25.6113C26.0801 29.1111 26.5488 28.7009 26.5488 28.1736V16.1033C25.9043 16.2791 25.3184 16.3962 24.6738 16.3962C21.041 16.3962 17.9941 13.8181 17.291 10.3611ZM26.5488 4.73608H22.7988L19.0488 0.986084V8.89624C19.0488 12.0017 21.5098 14.5212 24.6738 14.5212C27.7793 14.5212 30.2988 12.0017 30.2988 8.89624V0.986084L26.5488 4.73608ZM22.3301 9.42358C21.8027 9.42358 21.3926 9.01343 21.3926 8.48608C21.3926 8.01733 21.8027 7.54858 22.3301 7.54858C22.7988 7.54858 23.2676 8.01733 23.2676 8.48608C23.2676 9.01343 22.7988 9.42358 22.3301 9.42358ZM27.0176 9.42358C26.4902 9.42358 26.0801 9.01343 26.0801 8.48608C26.0801 8.01733 26.4902 7.54858 27.0176 7.54858C27.4863 7.54858 27.9551 8.01733 27.9551 8.48608C27.9551 9.01343 27.4863 9.42358 27.0176 9.42358Z"
                            fill="#7C58D3"
                          />
                        </svg>
                      </div>
                      <div className="item_content">
                        <h3 className="item_title mb-0">Vet services</h3>
                      </div>
                      <Link className="item_global_link" to="/service" />
                    </div>
                  </div>
                  <div className="col">
                    <div className="iconbox_item iconbox_overicon">
                      <div className="item_icon">
                        <svg
                          width={39}
                          height={20}
                          viewBox="0 0 39 20"
                          fill="none"
                          xmlns="http://www.w3.org/2000/svg"
                        >
                          <path
                            d="M35.6465 9.46655C35.373 9.58374 35.2363 9.79858 35.2363 10.1111C35.2363 10.4236 35.373 10.6384 35.6465 10.7556C36.3887 11.1462 36.9746 11.6931 37.4043 12.3962C37.834 13.0994 38.0488 13.8611 38.0488 14.6814V15.1501C38.0488 16.322 37.6191 17.3376 36.7598 18.197C35.9004 19.0564 34.8848 19.4861 33.7129 19.4861C32.7754 19.4861 31.916 19.2126 31.1348 18.6658C30.3926 18.1189 29.8652 17.3962 29.5527 16.4978C29.5527 16.4587 29.4941 16.283 29.377 15.9705C29.2598 15.6189 29.1816 15.4041 29.1426 15.3259C29.1035 15.2087 29.0254 15.033 28.9082 14.7986C28.791 14.5251 28.6543 14.3494 28.498 14.2712C28.3809 14.1931 28.2246 14.115 28.0293 14.0369C27.834 13.9197 27.5996 13.8611 27.3262 13.8611H11.2715C11.0762 13.8611 10.9004 13.8806 10.7441 13.9197C10.5879 13.9587 10.4512 14.0173 10.334 14.0955C10.2168 14.1345 10.0996 14.2322 9.98242 14.3884C9.9043 14.5056 9.82617 14.6033 9.74805 14.6814C9.66992 14.7595 9.5918 14.8962 9.51367 15.0916C9.43555 15.2869 9.37695 15.4431 9.33789 15.5603C9.33789 15.6384 9.29883 15.7947 9.2207 16.0291C9.14258 16.2634 9.08398 16.4197 9.04492 16.4978C8.73242 17.3962 8.18555 18.1189 7.4043 18.6658C6.66211 19.2126 5.82227 19.4861 4.88477 19.4861C3.71289 19.4861 2.69727 19.0564 1.83789 18.197C0.978516 17.3376 0.548828 16.322 0.548828 15.1501V14.6814C0.548828 13.8611 0.763672 13.0994 1.19336 12.3962C1.62305 11.6931 2.20898 11.1462 2.95117 10.7556C3.22461 10.6384 3.36133 10.4236 3.36133 10.1111C3.36133 9.79858 3.22461 9.58374 2.95117 9.46655C2.20898 9.07593 1.62305 8.52905 1.19336 7.82593C0.763672 7.1228 0.548828 6.36108 0.548828 5.54077V5.07202C0.548828 3.90015 0.978516 2.88452 1.83789 2.02515C2.69727 1.16577 3.71289 0.736084 4.88477 0.736084C5.50977 0.736084 6.0957 0.872803 6.64258 1.14624C7.22852 1.38062 7.7168 1.73218 8.10742 2.20093C8.53711 2.63062 8.84961 3.13843 9.04492 3.72437C9.04492 3.76343 9.10352 3.95874 9.2207 4.3103C9.33789 4.6228 9.41602 4.83765 9.45508 4.95483C9.49414 5.03296 9.57227 5.20874 9.68945 5.48218C9.80664 5.71655 9.92383 5.8728 10.041 5.95093C10.1973 6.02905 10.373 6.12671 10.5684 6.2439C10.7637 6.32202 10.998 6.36108 11.2715 6.36108H27.3262C27.5215 6.36108 27.6973 6.34155 27.8535 6.30249C28.0488 6.26343 28.2051 6.1853 28.3223 6.06812C28.4785 5.95093 28.5957 5.85327 28.6738 5.77515C28.791 5.69702 28.8887 5.54077 28.9668 5.3064C29.084 5.07202 29.1426 4.91577 29.1426 4.83765C29.1816 4.75952 29.2598 4.56421 29.377 4.25171C29.4941 3.93921 29.5527 3.76343 29.5527 3.72437C29.8652 2.82593 30.3926 2.10327 31.1348 1.5564C31.916 1.00952 32.7754 0.736084 33.7129 0.736084C34.8848 0.736084 35.9004 1.16577 36.7598 2.02515C37.6191 2.88452 38.0488 3.90015 38.0488 5.07202V5.54077C38.0488 6.36108 37.834 7.1228 37.4043 7.82593C36.9746 8.52905 36.3887 9.07593 35.6465 9.46655Z"
                            fill="#7C58D3"
                          />
                        </svg>
                      </div>
                      <div className="item_content">
                        <h3 className="item_title mb-0">Special care</h3>
                      </div>
                      <Link className="item_global_link" to="/service" />
                    </div>
                  </div>
                  <div className="col">
                    <div className="iconbox_item iconbox_overicon">
                      <div className="item_icon">
                        <svg
                          width={35}
                          height={32}
                          viewBox="0 0 35 32"
                          fill="none"
                          xmlns="http://www.w3.org/2000/svg"
                        >
                          <path
                            d="M16.5371 15.7595L10.1504 9.07983C9.25195 8.14233 8.82227 6.97046 8.86133 5.56421C8.93945 4.15796 9.48633 3.02515 10.502 2.16577C11.4004 1.38452 12.4355 1.05249 13.6074 1.16968C14.8184 1.2478 15.834 1.71655 16.6543 2.57593L17.2988 3.27905L17.9434 2.57593C18.8027 1.71655 19.8184 1.2478 20.9902 1.16968C22.1621 1.05249 23.1973 1.38452 24.0957 2.16577C25.1504 3.02515 25.6973 4.15796 25.7363 5.56421C25.8145 6.97046 25.3848 8.14233 24.4473 9.07983L18.0605 15.7595C17.8262 15.9939 17.5723 16.1111 17.2988 16.1111C17.0254 16.1111 16.7715 15.9939 16.5371 15.7595ZM33.5293 20.3298C33.998 20.7205 34.2129 21.2087 34.1738 21.7947C34.1738 22.3806 33.9395 22.8494 33.4707 23.2009L24.623 30.2908C23.9199 30.8376 23.1387 31.1111 22.2793 31.1111H1.36133C1.08789 31.1111 0.853516 31.0134 0.658203 30.8181C0.501953 30.6619 0.423828 30.447 0.423828 30.1736V24.5486C0.423828 24.2751 0.501953 24.0603 0.658203 23.9041C0.853516 23.7087 1.08789 23.6111 1.36133 23.6111H4.58398L7.33789 21.3845C8.58789 20.3689 10.0332 19.8611 11.6738 19.8611H21.0488C21.6348 19.8611 22.1035 20.0955 22.4551 20.5642C22.8457 20.9939 23.002 21.4822 22.9238 22.0291C22.8457 22.4978 22.6113 22.8884 22.2207 23.2009C21.8301 23.4744 21.4004 23.6111 20.9316 23.6111H16.3613C16.0879 23.6111 15.8535 23.7087 15.6582 23.9041C15.502 24.0603 15.4238 24.2751 15.4238 24.5486C15.4238 24.822 15.502 25.0564 15.6582 25.2517C15.8535 25.408 16.0879 25.4861 16.3613 25.4861H23.2754C24.1738 25.4861 24.9551 25.2126 25.6191 24.6658L31.0684 20.3298C31.4199 20.0173 31.8301 19.8611 32.2988 19.8611C32.7676 19.8611 33.1777 20.0173 33.5293 20.3298Z"
                            fill="#7C58D3"
                          />
                        </svg>
                      </div>
                      <div className="item_content">
                        <h3 className="item_title mb-0">High quality</h3>
                      </div>
                      <Link className="item_global_link" to="/service" />
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div className="decoration_item shape_1">
              <svg
                width={385}
                height={384}
                viewBox="0 0 385 384"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <circle
                  cx="192.243"
                  cy={192}
                  r={190}
                  stroke="url(#shape_circle_1)"
                  strokeWidth={4}
                  strokeLinecap="round"
                  strokeDasharray="107 107"
                />
                <circle cx="192.243" cy={192} r={170} fill="#FFDA47" />
                <defs>
                  <linearGradient
                    id="shape_circle_1"
                    x1="-30.159"
                    y1="-153.951"
                    x2="-30.159"
                    y2="290.853"
                    gradientUnits="userSpaceOnUse"
                  >
                    <stop offset={1} stopColor="#FFDA47" />
                    <stop offset={1} stopColor="#FFDA47" stopOpacity="0.01" />
                  </linearGradient>
                </defs>
              </svg>
            </div>
            <div className="decoration_item shape_2">
              <svg
                width={385}
                height={384}
                viewBox="0 0 385 384"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <circle
                  cx="192.243"
                  cy={192}
                  r={190}
                  stroke="url(#shape_circle_2)"
                  strokeWidth={4}
                  strokeLinecap="round"
                  strokeDasharray="107 107"
                />
                <circle cx="192.243" cy={192} r={170} fill="#FF47A2" />
                <defs>
                  <linearGradient
                    id="shape_circle_2"
                    x1="-30.159"
                    y1="-153.951"
                    x2="-30.159"
                    y2="290.853"
                    gradientUnits="userSpaceOnUse"
                  >
                    <stop offset={1} stopColor="#FF7DCF" />
                    <stop offset={1} stopColor="#FF47A2" stopOpacity="0.01" />
                  </linearGradient>
                </defs>
              </svg>
            </div>
            <div className="decoration_item chat_img_1">
              <img
                src="src/assets/images/banner/cat_img_1.png"
                alt="Cat Image"
              />
            </div>
            <div className="decoration_item chat_img_2">
              <img
                src="src/assets/images/banner/cat_img_2.png"
                alt="Cat Image"
              />
            </div>
          </section>

          <section className="pricing_section bg_gray section_space_lg decoration_wrap">
            <div className="container">
              <div className="section_title text-center">
                <h2 className="title_text">
                  <span className="sub_title">Our combo</span>  
                </h2>
              </div>
              <div className="pricing_tab_wrap">
                <div className="tab-content">
                  <div
                    className="tab-pane fade show active"
                    id="tab_small_dog"
                    role="tabpanel"
                  >
                    {/* Combo */}
                    <div className=" flex items-start gap-x-3 justify-center ">
                      {comboList.map((combo, index) => (
                        <ComboCard combo={combo} key={index} />
                      ))}
                    </div>
                  </div>

                </div>
              </div>
            </div>
            <div className="decoration_item shape_dot_1">
              <img
                src="src/assets/images/shape/shape_dot_group_3.svg"
                alt="Colorful Dots"
              />
            </div>
            <div className="decoration_item shape_dot_2">
              <img
                src="src/assets/images/shape/shape_dot_group_4.svg"
                alt="Colorful Dots"
              />
            </div>
          </section>

          <section className="team_section section_space_lg">
            <div className="container">
              <div className="section_title text-center">
                <h2 className="title_text mb-0">
                  <span className="sub_title">Staff Team</span>
                  Welcome To Our Family
                </h2>
              </div>
              <div className="row justify-content-center">
                {staffList.map((staff, index) => (
                  <div className="col col-lg-3 col-md-6 col-sm-6" key={index}>
                    <div className="team_item text-center">
                      <div className="item_image">
                        <img
                          src="src/assets/images/team/team_img_1.jpg"
                          alt="Team Image"
                        />
                      </div>
                      <div className="item_content">
                        <h3 className="item_title_2">{staff.fullName}</h3>
                        <span className="item_designation">
                          {staff.user.phoneNumber}
                        </span>
                        <ul className="social_links unorder_list">
                          <li>
                            <a
                              href="https://www.facebook.com"
                              target="_blank"
                              rel="noopener noreferrer"
                            >
                              <FontAwesomeIcon icon={faFacebook} />{" "}
                            </a>
                          </li>
                          <li>
                            <a
                              href="https://www.instagram.com"
                              target="_blank"
                              rel="noopener noreferrer"
                            >
                              <FontAwesomeIcon icon={faInstagram} />{" "}
                            </a>
                          </li>
                        </ul>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
              {/* <div className="text-center">
                <Link className="btn btn_primary" to="/team">
                  <FontAwesomeIcon icon={faPaw} /> Our Team
                </Link>
              </div> */}
            </div>
          </section>
          <section className="contact_section bg_gray section_space_lg decoration_wrap">
            <div className="container">
              <div className="section_title text-center">
                <h2 className="title_text">
                  <span className="sub_title">Our Contacts</span>
                  Contacts
                </h2>
              </div>
              <div className="row justify-content-center">
                <div className="col col-lg-3 col-md-6 col-sm-6">
                  <div className="iconbox_item iconbox_overicon">
                    <div className="item_icon">
                      <FontAwesomeIcon icon={faPhoneVolume} />{" "}
                    </div>
                    <div className="item_content">
                      <h3 className="item_title">Phone</h3>
                      <ul className="item_info_list unorder_list_block">
                        <li>0123.456.789</li>
                      </ul>
                    </div>
                  </div>
                </div>
                <div className="col col-lg-3 col-md-6 col-sm-6">
                  <div className="iconbox_item iconbox_overicon">
                    <div className="item_icon">
                      <FontAwesomeIcon icon={faEnvelope} />{" "}
                    </div>
                    <div className="item_content">
                      <h3 className="item_title">Email</h3>
                      <ul className="item_info_list unorder_list_block">
                        <li>
                          <a
                            href="/cdn-cgi/l/email-protection"
                            className="__cf_email__"
                            data-cfemail="4e272028210e2f2a3d2321603820"
                          >
                            petspaswp391@gmail.com
                          </a>
                        </li>
                      </ul>
                    </div>
                  </div>
                </div>
                <div className="col col-lg-3 col-md-6 col-sm-6">
                  <div className="iconbox_item iconbox_overicon">
                    <div className="item_icon">
                      <FontAwesomeIcon icon={faLocationDot} />{" "}
                    </div>
                    <div className="item_content">
                      <h3 className="item_title">Address</h3>
                      <ul className="item_info_list unorder_list_block">
                        <li>123 Le Van Viet</li>
                      </ul>
                    </div>
                  </div>
                </div>
                <div className="col col-lg-3 col-md-6 col-sm-6">
                  <div className="iconbox_item iconbox_overicon">
                    <div className="item_icon">
                      <FontAwesomeIcon icon={faClock} />{" "}
                    </div>
                    <div className="item_content">
                      <h3 className="item_title">Open Hours</h3>
                      <ul className="item_info_list unorder_list_block">
                        <li>Mon - Sat: 8am - 8pm</li>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div className="decoration_item shape_image_1">
              <img
                src="src/assets/images/shape/shape_purr.svg"
                alt="Pet Purr"
              />
            </div>
            <div className="decoration_item shape_image_2">
              <img src="src/assets/images/shape/shape_cat.svg" alt="Cat" />
            </div>
          </section>
        </main>
      </div>
    </>
  );
}

export default HomePage;
