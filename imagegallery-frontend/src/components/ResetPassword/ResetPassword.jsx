import React, { useState, useEffect } from 'react';
import './ResetPassword.css';
import { useNavigate, useLocation } from 'react-router-dom';
import axios from 'axios';

const ResetPassword = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [token, setToken] = useState('');
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);
    const tokenFromURL = queryParams.get('token');
    const emailFromURL = queryParams.get('Email'); // Extract email from URL
    if (tokenFromURL && emailFromURL) {
      setToken(tokenFromURL);
      setEmail(emailFromURL);
    } else {
      alert('Token or email is missing');
      navigate('/forgot-password'); // Redirect if token or email is missing
    }
  }, [location, navigate]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    console.log("Form submitted");
    console.log("Email: ", email);
    console.log("Password: ", password);
    console.log("Confirm Password: ", confirmPassword);
    console.log("Token: ", token);

    if (password !== confirmPassword) {
      alert("Passwords do not match");
      return;
    }

    try {
      const response = await axios.post('http://localhost:5263/api/Account/reset-password', { 
        email, 
        token, 
        newPassword: password, 
        confirmPassword 
      });
      console.log("API response: ", response);
      navigate('/login'); // Navigate to login or some other page after successful reset
    } catch (error) {
      console.error("Error during password reset: ", error);
      alert("Error resetting password. Please try again.");
    }
  };

  return (
    <div className="reset-password-container">
      <div className="reset-password-form">
        <h1>Reset Password</h1>
        <form onSubmit={handleSubmit}>
          <label>Email Address</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Enter Email"
            required
            readOnly // Make the email field read-only since it's prefilled
          />
          <label>New Password</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="New Password"
            required
          />
          <label>Confirm Password</label>
          <input
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            placeholder="Confirm Password"
            required
          />
          <button type="submit">Reset Password</button>
        </form>
      </div>
      <div className="reset-password-image">
        <img src="/src/assets/image-gallery.jpeg" alt="Reset Password" />
      </div>
    </div>
  );
};

export default ResetPassword;
