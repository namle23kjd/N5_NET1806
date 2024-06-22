// import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
// import {
//   faArrowLeft,
//   faArrowUp,
//   faQuoteRight,
// } from "@fortawesome/free-solid-svg-icons";
// import { faArrowRight } from "@fortawesome/free-solid-svg-icons";
// import "../../assets/js/email-decode.min.js";
// import "../../assets/js/jquery.min.js";
// import "../../assets/js/popper.min.js";
// import "../../assets/js/bootstrap.min.js";
// import "../../assets/js/bootstrap-dropdown-ml-hack.js";
// import "../../assets/js/slick.min.js";
// import "../../assets/js/magnific-popup.min.js";
// import "../../assets/js/nice-select.min.js";
// import "../../assets/js/jquery-ui.js";
// import "../../assets/js/vanilla-calendar.min.js";
// import "../../assets/js/countdown.js";
// import "../../assets/js/main.js";
// import { faClock, faStar } from "@fortawesome/free-regular-svg-icons";
// import "slick-carousel/slick/slick.css";
// import "slick-carousel/slick/slick-theme.css";
// import Slider from "react-slick";
// import PropTypes from "prop-types";
// import { Link } from "react-router-dom";

// function Service_Details() {
//   const CustomNextArrow = ({ onClick }) => (
//     <button type="button" className="arrow-button top-right" onClick={onClick}>
//       <FontAwesomeIcon icon={faArrowRight} />
//     </button>
//   );

//   // CustomPrevArrow component
//   const CustomPrevArrow = ({ onClick }) => (
//     <button type="button" className="arrow-button top-left" onClick={onClick}>
//       <FontAwesomeIcon icon={faArrowLeft} />
//     </button>
//   );
//   CustomPrevArrow.propTypes = {
//     onClick: PropTypes.func.isRequired,
//   };

//   CustomNextArrow.propTypes = {
//     onClick: PropTypes.func.isRequired,
//   };

