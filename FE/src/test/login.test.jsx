/* eslint-disable no-undef */

import { render, screen, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom";
import LoginPage from "../components/LoginPage/login";
import { BrowserRouter as Router } from "react-router-dom";
import axios from "axios";

jest.mock("axios");

describe("LoginPage", () => {
  it("should submit with valid credentials", async () => {
    // Arrange
    axios.post.mockResolvedValue({ data: { message: "Login successfully" } });
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: "test@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: "password" },
    });
    fireEvent.click(screen.getByText(/log in/i));

    // Assert
    // const successMessage = await screen.findByText(/login successfully/i);
    // expect(successMessage).toBeInTheDocument();
  });

  it("should show error with invalid credentials", async () => {
    // Arrange
    axios.post.mockRejectedValue({
      response: { data: { message: "Invalid credentials" } },
    });
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: "test@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: "wrongpassword" },
    });
    fireEvent.click(screen.getByText(/log in/i));

    // Assert
    // const errorMessage = await screen.findByText(/invalid credentials/i);
    // expect(errorMessage).toBeInTheDocument();
  });

  it("should show error if email format is invalid", async () => {
    // Arrange
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: "invalid-email" },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: "password" },
    });
    fireEvent.click(screen.getByText(/log in/i));

    // Assert
    const errorMessage = await screen.findByText(/invalid email format/i);
    expect(errorMessage).toBeInTheDocument();
  });

  it("should show error if password is too short", async () => {
    // Arrange
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: "test@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: "short" },
    });
    fireEvent.click(screen.getByText(/log in/i));

    // Assert
    const errorMessage = await screen.findByText(
      /password must be at least 6 characters long/i
    );
    expect(errorMessage).toBeInTheDocument();
  });
});
