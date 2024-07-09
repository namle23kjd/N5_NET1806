import { useState } from "react";
import axios from "axios";
import backgroundImage from "../../assets/images/background/2.jpg";

function ResetPassword() {
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState(null);

  async function handleSubmit(e) {
    e.preventDefault();

    const tokenMatch = window.location.href.match(/token=([^&]+)/);
    const emailMatch = window.location.href.match(/email=([^&]+)/);

    const token = tokenMatch ? tokenMatch[1] : null;
    const email = emailMatch ? emailMatch[1] : null;

    if (!token || !email) {
      setError("Invalid token or email");
      return;
    }

    const minPasswordLength = 6;
    const passwordRegex =
      /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*]).{8,}$/;
    if (
      !passwordRegex.test(newPassword) ||
      newPassword.length < minPasswordLength
    ) {
      setError(
        "Password is invalid (minimum 8 characters, must include uppercase, lowercase, number, and special character)"
      );
      return;
    }

    if (newPassword !== confirmPassword) {
      setError("Passwords do not match");
      return;
    }

    const data = {
      token,
      email,
      password: newPassword,
    };

    try {
      const response = await axios.post(
        "https://localhost:7150/api/Auth/reset-password",
        data
      );

      if (response.status === 200) {
        setMessage("Password reset successful");
        // Redirect to login page or display appropriate message for success
      } else {
        setError(response.data.msg || "Error resetting password");
      }
    } catch (error) {
      setError("An error occurred. Please try again.");
      console.error(error);
    }
  }

  return (
    <div
      className="flex items-center justify-center min-h-screen"
      style={{
        backgroundImage: `url(${backgroundImage})`,
        backgroundSize: "cover",
        backgroundPosition: "center",
      }}
    >
      <form
        onSubmit={handleSubmit}
        className="bg-white p-8 rounded-lg shadow-md max-w-md w-full"
      >
        <h2 className="text-3xl font-semibold mb-6 text-center text-gray-800">
          Reset Password
        </h2>
        <div className="mb-4">
          <label
            htmlFor="newPassword"
            className="block text-sm font-medium text-gray-700"
          >
            New Password
          </label>
          <input
            type="password"
            id="newPassword"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
          />
        </div>
        <div className="mb-6">
          <label
            htmlFor="confirmPassword"
            className="block text-sm font-medium text-gray-700"
          >
            Confirm Password
          </label>
          <input
            type="password"
            id="confirmPassword"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring focus:ring-indigo-200 focus:ring-opacity-50"
          />
        </div>
        <button
          type="submit"
          className="bg-indigo-500 hover:bg-indigo-600 text-white font-bold py-2 px-4 rounded-md w-full"
        >
          Submit
        </button>
        {message && <p className="mt-4 text-sm text-green-500">{message}</p>}
        {error && <p className="mt-4 text-sm text-red-500">{error}</p>}
      </form>
    </div>
  );
}

export default ResetPassword;