//   const settings = {
//     dots: false,
//     centerMode: true,
//     slidesToShow: 3,
//     slidesToScroll: 1,
//     infinite: true,
//     centerPadding: "0",
//     prevArrow: <CustomPrevArrow />,
//     nextArrow: <CustomNextArrow />,
//   };
//   return (
//     <div>
//       <div>
//         <meta charSet="utf-8" />
//         <meta
//           name="viewport"
//           content="width=device-width,initial-scale=1,shrink-to-fit=no"
//         />
//         <meta httpEquiv="x-ua-compatible" content="ie=edge" />
//         <title>Service Details - Petpia – Pet Care Service Template</title>
//         <link
//           rel="shortcut icon"
//           href="src/assets/images/logo/favourite_icon.png"
//         />
//         <link rel="stylesheet" href="src/assets/css/all.css" />
//         <div className="body_wrap">
//           <div className="backtotop">
//             <a href="#" className="scroll">
//             <FontAwesomeIcon icon={faArrowUp} />{" "}
//             </a>
//           </div>
//           <main>
//             <section className="breadcrumb_section">
//               <div className="container">
//                 <div className="row">
//                   <div className="col col-lg-4 col-md-7 col-sm-9">
//                     <ul className="breadcrumb_nav">
//                       <li>
//                             <Link to="/Homepage">Home</Link>
//                       </li>
//                       <li>Single</li>
//                     </ul>
//                     <h1 className="page_title">Pet Sitting</h1>
//                     <p className="page_description mb-0">
//                       Feugiat scelerisque varius morbi enim nunc faucibus.
//                       Imperdiet dui accumsan sit amet nulla facilisi morbi
//                     </p>
//                   </div>
//                 </div>
//               </div>
//               <div className="breadcrumb_img">
//                 <img
//                   src="src/assets/images/breadcrumb/breadcrumb_img_3.png"
//                   alt="Pet Care Service"
//                 />
//               </div>
//             </section>
//             <section
//               className="service_section bg_gray section_space_lg"
//               style={{
//                 backgroundImage:
//                   'url("src/assets/images/shape/shape_paws_bg_2.svg")',
//               }}
//             >
//               <div className="container">
//                 <div className="section_title text-center mb-2">
//                   <div className="row justify-content-center">
//                     <div className="col col-lg-5">
//                       <h2 className="title_text">
//                         <span className="sub_title">Our Prices</span> Pet
//                         Services + Rates
//                       </h2>
//                       <p className="mb-0">
//                         We can fully customize your pet sitting schedule to fit
//                         your pet’s needs. Pick and choose what visits work best
//                         for you and your family
//                       </p>
//                     </div>
//                   </div>
//                 </div>
//                 <div className="row">
//                   <div className="col col-lg-6">
//                     <div className="services_price_items_wrap">
//                       <div className="service_price_item">
//                         <div className="item_image">
//                           <img
//                             src="src/assets/images/service/service_img_1.jpg"
//                             alt="Pet Care Service"
//                           />
//                         </div>
//                         <div className="item_content">
//                           <div className="d-flex justify-content-between align-items-center">
//                             <div className="service_time">
//                               <FontAwesomeIcon icon={faClock} /> 15 minute visit
//                             </div>
//                             <div className="item_price">
//                               <span>$22.00</span>
//                             </div>
//                           </div>
//                           <h3 className="item_title mb-0">
//                             1 x Visit per day, small pet visit can be added per
//                             quote
//                           </h3>
//                         </div>
//                         <Link
//                           className="item_global_link"
//                           to="/service_details"
//                         />
//                       </div>
//                       <div className="service_price_item">
//                         <div className="item_image">
//                           <img
//                             src="src/assets/images/service/service_img_2.jpg"
//                             alt="Pet Care Service"
//                           />
//                         </div>
//                         <div className="item_content">
//                           <div className="d-flex justify-content-between align-items-center">
//                             <div className="service_time">
//                               <FontAwesomeIcon icon={faClock} /> 30 minute visit
//                             </div>
//                             <div className="item_price">
//                               <span>$29.00</span>
//                             </div>
//                           </div>
//                           <h3 className="item_title mb-0">
//                             A 12-hour stay, including the evening visit and
//                             morning visit
//                           </h3>
//                         </div>
//                         <Link
//                           className="item_global_link"
//                           to="/service_details"
//                         />
//                       </div>
//                       <div className="service_price_item">
//                         <div className="item_image">
//                           <img
//                             src="src/assets/images/service/service_img_3.jpg"
//                             alt="Pet Care Service"
//                           />
//                         </div>
//                         <div className="item_content">
//                           <div className="d-flex justify-content-between align-items-center">
//                             <div className="service_time">
//                               <FontAwesomeIcon icon={faClock} /> 45 minute visit
//                             </div>
//                             <div className="item_price">
//                               <span>$36.00</span>
//                             </div>
//                           </div>
//                           <h3 className="item_title mb-0">
//                             Drop-off and pick-up times are flexible. $10 each
//                             additional dog.
//                           </h3>
//                         </div>
//                         <Link
//                           className="item_global_link"
//                           to="/service_details"
//                         />
//                       </div>
//                     </div>
//                   </div>
//                   <div className="col col-lg-6">
//                     <div className="services_price_items_wrap">
//                       <div className="service_price_item">
//                         <div className="item_image">
//                           <img
//                             src="src/assets/images/service/service_img_4.jpg"
//                             alt="Pet Care Service"
//                           />
//                         </div>
//                         <div className="item_content">
//                           <div className="d-flex justify-content-between align-items-center">
//                             <div className="service_time">
//                               <FontAwesomeIcon icon={faClock} /> 29 minute visit
//                             </div>
//                             <div className="item_price">
//                               <span>$22.00</span>
//                             </div>
//                           </div>
//                           <h3 className="item_title mb-0">
//                             1 x Visit per day, small pet visit can be added per
//                             quote
//                           </h3>
//                         </div>
//                         <link
//                           className="item_global_link"
//                           to="/service_details"
//                         />
//                       </div>
//                       <div className="service_price_item">
//                         <div className="item_image">
//                           <img
//                             src="src/assets/images/service/service_img_5.jpg"
//                             alt="Pet Care Service"
//                           />
//                         </div>
//                         <div className="item_content">
//                           <div className="d-flex justify-content-between align-items-center">
//                             <div className="service_time">
//                               <FontAwesomeIcon icon={faClock} /> 65 Overnight
//                               Pet Sitting
//                             </div>
//                             <div className="item_price">
//                               <span>$29.00</span>
//                             </div>
//                           </div>
//                           <h3 className="item_title mb-0">
//                             A 12-hour stay, including the evening visit and
//                             morning visit
//                           </h3>
//                         </div>
//                         <Link
//                           className="item_global_link"
//                           to="/service_details"
//                         />
//                       </div>
//                       <div className="service_price_item">
//                         <div className="item_image">
//                           <img
//                             src="src/assets/images/service/service_img_6.jpg"
//                             alt="Pet Care Service"
//                           />
//                         </div>
//                         <div className="item_content">
//                           <div className="d-flex justify-content-between align-items-center">
//                             <div className="service_time">
//                               <FontAwesomeIcon icon={faClock} /> 40 Private
//                               Boarding
//                             </div>
//                             <div className="item_price">
//                               <span>$36.00</span>
//                             </div>
//                           </div>
//                           <h3 className="item_title mb-0">
//                             Drop-off and pick-up times are flexible. $10 each
//                             additional dog.
//                           </h3>
//                         </div>
//                         <Link
//                           className="item_global_link"
//                           to="/service_details"
//                         />
//                       </div>
//                     </div>
//                   </div>
//                 </div>
//               </div>
//             </section>
//             <section className="testimonial_section bg_gray section_space_lg">
//               <div className="container">
//                 <div className="section_title">
//                   <h2 className="title_text mb-0">
//                     <span className="sub_title">Our Reviews</span> What People
//                     Say
//                   </h2>
//                 </div>
//               </div>
//               <div className="testimonial_carousel">
//                 <Slider {...settings}>
//                   <div className="col carousel_item">
//                     <div className="testimonial_item">
//                       <div className="testimonial_admin">
//                         <div className="admin_thumbnail">
//                           <img
//                             src="src/assets/images/meta/thumbnail_img_1.png"
//                             alt="Pet Thumbnail Image"
//                           />
//                         </div>
//                         <div className="admin_info">
//                           <h4 className="admin_name">Home Visits</h4>
//                           <span className="admin_designation">
//                             Lucas Simões
//                           </span>
//                         </div>
//                       </div>
//                       <ul className="rating_star">
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                       </ul>
//                       <p className="mb-0">
//                         Tristique nulla aliquet enim tortor at auctor urna nunc.
//                         Massa enim nec dui nunc mattis enim ut tellus
//                       </p>
//                       <span className="quote_icon">
//                         <FontAwesomeIcon icon={faQuoteRight} />{" "}
//                       </span>
//                     </div>
//                   </div>

