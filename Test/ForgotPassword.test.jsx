/* eslint-disable no-unused-vars */
/* eslint-disable no-undef */
import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { BrowserRouter as Router } from "react-router-dom";
import ForgotPassword from "./src/components/Forgotpassword/ForgotPassword";

describe("ForgotPassword component", () => {
  beforeEach(() => {
    fetch.resetMocks();
  });

  test("renders correctly", () => {
    render(
      <Router>
        <ForgotPassword />
      </Router>
    );
    expect(screen.getByText(/Forgot Password/i)).toBeInTheDocument();
  });

  test("shows error message when email is not found", async () => {
    fetch.mockResponseOnce(JSON.stringify({}), { status: 404 });

    render(
      <Router>
        <ForgotPassword />
      </Router>
    );

    fireEvent.change(screen.getByLabelText(/Email Address/i), {
      target: { value: "nonexistent@example.com" },
    });
    fireEvent.click(screen.getByText(/Send/i));

    await waitFor(() =>
      expect(
        screen.getByText(/Email not found or an error occurred./i)
      ).toBeInTheDocument()
    );
  });

  test("shows success message when email is found", async () => {
    fetch.mockResponseOnce(JSON.stringify({}), { status: 200 });

    render(
      <Router>
        <ForgotPassword />
      </Router>
    );

    fireEvent.change(screen.getByLabelText(/Email Address/i), {
      target: { value: "existing@example.com" },
    });
    fireEvent.click(screen.getByText(/Send/i));

    await waitFor(() =>
      expect(
        screen.getByText(/Check your email for password reset instructions./i)
      ).toBeInTheDocument()
    );
  });

  test("disables button and shows loading state when request is in progress", async () => {
    fetch.mockResponseOnce(
      () =>
        new Promise((resolve) =>
          setTimeout(() => resolve({ status: 200 }), 100)
        )
    );

    render(
      <Router>
        <ForgotPassword />
      </Router>
    );

    fireEvent.change(screen.getByLabelText(/Email Address/i), {
      target: { value: "loading@example.com" },
    });
    fireEvent.click(screen.getByText(/Send/i));

    expect(screen.getByText(/Sending.../i)).toBeInTheDocument();
    expect(screen.getByRole("button")).toBeDisabled();

    await waitFor(() =>
      expect(
        screen.getByText(/Check your email for password reset instructions./i)
      ).toBeInTheDocument()
    );
  });

  test("shows generic error message when fetch fails", async () => {
    fetch.mockRejectOnce(new Error("Failed to fetch"));

    render(
      <Router>
        <ForgotPassword />
      </Router>
    );

    fireEvent.change(screen.getByLabelText(/Email Address/i), {
      target: { value: "error@example.com" },
    });
    fireEvent.click(screen.getByText(/Send/i));

    await waitFor(() =>
      expect(screen.getByText(/An error occurred./i)).toBeInTheDocument()
    );
  });
});