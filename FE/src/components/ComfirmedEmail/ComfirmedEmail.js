import { useEffect } from "react";

import axios from "axios";
import { useNavigate } from "react-router-dom";

function ComfirmedEmail() {
  const navigate = useNavigate();
  async function coEmail(e) {
    e.preventDefault();

    const tokenMatch = window.location.href.match(/token=([^&]+)/);
    const emailMatch = window.location.href.match(/email=([^&]+)/);

    const token = tokenMatch ? tokenMatch[1] : null;
    const email = emailMatch ? emailMatch[1] : null;
    if (!token || !email) {
      return;
    }
    const data = {
      token,
      email,
    };
    try {
      const response = await axios.post(
        "https://localhost:7150/api/Auth/ConfirmEmail",
        data // Axios tự động chuyển đổi data thành JSON
      );

      if (response.status === 200) {
        // Kiểm tra mã trạng thái 200 OK
        navigate("/");
        // ... (Chuyển hướng đến trang đăng nhập)
      } else {
        console.log(response.msg || "Có lỗi xảy ra khi đặt lại mật khẩu");
      }
    } catch (error) {
      console.log("Có lỗi xảy ra. Vui lòng thử lại.");
      console.error(error);
    }
  }
  useEffect(function () {
    coEmail();
  }, []);
}

export default ComfirmedEmail;
