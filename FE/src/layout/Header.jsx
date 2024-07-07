import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faBars,
  faCircleUser,
  faPaw,
  faCartShopping,
  faArrowRightToBracket,
  faArrowRightFromBracket,
  faClockRotateLeft,
} from "@fortawesome/free-solid-svg-icons";
import { Link } from "react-router-dom";
import { useEffect, useState } from "react";

const Header = () => {
  const [checkLogin, setCheckLogin] = useState(false);
  const [userRole, setUserRole] = useState("");

  const checkUserLogin = () => {
    const user = JSON.parse(localStorage.getItem("user-info"));
    if (user) {
      setCheckLogin(true);
      setUserRole(user.data.user.role);
    } else {
      setCheckLogin(false);
      setUserRole("");
    }
  };

  useEffect(() => {
    checkUserLogin(); // Initial check when component mounts

    const handleStorageChange = () => {
      checkUserLogin(); // Check login status on localStorage change
    };

    const intervalId = setInterval(checkUserLogin, 1000); // Periodic check every second

    window.addEventListener("storage", handleStorageChange);

    return () => {
      window.removeEventListener("storage", handleStorageChange);
      clearInterval(intervalId); // Clear the interval when component unmounts
    };
  }, []);

  return (
    <section className="w-full flex justify-between p-5 border-b-2 lg:px-10 lg:py-5">
      <header className="header_section header_boxed">
        <div className="container">
          <link rel="stylesheet" href="src/assets/css/all.css" />
          <div className="box_wrap d-flex align-items-center justify-content-between">
            <div className="site_logo">
                <img
                  className="logo_before"
                  src="src/assets/images/logo/logo.png"
                  alt="Petpia Logo"
                />
            </div>
            <nav className="main_menu navbar navbar-expand-lg">
              <div id="main_menu_dropdown">
                <ul className="main_menu_list unorder_list_center">
                  {userRole !== "Admin" && userRole !== "Manager" && userRole !== "Staff" && (
                    <>
                      <li>
                        <Link to="/HomePage">Home</Link>
                      </li>
                      <li>
                        <Link to="/service">Service</Link>
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
                    </>
                  )}
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
                    {checkLogin && userRole === "Customer" && (
                      <>
                        <li>
                          <Link className="item_image" to="/account"></Link>
                          <div className="item_content">
                            <h3 className="item_title ">
                              <FontAwesomeIcon icon={faCircleUser} />{" "}
                              <Link to="/account">Account</Link>
                            </h3>
                          </div>
                        </li>
                        <li>
                          <Link className="item_image" to="/account"></Link>
                          <div className="item_content">
                            <h3 className="item_title ">
                              <FontAwesomeIcon icon={faPaw} />{" "}
                              <Link to="/Pet">Pet</Link>
                            </h3>
                          </div>
                        </li>
                        <li>
                          <Link className="item_image" to="/account"></Link>
                          <div className="item_content">
                            <h3 className="item_title ">
                              <FontAwesomeIcon icon={faCartShopping} />{" "}
                              <Link to="/Cart">Cart</Link>
                            </h3>
                          </div>
                        </li>
                        <li>
                          <Link className="item_image" to="/account"></Link>
                          <div className="item_content">
                            <h3 className="item_title ">
                              <FontAwesomeIcon icon={faClockRotateLeft} />{" "}
                              <Link to="/transac">Orders</Link>
                            </h3>
                          </div>
                        </li>
                      </>
                    )}
                    {!checkLogin && (
                      <li>
                        <Link className="item_image" to="/account"></Link>
                        <div className="item_content">
                          <h3 className="item_title">
                            <FontAwesomeIcon icon={faArrowRightToBracket} />{" "}
                            <Link to="/Login">Log in</Link>
                          </h3>
                        </div>
                      </li>
                    )}
                    {checkLogin && (
                      <li>
                        <Link className="item_image" to="/account"></Link>
                        <div className="item_content">
                          <h3 className="item_title ">
                            <FontAwesomeIcon icon={faArrowRightFromBracket} />{" "}
                            <Link to="/Logout">Log out</Link>
                          </h3>
                        </div>
                      </li>
                    )}
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