//                   <div className="col carousel_item">
//                     <div className="testimonial_item">
//                       <div className="testimonial_admin">
//                         <div className="admin_thumbnail">
//                           <img
//                             src="src/assets/images/meta/thumbnail_img_2.png"
//                             alt="Pet Thumbnail Image"
//                           />
//                         </div>
//                         <div className="admin_info">
//                           <h4 className="admin_name">Dog Boarding</h4>
//                           <span className="admin_designation">
//                             Wilhelm Dowall
//                           </span>
//                         </div>
//                       </div>
//                       <ul className="rating_star">
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                       </ul>
//                       <p className="mb-0">
//                         Lectus magna fringilla urna porttitor rhoncus dolor
//                         purus non enim. Tellus in hac habitasse platea dictumst
//                       </p>
//                       <span className="quote_icon">
//                         <FontAwesomeIcon icon={faQuoteRight} />{" "}
//                       </span>
//                     </div>
//                   </div>
//                   <div className="col carousel_item">
//                     <div className="testimonial_item">
//                       <div className="testimonial_admin">
//                         <div className="admin_thumbnail">
//                           <img
//                             src="src/assets/images/meta/thumbnail_img_3.png"
//                             alt="Pet Thumbnail Image"
//                           />
//                         </div>
//                         <div className="admin_info">
//                           <h4 className="admin_name">Pet Training</h4>
//                           <span className="admin_designation">
//                             Lara Madrigal
//                           </span>
//                         </div>
//                       </div>
//                       <ul className="rating_star">
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                       </ul>
//                       <p className="mb-0">
//                         Ut tortor pretium viverra suspendisse potenti nullam.
//                         Venenatis urna cursus eget nunc scelerisque viverra
//                         mauris in aliquam
//                       </p>
//                       <span className="quote_icon">
//                         <FontAwesomeIcon icon={faQuoteRight} />{" "}
//                       </span>
//                     </div>
//                   </div>
//                   <div className="col carousel_item">
//                     <div className="testimonial_item">
//                       <div className="testimonial_admin">
//                         <div className="admin_thumbnail">
//                           <img
//                             src="src/assets/images/meta/thumbnail_img_4.png"
//                             alt="Pet Thumbnail Image"
//                           />
//                         </div>
//                         <div className="admin_info">
//                           <h4 className="admin_name">Home Visit</h4>
//                           <span className="admin_designation">
//                             Lara Madrigal
//                           </span>
//                         </div>
//                       </div>
//                       <ul className="rating_star">
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                       </ul>
//                       <p className="mb-0">
//                         Ut tortor pretium viverra suspendisse potenti nullam.
//                         Venenatis urna cursus eget nunc scelerisque viverra
//                         mauris in aliquam
//                       </p>
//                       <span className="quote_icon">
//                         <FontAwesomeIcon icon={faQuoteRight} />{" "}
//                       </span>
//                     </div>
//                   </div>
//                   <div className="col carousel_item">
//                     <div className="testimonial_item">
//                       <div className="testimonial_admin">
//                         <div className="admin_thumbnail">
//                           <img
//                             src="src/assets/images/meta/thumbnail_img_1.png"
//                             alt="Pet Thumbnail Image"
//                           />
//                         </div>
//                         <div className="admin_info">
//                           <h4 className="admin_name">Home Visits</h4>
//                           <span className="admin_designation">
//                             Lucas Simões
//                           </span>
//                         </div>
//                       </div>
//                       <ul className="rating_star">
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                       </ul>
//                       <p className="mb-0">
//                         Tristique nulla aliquet enim tortor at auctor urna nunc.
//                         Massa enim nec dui nunc mattis enim ut tellus
//                       </p>
//                       <span className="quote_icon">
//                         <FontAwesomeIcon icon={faQuoteRight} />{" "}
//                       </span>
//                     </div>
//                   </div>
//                   <div className="col carousel_item">
//                     <div className="testimonial_item">
//                       <div className="testimonial_admin">
//                         <div className="admin_thumbnail">
//                           <img
//                             src="src/assets/images/meta/thumbnail_img_2.png"
//                             alt="Pet Thumbnail Image"
//                           />
//                         </div>
//                         <div className="admin_info">
//                           <h4 className="admin_name">Dog Boarding</h4>
//                           <span className="admin_designation">
//                             Wilhelm Dowall
//                           </span>
//                         </div>
//                       </div>
//                       <ul className="rating_star">
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                       </ul>
//                       <p className="mb-0">
//                         Lectus magna fringilla urna porttitor rhoncus dolor
//                         purus non enim. Tellus in hac habitasse platea dictumst
//                       </p>
//                       <span className="quote_icon">
//                         <FontAwesomeIcon icon={faQuoteRight} />{" "}
//                       </span>
//                     </div>
//                   </div>
//                   <div className="col carousel_item">
//                     <div className="testimonial_item">
//                       <div className="testimonial_admin">
//                         <div className="admin_thumbnail">
//                           <img
//                             src="src/assets/images/meta/thumbnail_img_3.png"
//                             alt="Pet Thumbnail Image"
//                           />
//                         </div>
//                         <div className="admin_info">
//                           <h4 className="admin_name">Pet Training</h4>
//                           <span className="admin_designation">
//                             Lara Madrigal
//                           </span>
//                         </div>
//                       </div>
//                       <ul className="rating_star">
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                       </ul>
//                       <p className="mb-0">
//                         Ut tortor pretium viverra suspendisse potenti nullam.
//                         Venenatis urna cursus eget nunc scelerisque viverra
//                         mauris in aliquam
//                       </p>
//                       <span className="quote_icon">
//                         <FontAwesomeIcon icon={faQuoteRight} />{" "}
//                       </span>
//                     </div>
//                   </div>
//                   <div className="col carousel_item">
//                     <div className="testimonial_item">
//                       <div className="testimonial_admin">
//                         <div className="admin_thumbnail">
//                           <img
//                             src="src/assets/images/meta/thumbnail_img_4.png"
//                             alt="Pet Thumbnail Image"
//                           />
//                         </div>
//                         <div className="admin_info">
//                           <h4 className="admin_name">Home Visit</h4>
//                           <span className="admin_designation">
//                             Lara Madrigal
//                           </span>
//                         </div>
//                       </div>
//                       <ul className="rating_star">
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                         <li>
//                           <FontAwesomeIcon icon={faStar} />{" "}
//                         </li>
//                       </ul>
//                       <p className="mb-0">
//                         Ut tortor pretium viverra suspendisse potenti nullam.
//                         Venenatis urna cursus eget nunc scelerisque viverra
//                         mauris in aliquam
//                       </p>
//                       <span className="quote_icon">
//                         <FontAwesomeIcon icon={faQuoteRight} />{" "}
//                       </span>
//                     </div>
//                   </div>
//                 </Slider>
//               </div>
//             </section>
//             <section className="service_section section_space_lg pb-0">
//               <div className="container">
//                 <div className="section_title">
//                   <h2 className="title_text mb-0">Other Services</h2>
//                 </div>
//                 <div className="row justify-content-center">
//                   <div className="col col-lg-4">
//                     <div
//                       className="service_item"
//                       style={{
//                         backgroundImage:
//                           'url("src/assets/images/shape/shape_path_1.svg")',
//                       }}
//                     >
//                       <div className="title_wrap">
//                         <div className="item_icon">
//                           <img
//                             src="src/assets/images/icon/icon_pet_training.svg"
//                             alt="Pet Training"
//                           />
//                         </div>
//                         <h3 className="item_title mb-0">Pet Training</h3>
//                       </div>
//                       <p>
//                         Aliquam ut porttitor leo a diam sollicitudin tempor sit
//                         amet est placerat
//                       </p>
//                       <div className="item_price">
//                         <span>From $27 / hour</span>
//                       </div>
//                       <Link className="btn_unfill" to="/service_details">
//                         <span>Get Service</span>{" "}
//                         <FontAwesomeIcon icon={faArrowRight} />
//                       </Link>
//                       <div className="decoration_image">
//                         <img
//                           src="src/assets/images/shape/shape_paws.svg"
//                           alt="Pet Paws"
//                         />
//                       </div>
//                     </div>
//                   </div>
//                   <div className="col col-lg-4">
//                     <div
//                       className="service_item"
//                       style={{
//                         backgroundImage:
//                           'url("src/assets/images/shape/shape_path_1.svg")',
//                       }}
//                     >
//                       <div className="title_wrap">
//                         <div className="item_icon">
//                           <img
//                             src="src/assets/images/icon/icon_pet_grooming.svg"
//                             alt="Pet Grooming"
//                           />
//                         </div>
//                         <h3 className="item_title mb-0">Pet Grooming</h3>
//                       </div>
//                       <p>
//                         Et odio pellentesque diam volutpat commodo sed egestas
//                         egestas pellentesque nec nam
//                       </p>
//                       <div className="item_price">
//                         <span>From $39 / complex</span>
//                       </div>
//                       <Link className="btn_unfill" to="/service_details">
//                         <span>Get Service</span>{" "}
//                         <FontAwesomeIcon icon={faArrowRight} />
//                       </Link>
//                       <div className="decoration_image">
//                         <img
//                           src="src/assets/images/shape/shape_paws.svg"
//                           alt="Pet Paws"
//                         />
//                       </div>
//                     </div>
//                   </div>
//                   <div className="col col-lg-4">
//                     <div
//                       className="service_item"
//                       style={{
//                         backgroundImage:
//                           'url("src/assets/images/shape/shape_path_1.svg")',
//                       }}
//                     >
//                       <div className="title_wrap">
//                         <div className="item_icon">
//                           <img
//                             src="src/assets/images/icon/icon_pet_walking.svg"
//                             alt="Pet Walking"
//                           />
//                         </div>
//                         <h3 className="item_title mb-0">
//                           Walking &amp; Sitting
//                         </h3>
//                       </div>
//                       <p>
//                         Ut tortor pretium viverra suspendisse potenti nullam ac
//                         tortor vitae eget dolor morbi
//                       </p>
//                       <div className="item_price">
//                         <span>From $15 / hour</span>
//                       </div>
//                       <Link className="btn_unfill" to="/service_details">
//                         <span>Get Service</span>{" "}
//                         <FontAwesomeIcon icon={faArrowRight} />
//                       </Link>
//                       <div className="decoration_image">
//                         <img
//                           src="src/assets/images/shape/shape_paws.svg"
//                           alt="Pet Paws"
//                         />
//                       </div>
//                     </div>
//                   </div>
//                 </div>
//               </div>
//             </section>
//           </main>
//         </div>
//       </div>
//     </div>
//   );
// }

