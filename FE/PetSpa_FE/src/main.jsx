import { RouterProvider, createBrowserRouter } from "react-router-dom";
// import MainLayout from "./layout/MainLayout";
import ReactDOM from "react-dom/client";
import "./index.css";
import LoginPage from "./components/LoginPage/login";
import Register from "./components/Register/Register";
import ForgotPassword from "./components/Forgotpassword/ForgotPassword";
import ResetPassword from "./components/ResetPassword/ResetPassword";
import Petmanagement from "./components/Petmanagement/Petmanagement";
import HomePage from "./components/HomePage/HomePage";
import Service from "./components/Service/Service";
import Cart from "./components/Cart/Cart";
import MainLayout from "./layout/MainLayout";
import Account from "./components/Account/Account";
import AccountSetting from "./components/Account/AccountSetting";
import PrivacyPolicyTerms from "./components/Register/policy";
import Logout from "./components/Logout/logout";
import Transac from "./components/Service/transac";
import Blog from "./components/Blog/Blog";
import BlogDetails from "./components/Blog/Blog_Details";
import AdminPage from "./components/Account/AdminPage";
import StaffPage from "./components/Account/StaffPage";
import ManagerPage from "./components/Account/ManagerPage";
import ProtectedRoute from "./components/ProtectedRoute";
//route
const router = createBrowserRouter([
  {
    element: <MainLayout />,
    children: [
      {
        path: "/",
        element: <HomePage />,
      },
      {
        path: "/login",
        element: <LoginPage />,
      },
      {
        path: "/zadmin",
        element: <AdminPage />,
      },
      {
        path: "/register",
        element: <Register />,
      },
      {
        path: "/forgot-password",
        element: <ForgotPassword />,
      },
      {
        path: "/reset-password",
        element: <ResetPassword />,
      },
      {
        path: "/Pet",
        element: (
          <ProtectedRoute allowedRoles={["Customer"]}>
            <Petmanagement />
          </ProtectedRoute>
        ),
      },
      {
        path: "/HOMEPage",
        element: <HomePage />,
      },
      {
        path: "/Service",
        element: <Service />,
      },
      {
        path: "/Cart",
        element: (
          <ProtectedRoute allowedRoles={["Customer"]}>
            <Cart />
          </ProtectedRoute>
        ),
      },
      {
        path: "/Account",
        element: (
          <ProtectedRoute allowedRoles={["Customer"]}>
            <Account />
          </ProtectedRoute>
        ),
      },
      {
        path: "/AccountSetting",
        element: (
          <ProtectedRoute allowedRoles={["Customer"]}>
            <AccountSetting />
          </ProtectedRoute>
        ),
      },
      {
        path: "/policy",
        element: <PrivacyPolicyTerms />,
      },
      {
        path: "/logout",
        element: <Logout />,
      },
      {
        path: "/transac",
        element: (
          <ProtectedRoute allowedRoles={["Customer"]}>
            <Transac />
          </ProtectedRoute>
        ),
      },
      {
        path: "/blog",
        element: <Blog />,
      },
      {
        path: "/post/:id",
        element: <BlogDetails />,
      },
      {
        path: "/admin",
        element: (
          <ProtectedRoute allowedRoles={["Admin"]}>
            <AdminPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "/staff",
        element: (
          <ProtectedRoute allowedRoles={["Staff"]}>
            <StaffPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "/manager",
        element: (
          <ProtectedRoute allowedRoles={["Manager"]}>
            <ManagerPage />
          </ProtectedRoute>
        ),
      },
    ],
  },
]);

ReactDOM.createRoot(document.getElementById("root")).render(
  <RouterProvider router={router} />
);
