import React, { useState, useEffect } from 'react';
import './ResetPassword.css';
import { useNavigate, useLocation } from 'react-router-dom';
import axios from 'axios';
import VisibilityIcon from '@mui/icons-material/Visibility';
import VisibilityOffIcon from '@mui/icons-material/VisibilityOff';

const ResetPassword = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [token, setToken] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    const queryParams = new URLSearchParams(location.search);
    const tokenFromURL = queryParams.get('token');
    const emailFromURL = queryParams.get('Email');
    if (tokenFromURL && emailFromURL) {
      setToken(tokenFromURL);
      setEmail(emailFromURL);
    } else {
      alert('Token or email is missing');
      navigate('/forgot-password');
    }
  }, [location, navigate]);

  const handleSubmit = async (event) => {
    event.preventDefault();
    setErrorMessage('');

    if (password !== confirmPassword) {
      setErrorMessage("Passwords do not match");
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
      navigate('/PasswordChanged');
    } catch (error) {
      console.error("Error during password reset: ", error);
      const errorMessage = error.response?.data?.message || "Error resetting password. Please try again.";
      setErrorMessage(errorMessage);
    }
  };

  return (
    <div className="reset-password-container">
      <div className="reset-password-form">
        <h1>Reset Password</h1>
        {errorMessage && <p className="error-message">{errorMessage}</p>}
        <form onSubmit={handleSubmit}>
          <input
            type="hidden"
            value={email}
            readOnly
          />
          <label>New Password</label>
          <div className="password-input-container">
            <input
              type={showPassword ? "text" : "password"}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="New Password"
              required
            />
            <span
              className="password-toggle-icon"
              onClick={() => setShowPassword(!showPassword)}
            >
              {showPassword ? <VisibilityOffIcon /> : <VisibilityIcon />}
            </span>
          </div>
          <label>Confirm Password</label>
          <div className="password-input-container">
            <input
              type={showConfirmPassword ? "text" : "password"}
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              placeholder="Confirm Password"
              required
            />
            <span
              className="password-toggle-icon"
              onClick={() => setShowConfirmPassword(!showConfirmPassword)}
            >
              {showConfirmPassword ? <VisibilityOffIcon /> : <VisibilityIcon />}
              
            </span>
          </div>
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
