import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import '@testing-library/jest-dom';
import LoginPage from './login';
import { BrowserRouter as Router } from 'react-router-dom';
import axios from 'axios';

jest.mock('axios');

describe('LoginPage', () => {
  it('should submit with valid credentials', async () => {
    // Arrange
    axios.post.mockResolvedValue({ data: { message: 'Login successfully' } });
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: 'test@example.com' },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: 'password' },
    });
    fireEvent.click(screen.getByText(/log in/i));

  });

  it('should show error with invalid credentials', async () => {
    // Arrange
    axios.post.mockRejectedValue({
      response: { data: { message: 'Login failed. Please check your credentials.' } },
    });
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: 'test@example.com' },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: 'wrongpassword' },
    });
    fireEvent.click(screen.getByText(/log in/i));

  });

  it('should show error if email format is invalid', async () => {
    // Arrange
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: 'invalid-email' },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: 'password' },
    });
    fireEvent.click(screen.getByText(/log in/i));

    // Assert
    const errorMessage = await screen.findByText(/invalid email format/i);
    expect(errorMessage).toBeInTheDocument();
  });

  it('should show error if password is too short', async () => {
    // Arrange
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: 'test@example.com' },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: 'short' },
    });
    fireEvent.click(screen.getByText(/log in/i));

    // Assert
    const errorMessage = await screen.findByText(/password must be at least 6 characters long/i);
    expect(errorMessage).toBeInTheDocument();
  });

  it('should show error if password exceeds maximum length', async () => {
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: 'test@example.com' },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: 'thisisaverylongpasswordthisisaverylongpassword' },
    });
    fireEvent.click(screen.getByText(/log in/i));

    const errorMessage = await screen.findByText(/password must not exceed 30 characters/i);
    expect(errorMessage).toBeInTheDocument();
  });
  

  it('should show error if username is blank', async () => {
    // Arrange
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: '' },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: 'password' },
    });
    fireEvent.click(screen.getByText(/log in/i));

    // Assert
    const errorMessage = await screen.findByText(/username must not be blank/i);
    expect(errorMessage).toBeInTheDocument();
  });

  it('should show error if password is blank', async () => {
    // Arrange
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: 'test@example.com' },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: '' },
    });
    fireEvent.click(screen.getByText(/log in/i));

    // Assert
    const errorMessage = await screen.findByText(/password must not be blank/i);
    expect(errorMessage).toBeInTheDocument();
  });
  it('should show error if both username and password are blank', async () => {
    // Arrange
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Act
    fireEvent.change(screen.getByPlaceholderText(/email/i), {
      target: { value: '' },
    });
    fireEvent.change(screen.getByPlaceholderText(/password/i), {
      target: { value: '' },
    });
    fireEvent.click(screen.getByText(/log in/i));

    // Assert
    const errorMessage = await screen.findByText(/username and password must not be blank/i);
    expect(errorMessage).toBeInTheDocument();
  });

  it('should show error if user is already logged in', async () => {
    // Arrange
    localStorage.setItem('user-info', JSON.stringify({ user: 'test user' }));
    render(
      <Router>
        <LoginPage />
      </Router>
    );

    // Assert
    const loginError = await screen.findByText(/logined/i);
    expect(loginError).toBeInTheDocument();

    // Cleanup
    localStorage.removeItem('user-info');
  });
});