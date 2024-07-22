import { useState } from "react";
import { Link } from "react-router-dom";
import COVER_IMAGE from "../../assets/images/background/1.png";

function ForgotPassword() {
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);

    var item = { email };
    try {
      let response = await fetch(
        "https://localhost:7150/api/Auth/forgot-password",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
          },
          body: JSON.stringify(item),
        }
      );

      let responseData = await response.json();

      if (response.ok) {
        setMessage("Check your email for password reset instructions");
      } else {
        console.log(responseData);
        setMessage(responseData.msg);
      }
    } catch (error) {
      setMessage("Có lỗi xảy ra");
      console.error(error);
    } finally {
      setIsLoading(false); // Kết thúc quá trình gửi
    }
  };
  return (
    <div
      className="flex items-center justify-center h-screen"
      style={{
        backgroundImage: `url(${COVER_IMAGE})`,
        backgroundSize: "cover",
        backgroundPosition: "center",
      }}
    >
      <div className="bg-white p-8 rounded-lg shadow-md max-w-md w-full">
        <h2 className="text-3xl font-semibold mb-6 text-center text-gray-800">
          Forgot Password
        </h2>
        <form onSubmit={handleSubmit}>
          <div className="mb-4">
            <label
              htmlFor="email"
              className="block text-sm font-medium text-gray-700"
            >
              Email Address
            </label>
            <input
              type="email"
              id="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="mt-1 p-3 w-full border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring focus:ring-blue-500"
              required
            />
          </div>
          <button
            type="submit"
            className={`bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded-md w-full ${
              isLoading ? "opacity-50 cursor-not-allowed" : ""
            }`}
            disabled={isLoading}
          >
            {isLoading ? "Sending..." : "Send"}
          </button>
          {message && (
            <p className="mt-4 text-sm text-center text-gray-700">{message}</p>
          )}
        </form>
        <div className="mt-4 text-sm text-gray-700 text-center">
          <Link
            to="/login"
            className="text-blue-500 hover:underline focus:outline-none"
          >
            Back to Login
          </Link>
        </div>
      </div>
    </div>
  );
}

export default ForgotPassword;