// export default Service_Details;
import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowLeft } from "@fortawesome/free-solid-svg-icons";
import { Button } from "antd";
import BookingCard from "../BookingCard.jsx";

function Service_Details() {
  const { serviceId } = useParams();
  const [service, setService] = useState(null);
  const [isOpen, setIsOpen] = useState(false);
  const [selectedServiceId, setSelectedServiceId] = useState(null);

  useEffect(() => {
    const fetchServiceDetails = async () => {
      console.log(serviceId);
      try {
        const response = await axios.get(`https://localhost:7150/api/Service/${serviceId}`);
        setService(response.data.data);
      } catch (error) {
        console.error("Error fetching service details:", error);
      }
    };
    fetchServiceDetails();
  }, [serviceId]);

  const handleBookNow = (serviceID) => {
    setIsOpen(true);
    setSelectedServiceId(serviceID);
  };

  function handleHideModal() {
    setIsOpen(false);
  }

  if (!service) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <div className="breadcrumb_section">
        <div className="container">
          <div className="row">
            <div className="col col-lg-4 col-md-7 col-sm-9">
              <ul className="breadcrumb_nav">
                <li>
                  <Link to="/service">
                    <FontAwesomeIcon icon={faArrowLeft} /> Back to Services
                  </Link>
                </li>
                <li>{service.serviceName}</li>
              </ul>
              <h1 className="page_title">{service.serviceName}</h1>
              <p className="page_description mb-0">{service.serviceDescription}</p>
            </div>
          </div>
        </div>
      </div>
      <section className="service_detail_section section_space_lg">
        <div className="container">
          <div className="row">
            <div className="col col-lg-6">
              <img src={service.image} alt={service.serviceName} className="service_image" />
            </div>
            <div className="col col-lg-6">
              <h2 className="service_title">{service.serviceName}</h2>
              <p>{service.serviceDescription}</p>
              <div className="item_price">
                <span>{service.price}</span>
              </div>
              <Button
                onClick={() => handleBookNow(service.serviceId)}
                className="group w-32 h-10 m-4 bg-gradient-to-r from-purple-500 to-blue-500 text-white font-semibold rounded-full shadow-lg hover:from-purple-600 hover:to-blue-600 transition duration-300 ease-in-out transform hover:scale-105"
              >
                <span className="block group-hover:text-black transition duration-300 ease-in-out">
                  Book now
                </span>
              </Button>
              {isOpen && selectedServiceId === service.serviceId && (
                <BookingCard
                  isOpen={isOpen}
                  handleHideModal={handleHideModal}
                  serviceId={service.serviceId}
                />
              )}
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}

export default Service_Details;