import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import '@testing-library/jest-dom';
import ChangePassword from './ChangePassword';
import { BrowserRouter as Router } from 'react-router-dom';
import axios from 'axios';

jest.mock('axios');

describe('ChangePassword', () => {
  beforeEach(() => {
    localStorage.setItem('user-info', JSON.stringify({
      data: {
        token: 'fake-token',
        user: { email: 'test@example.com' },
      },
    }));
  });

  afterEach(() => {
    localStorage.clear();
    jest.clearAllMocks();
  });

  it('should render the component', () => {
    render(
      <Router>
        <ChangePassword />
      </Router>
    );
    expect(screen.getByText('Change Password')).toBeInTheDocument();
  });

  it('should display error if current password is incorrect', async () => {
    axios.post.mockRejectedValueOnce({
      response: { status: 401, data: { message: 'Current password wrong to change' } },
    });

    render(
      <Router>
        <ChangePassword />
      </Router>
    );

    fireEvent.change(screen.getByTestId('currentPassword'), { target: { value: 'wrongpassword' } });
    fireEvent.change(screen.getByTestId('newPassword'), { target: { value: 'newpassword123' } });
    fireEvent.change(screen.getByTestId('confirmPassword'), { target: { value: 'newpassword123' } });

    fireEvent.click(screen.getByRole('button', { name: /Save changes/i }));

    const errorMessage = await screen.findByText('Current password wrong to change');
    expect(errorMessage).toBeInTheDocument();
  });


  it('should show error if new password and confirm password do not match', async () => {
    render(
      <Router>
        <ChangePassword />
      </Router>
    );

    fireEvent.change(screen.getByTestId('currentPassword'), { target: { value: 'correctpassword' } });
    fireEvent.change(screen.getByTestId('newPassword'), { target: { value: 'newpassword123' } });
    fireEvent.change(screen.getByTestId('confirmPassword'), { target: { value: 'differentpassword123' } });

    fireEvent.click(screen.getByRole('button', { name: /Save changes/i }));

    const errorMessage = await screen.findByText('The two passwords do not match!');
    expect(errorMessage).toBeInTheDocument();
  });

  it('should show error if current password is blank', async () => {
    render(
      <Router>
        <ChangePassword />
      </Router>
    );

    fireEvent.change(screen.getByTestId('currentPassword'), { target: { value: '' } });
    fireEvent.change(screen.getByTestId('newPassword'), { target: { value: 'newpassword123' } });
    fireEvent.change(screen.getByTestId('confirmPassword'), { target: { value: 'newpassword123' } });

    fireEvent.click(screen.getByRole('button', { name: /Save changes/i }));

    const errorMessage = await screen.findByText(/Please input your current password!/i);
    expect(errorMessage).toBeInTheDocument();
  });

  it('should show error if new password is too short', async () => {
    render(
      <Router>
        <ChangePassword />
      </Router>
    );

    fireEvent.change(screen.getByTestId('currentPassword'), { target: { value: 'correctpassword' } });
    fireEvent.change(screen.getByTestId('newPassword'), { target: { value: 'short' } });
    fireEvent.change(screen.getByTestId('confirmPassword'), { target: { value: 'short' } });

    fireEvent.click(screen.getByRole('button', { name: /Save changes/i }));

    const errorMessage = await screen.findByText(/Password must be at least 6 characters long!/i);
    expect(errorMessage).toBeInTheDocument();
  });

  it('should show error if new password exceeds maximum length', async () => {
    render(
      <Router>
        <ChangePassword />
      </Router>
    );

    fireEvent.change(screen.getByTestId('currentPassword'), { target: { value: 'correctpassword' } });
    fireEvent.change(screen.getByTestId('newPassword'), { target: { value: 'thisisaverylongpangpangpasswordthisisaverylongpassword' } });
    fireEvent.change(screen.getByTestId('confirmPassword'), { target: { value: 'thisisaverylongpangpangpasswordthisisaverylongpassword' } });

    fireEvent.click(screen.getByRole('button', { name: /Save changes/i }));

    const errorMessage = await screen.findByText(/Password must not exceed 30 characters!/i);
    expect(errorMessage).toBeInTheDocument();
  });
});