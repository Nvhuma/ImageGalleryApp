import React from 'react';
import { useNavigate } from 'react-router-dom';
import './PasswordChanged.css'; // Import the CSS file

const PasswordChanged = () => {
  const navigate = useNavigate();

  const handleBackToLogin = () => {
    navigate('/login'); // Adjust the route to your homepage
  };

  return (
    <div className="password-changed-container">
      <div className="message-container">
        <h1>Password Changed!</h1>
        <p>Your password has been changed successfully!</p>
        <button className="back-to-login-button" onClick={handleBackToLogin}>
          Back to Login
        </button>
      </div>
      <div className="image-container">
        <img src="'/src/assets/image-gallery.jpeg'" />
      </div>
    </div>
  );
};

export default PasswordChanged;
