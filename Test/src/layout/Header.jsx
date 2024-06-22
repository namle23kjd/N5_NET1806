// import { List, PawPrint } from "lucide-react";
// import {
//   DropdownMenu,
//   DropdownMenuContent,
//   DropdownMenuItem,
//   DropdownMenuLabel,
//   DropdownMenuSeparator,
//   DropdownMenuTrigger,
// } from "@/components/ui/dropdown-menu";
// import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBars } from "@fortawesome/free-solid-svg-icons";
import { faCircleUser } from "@fortawesome/free-solid-svg-icons";
import { faPaw } from "@fortawesome/free-solid-svg-icons";
import { faCartShopping } from "@fortawesome/free-solid-svg-icons";
import { faArrowRightToBracket } from "@fortawesome/free-solid-svg-icons";
import { faArrowRightFromBracket } from "@fortawesome/free-solid-svg-icons";
import { Link } from "react-router-dom";

const Header = () => {
  return (
    <section className="w-full flex justify-between p-5 border-b-2 lg:px-10 lg:py-5">
      <header className="header_section header_boxed">
        <div className="container">
          <link rel="stylesheet" href="src/assets/css/all.css" />
          <div className="box_wrap d-flex align-items-center justify-content-between">
            <div className="site_logo">
              <Link to="/HomePage">
                <img
                  className="logo_before"
                  src="src/assets/images/logo/logo.png"
                  alt="Petpia Logo"
                />
              </Link>
            </div>
            <nav className="main_menu navbar navbar-expand-lg">
              <div id="main_menu_dropdown">
                <ul className="main_menu_list unorder_list_center">
                  <li>
                    <Link to="/HomePage">Home</Link>
                  </li>
                  {/* <li class="active dropdown">
                                  <a class="nav-link" href="#" id="home_submenu" role="button" data-bs-toggle="dropdown" aria-expanded="false">Home</a>
                                  <ul class="dropdown-menu" aria-labelledby="home_submenu">
                                      <li class="active">
                                          <a href="index_1">Home V.1</a>
                                      </li>
                                      <li>
                                          <a href="index_2">Home V.2</a>
                                      </li>
                                  </ul>
                              </li> */}
                  <li className="dropdown">
                    <a
                      className="nav-link"
                      href="#"
                      id="service_submenu"
                      role="button"
                      data-bs-toggle="dropdown"
                      aria-expanded="false"
                    >
                      Services
                    </a>
                    <ul
                      className="dropdown-menu"
                      aria-labelledby="service_submenu"
                    >
                      <li>
                      <Link to="/service">Our Service</Link>
                      </li>
                      <li>
                      <Link to="/service_details">Service Details</Link>
                      </li>
                    </ul>
                  </li>
                  <li className="dropdown">
                    <a
                      className="nav-link"
                      href="#"
                      id="blog_submenu"
                      role="button"
                      data-bs-toggle="dropdown"
                      aria-expanded="false"
                    >
                      Blog
                    </a>
                    <ul
                      className="dropdown-menu"
                      aria-labelledby="blog_submenu"
                    >
                      <li>
                        <Link to="/blog">Our Blogs</Link>
                      </li>
                      <li>
                        <Link to="/post/:id">Blog Details</Link>
                      </li>
                    </ul>
                  </li>
                    {/* <li className="dropdown">
                      <a
                        className="nav-link"
                        href="#"
                        id="pages_submenu"
                        role="button"
                        data-bs-toggle="dropdown"
                        aria-expanded="false"
                      >
                        Pages
                      </a>
                      <ul
                        className="dropdown-menu"
                        aria-labelledby="pages_submenu"
                      >
                        <li>
                          <a href="about">About Us</a>
                        </li>
                        <li>
                          <a href="contact">Contact Us</a>
                        </li>
                        <li>
                          <a href="faq">FAQ</a>
                        </li>
                        <li>
                          <a href="error">Error 404</a>
                        </li>
                        <li>
                          <a href="team">Our Team</a>
                        </li>
                      </ul>
                    </li> */}
                </ul>
              </div>
            </nav>
            <ul className="unorder_list_right">
              <li className="dropdown">
                <button
                  className="cart_btn"
                  type="button"
                  id="cart_dropdown"
                  data-bs-toggle="dropdown"
                  aria-expanded="false"
                >
                  <FontAwesomeIcon icon={faBars} />{" "}
                </button>
                <div className="dropdown-menu" aria-labelledby="cart_dropdown">
                  <ul className="cart_items_list unorder_list_block">
                    <li>
                      <a className="item_image" href="account"></a>
                      <div className="item_content">
                        <h3 className="item_title ">
                          <FontAwesomeIcon icon={faCircleUser} />{" "}
                          <Link to="/account">Account</Link>
                        </h3>
                      </div>
                    </li>
                    <li>
                      <a className="item_image" href="account"></a>
                      <div className="item_content">
                        <h3 className="item_title ">
                          <FontAwesomeIcon icon={faPaw} /> <Link to="/Pet">Pet</Link>
                        </h3>
                      </div>
                    </li>
                    <li>
                      <a className="item_image" href="account"></a>
                      <div className="item_content">
                        <h3 className="item_title ">
                          <FontAwesomeIcon icon={faCartShopping} />{" "}
                          <Link to="/Cart">Cart</Link>
                        </h3>
                      </div>
                    </li>
                    <li>
                      <a className="item_image" href="account"></a>
                      <div className="item_content">
                        <h3 className="item_title ">
                          <FontAwesomeIcon icon={faArrowRightToBracket} />{" "}
                          <Link to="/Login">Log in</Link>
                        </h3>
                      </div>
                    </li>
                    <li>
                      <a className="item_image" href="account"></a>
                      <div className="item_content">
                        <h3 className="item_title ">
                          <FontAwesomeIcon icon={faArrowRightFromBracket} />{" "}
                          <Link to="/Logout">Log out</Link>
                        </h3>
                      </div>
                    </li>
                  </ul>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </header>
    </section>
  );
};

export default Header;
