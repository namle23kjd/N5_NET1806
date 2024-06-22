// const colors = {
//   primary: "#060606",
//   background: "#E0E0E0",
//   disbaled: "#D9D9D9",
import React from "react";
import { Link, useNavigate } from "react-router-dom";
import COVER_IMAGE from "../LoginPage/login.jpg";
import { useState } from "react";
import { message } from "antd";

const Register = () => {
  const [email, setEmail] = useState("");
  const [first, setFirst] = useState("");
  const [last, setLast] = useState("");
  const [password, setPassword] = useState("");
  const [gender, setGender] = useState("");
  const [phone, setPhone] = useState("");
  const [error, setError] = useState("");
  const [checkBox, setCheckBox] = useState(false);
  const navigate = useNavigate();

  async function handleRegister(event) {
    event.preventDefault();

    // Regular expressions and parameters
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const minPasswordLength = 6;
    const passwordRegex =
      /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).{6,30}$/;

    const phoneRegex = /^0\d{9}$/; // Adjust regex for Vietnamese phone number
    const genderRegex = /^(Male|Female|Other)$/i;

    // Validate data
    if(!email){
      setError("Please input email");
      return;
    }
    if (!emailRegex.test(email)) {
      setError("Invalid email format");
      return; // Stop the function if there's an error
    }
    if (password.length < minPasswordLength) {
      setError("Password must be at least 6 characters long");
      return; // Stop the function if there's an error
    }
    if (password.length > 30) {
      setError("Password must not exceed 30 characters");
      return; // Stop the function if there's an error
    }

    if (!passwordRegex.test(password)) {
      setError(
        "Invalid password (must include uppercase, lowercase, number, and special character)"
      );
      return; // Stop the function if there's an error
    }

    if (!phoneRegex.test(phone)) {
      setError("Invalid phone number");
      return; // Stop the function if there's an error
    }

    if (!genderRegex.test(gender)) {
      setError("Invalid gender");
      return; // Stop the function if there's an error
    }
    if (!checkBox) {
      setError("You must agree to the privacy policy and terms");
      return;
    }

    // Prepare data
    const item = {
      email,
      password,
      confirmPassword: password, // No need to send this to server
      fullName: first + " " + last,
      gender,
      phoneNumber: phone,
    };

    try {
      // Call the registration API
      const response = await fetch("https://localhost:7150/api/Auth/Register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        body: JSON.stringify(item),
      });

      // Handle response from API
      if (!response.ok) {
        // If registration fails
        const errorData = await response.json(); // Get error information from response
        setError(errorData.message || "Email is already registered"); // Display error message
      } else {
        // If registration is successful
        const result = await response.json();
        localStorage.setItem("user-info", JSON.stringify(result));

        message.success("You Register successfully");

        navigate("/"); // Clear error message if any
      }
    } catch (error) {
      setError("An error occurred. Please try again later.");
      console.error("Registration error:", error);
    }
  }
  return (
    <div className="w-700 h-screen flex items-start my-1 ">
      <div className="relative w-1/2 h-full flex flex-col">
        <img
          src={COVER_IMAGE}
          className="h-full w-full object-cover bg-center"
        />
      </div>

      <div className="w-1/2 mr-auto h-full bg-[#f5f5f5] flex flex-col px-20 py-10 justify-between items-center">
        {/* <h1 className="text-xl text-[#060606] font-semibold">
          Interative Brand
        </h1> */}
        <div className="text-xl text-[#060606] font-semibold mr-auto max-w-[500px]"></div>
        <div className="w-full flex flex-col  max-w-[500px]">
          <div className="w-full flex flex-col mb-2">
            <h1 className="text-4xl text-center font-semibold mb-4 ">
              Register
            </h1>
          </div>
          <div className="w-full flex flex-col ">
            <div className="w-full flex items-center justify-between">
              <input
                className="w-full/3 text-black py-2 my-2 bg-transparent border-b border-black outline-none focus:outline-none"
                type="text"
                placeholder="First"
                value={first}
                onChange={(e) => setFirst(e.target.value)}
              />
              <input
                className="w-full/3 text-black py-2 my-2 bg-transparent border-b border-black outline-none focus:outline-none"
                type="text"
                value={last}
                onChange={(e) => setLast(e.target.value)}
                placeholder="Last"
              />
            </div>
            <input
              className="w-200 text-black py-2 my-2 bg-transparent border-b border-black outline-none focus:outline-none"
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
            <input
              className="w-200 text-black py-2 my-2 bg-transparent border-b border-black outline-none focus:outline-none"
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />

            <input
              type="tel"
              id="phone"
              name="phone"
              placeholder="PhoneNumber"
              className="w-200 text-black py-2 my-2 bg-transparent border-b border-black outline-none focus:outline-none"
              pattern="[0-9]{3}-[0-9]{3}-[0-9]{4}"
              required
              value={phone}
              onChange={(e) => setPhone(e.target.value)}
            />
            <select
              id="gender"
              name="gender"
              className="w-200 text-black py-3 my-3 bg-transparent border border-black  outline-none focus:outline-none"
              required
              placeholder="Gender"
              value={gender}
              onChange={(e) => setGender(e.target.value)}
            >
              <option value="">Select Gender</option>
              <option value="male">Male</option>
              <option value="female">Female</option>
              <option value="other">Other</option>
            </select>
            <div className="mb-3 fv-plugins-icon-container fv-plugins-bootstrap5-row-valid">
              <div className="form-check">
                <input
                  className="form-check-input"
                  type="checkbox"
                  id="terms-conditions"
                  name="terms"
                  checked={checkBox}
                  onChange={(e) => setCheckBox(e.target.checked)}
                />
                <label htmlFor="terms-conditions" className="text-[#060606]">
                  I agree to{" "}
                  <Link to="/policy" className="text-blue underline">
                    privacy policy &amp; terms
                  </Link>
                </label>
                <div className="fv-plugins-message-container fv-plugins-message-container--enabled invalid-feedback" />
              </div>
            </div>
          </div>
          <p style={{ color: "red" }}>{error}</p>
          <div className="w-full flex flex-col my-4">
            <button
              onClick={handleRegister}
              className="w-full text-white my-2 font-semibold bg-[#060606] rounded-md p-4 text-center flex items-center justify-center cursor-pointer"
            >
              Register
            </button>
          </div>

          {/* <div className="w-full text-[#060606] my-2 font-semibold bg-white border border-black/40 rounded-md p-4 text-center flex items-center justify-center cursor-pointer">
            <img src={LOGIN_GG} className="h-6 mr-2" />
            Sign In With Google
          </div> */}
        </div>
        <div className="w-full flex items-center justify-center py-2">
          <p className="text-lg font-normal text-[#060606]">
            Already have an account?{" "}
            <Link to={"/login"}>
              <span className="font-semibold cursor-pointer">Sign in</span>
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Register;
