import React, { useEffect } from "react";
import { Navigate } from "react-router-dom";
import { message } from "antd";

const ProtectedRoute = ({ children, allowedRoles }) => {
  const userInfoString = localStorage.getItem("user-info");

  useEffect(() => {
    if (!userInfoString) {
      // Nếu không có thông tin người dùng, hiển thị thông báo và điều hướng đến trang đăng nhập
      message.error("You need to login to access this page.");
    } else {
      const userInfo = JSON.parse(userInfoString);
      const role = userInfo.data.user.role;
      if (!allowedRoles.includes(role)) {
        // Nếu vai trò người dùng không được phép, hiển thị thông báo lỗi
        message.error("You do not have permission to access this page.");
      }
    }
  }, [userInfoString, allowedRoles]);

  if (!userInfoString) {
    return <Navigate to="/login" replace />;
  }

  const userInfo = JSON.parse(userInfoString);
  const role = userInfo.data.user.role;
  if (!allowedRoles.includes(role)) {
    return <Navigate to="/" replace />;
  }

  // Nếu vai trò người dùng được phép, render component con
  return children;
};

export default ProtectedRoute;
