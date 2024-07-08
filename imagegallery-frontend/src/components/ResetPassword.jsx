import React, { useState } from 'react';
import './ResetPassword.css';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';  // Assuming you are using axios for API calls

const ResetPassword = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const navigate = useNavigate(); // Assuming you want to navigate after a successful reset

  const handleSubmit = async (event) => {
    event.preventDefault();
    console.log("Form submitted");
    console.log("Email: ", email);
    console.log("Password: ", password);
    console.log("Confirm Password: ", confirmPassword);

    if (password !== confirmPassword) {
      alert("Passwords do not match");
      return;
    }

    try {
      const response = await axios.post('/api/account/reset-password', { email, password });
      console.log("API response: ", response);
      // Navigate to login or some other page after successful reset
      navigate('/login');
    } catch (error) {
      console.error("Error during password reset: ", error);
      alert("Error resetting password");
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
          />
          <label>Password</label>
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
            placeholder="Enter Password"
            required
          />
          <button type="submit">Reset Password</button>
        </form>
      </div>
      <div className="reset-password-image">
        <img src="/path/to/your/image.jpg" alt="Reset Password" />
      </div>
    </div>
  );
};

export default ResetPassword;
