import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import "@testing-library/jest-dom";
import { BrowserRouter as Router } from "react-router-dom";
import Register from "./src/components/Register/Register";

// Import fetchMock
import fetchMock from "jest-fetch-mock";

// Enable fetchMock
fetchMock.enableMocks();

describe("Register", () => {
  beforeEach(() => {
    fetch.resetMocks();
  });

  it("should submit with valid credentials", async () => {
    // Arrange
    fetch.mockResponseOnce(
      JSON.stringify({ message: "You Register successfully" })
    );
    render(
      <Router>
        <Register />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/First/i), {
      target: { value: "John" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Last/i), {
      target: { value: "Doe" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Email/i), {
      target: { value: "test@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), {
      target: { value: "Password123!" },
    });
    fireEvent.change(screen.getByPlaceholderText(/PhoneNumber/i), {
      target: { value: "0123456789" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Gender/i), {
      target: { value: "male" },
    });
    fireEvent.click(
      screen.getByRole("checkbox", {
        name: /I agree to privacy policy & terms/i,
      })
    );
    fireEvent.click(screen.getByRole("button", { name: /Register/i }));

    // Assert
    await waitFor(() => {
      expect(
        screen.getByText(/You Register successfully/i)
      ).toBeInTheDocument();
    });
  });

  it("should show error if email format is invalid", async () => {
    // Arrange
    render(
      <Router>
        <Register />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/First/i), {
      target: { value: "John" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Last/i), {
      target: { value: "Doe" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Email/i), {
      target: { value: "invalid-email" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), {
      target: { value: "Password123!" },
    });
    fireEvent.change(screen.getByPlaceholderText(/PhoneNumber/i), {
      target: { value: "0123456789" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Gender/i), {
      target: { value: "male" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Register/i }));

    // Assert
    await waitFor(() => {
      expect(screen.getByText(/Invalid email format/i)).toBeInTheDocument();
    });
  });

  it("should show error if password is too short", async () => {
    // Arrange
    render(
      <Router>
        <Register />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/First/i), {
      target: { value: "John" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Last/i), {
      target: { value: "Doe" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Email/i), {
      target: { value: "test@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), {
      target: { value: "short" },
    });
    fireEvent.change(screen.getByPlaceholderText(/PhoneNumber/i), {
      target: { value: "0123456789" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Gender/i), {
      target: { value: "male" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Register/i }));

    // Assert
    const errorMessage = await screen.findByText(
      /Password must be at least 6 characters long/i
    );
    expect(errorMessage).toBeInTheDocument();
  });

  it("should show error if password exceeds maximum length", async () => {
    // Arrange
    render(
      <Router>
        <Register />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/First/i), {
      target: { value: "John" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Last/i), {
      target: { value: "Doe" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Email/i), {
      target: { value: "test@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), {
      target: {
        value: "thisisaverylongpasswordthatexceedsthelimitismaxlengthpassword",
      },
    });
    fireEvent.change(screen.getByPlaceholderText(/PhoneNumber/i), {
      target: { value: "0123456789" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Gender/i), {
      target: { value: "male" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Register/i }));

    // Assert
    await waitFor(() => {
      expect(
        screen.getByText(/password must not exceed 30 characters/i)
      ).toBeInTheDocument();
    });
  });

  it("should show error if password is invalid", async () => {
    // Arrange
    render(
      <Router>
        <Register />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/First/i), {
      target: { value: "John" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Last/i), {
      target: { value: "Doe" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Email/i), {
      target: { value: "test@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), {
      target: { value: "password" },
    });
    fireEvent.change(screen.getByPlaceholderText(/PhoneNumber/i), {
      target: { value: "0123456789" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Gender/i), {
      target: { value: "male" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Register/i }));

    // Assert
    const errorMessage = await screen.findByText(
      /Invalid password \(must include uppercase, lowercase, number, and special character\)/i
    );
    expect(errorMessage).toBeInTheDocument();
  });

  it("should show error if phone number is invalid", async () => {
    // Arrange
    render(
      <Router>
        <Register />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/First/i), {
      target: { value: "John" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Last/i), {
      target: { value: "Doe" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Email/i), {
      target: { value: "test@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), {
      target: { value: "Password123!" },
    });
    fireEvent.change(screen.getByPlaceholderText(/PhoneNumber/i), {
      target: { value: "invalid-phone" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Gender/i), {
      target: { value: "male" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Register/i }));

    // Assert
    const errorMessage = await screen.findByText(/Invalid phone number/i);
    expect(errorMessage).toBeInTheDocument();
  });

  it("should show error if gender is invalid", async () => {
    // Arrange
    render(
      <Router>
        <Register />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/First/i), {
      target: { value: "John" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Last/i), {
      target: { value: "Doe" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Email/i), {
      target: { value: "test@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), {
      target: { value: "Password123!" },
    });
    fireEvent.change(screen.getByPlaceholderText(/PhoneNumber/i), {
      target: { value: "0123456789" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Gender/i), {
      target: { value: "invalid-gender" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Register/i }));

    // Assert
    const errorMessage = await screen.findByText(/Invalid gender/i);
    expect(errorMessage).toBeInTheDocument();
  });
  it("should show error if email is empty", async () => {
    // Arrange
    render(
      <Router>
        <Register />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/First/i), {
      target: { value: "John" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Last/i), {
      target: { value: "Doe" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Email/i), {
      target: { value: "" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), {
      target: { value: "Password123!" },
    });
    fireEvent.change(screen.getByPlaceholderText(/PhoneNumber/i), {
      target: { value: "0123456789" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Gender/i), {
      target: { value: "male" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Register/i }));

    // Assert
    const errorMessage = await screen.findByText(/Please input email/i);
    expect(errorMessage).toBeInTheDocument();
  });
//   it("should show error if phone number contains letters", async () => {
//     // Arrange
//     render(
//       <Router>
//         <Register />
//       </Router>
//     );

//     // Act
//     fireEvent.change(screen.getByPlaceholderText(/First/i), {
//       target: { value: "John" },
//     });
//     fireEvent.change(screen.getByPlaceholderText(/Last/i), {
//       target: { value: "Doe" },
//     });
//     fireEvent.change(screen.getByPlaceholderText(/Email/i), {
//       target: { value: "test@example.com" },
//     });
//     fireEvent.change(screen.getByPlaceholderText(/Password/i), {
//       target: { value: "Password123!" },
//     });
//     fireEvent.change(screen.getByPlaceholderText(/PhoneNumber/i), {
//       target: { value: "123abc456" },
//     });
//     fireEvent.change(screen.getByPlaceholderText(/Gender/i), {
//       target: { value: "male" },
//     });
//     fireEvent.click(screen.getByRole("button", { name: /Register/i }));

//     // Assert
//     const errorMessage = await screen.findByText(/Invalid phone number/i);
//     expect(errorMessage).toBeInTheDocument();
//   });

//   it("should check the terms and conditions checkbox", async () => {
//     // Arrange
//     render(
//       <Router>
//         <Register />
//       </Router>
//     );

//     // Act
//     const checkbox = screen.getByRole("checkbox", {
//       name: /I agree to privacy policy & terms/i,
//     });
//     fireEvent.click(checkbox);

//     // Assert
//     expect(checkbox).toBeChecked();
//   });
  it("should show error if terms and conditions checkbox is not checked", async () => {
    // Arrange
    render(
      <Router>
        <Register />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/First/i), {
      target: { value: "John" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Last/i), {
      target: { value: "Doe" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Email/i), {
      target: { value: "test@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), {
      target: { value: "Password123!" },
    });
    fireEvent.change(screen.getByPlaceholderText(/PhoneNumber/i), {
      target: { value: "0123456789" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Gender/i), {
      target: { value: "male" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Register/i }));

    // Assert
    await waitFor(() => {
      expect(
        screen.getByText(/You must agree to the privacy policy and terms/i)
      ).toBeInTheDocument();
    });
  });
  it("should show error if email is already registered", async () => {
    // Arrange
    fetch.mockResponseOnce(
      JSON.stringify({ message: "Email is already registered" }), { status: 400 }
    );
    render(
      <Router>
        <Register />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/First/i), {
      target: { value: "John" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Last/i), {
      target: { value: "Doe" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Email/i), {
      target: { value: "registered@example.com" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Password/i), {
      target: { value: "Password123!" },
    });
    fireEvent.change(screen.getByPlaceholderText(/PhoneNumber/i), {
      target: { value: "0123456789" },
    });
    fireEvent.change(screen.getByPlaceholderText(/Gender/i), {
      target: { value: "male" },
    });
    fireEvent.click(screen.getByRole("checkbox", {
      name: /I agree to privacy policy & terms/i,
    }));
    fireEvent.click(screen.getByRole("button", { name: /Register/i }));

    // Assert
    await waitFor(() => {
      expect(fetch).toHaveBeenCalledWith(
        "https://localhost:7150/api/Auth/Register",
        expect.objectContaining({
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            "Accept": "application/json", // Include Accept header
          },
          body: JSON.stringify({
            email: "registered@example.com",
            password: "Password123!",
            confirmPassword: "Password123!", // Include confirmPassword field
            fullName: "John Doe",
            gender: "male",
            phoneNumber: "0123456789",
          }),
        })
      );
      expect(screen.getByText(/Email is already registered/i)).toBeInTheDocument();
    });
  });
});
