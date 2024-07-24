import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import './logout.css';

const LogoutPage = () => {
  const navigate = useNavigate();

  useEffect(() => {
    const performLogout = async () => {
      try {
        await axios.post('http://localhost:5263/api/Account/logout');
        // Clear any stored tokens or user information
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        navigate('/logout'); // Navigate to the login page after logout
      } catch (error) {
        console.error('Logout failed', error);
        alert('Logout failed!');
      }
    };
	
    performLogout();
  }, [navigate]);

  const handleBackToLogin = () => {
    navigate('/login');
  };

  return (
    <div className="container">
      <div className="sidebar">
        <h1>Logo</h1>
        <nav>
          <a href="/home">Home</a>
          <a href="/upload">Image Upload</a>
        </nav>
      </div>
      <div className="main-content">
        <div className="logout-message">
          <h2>Logo</h2>
          <p>You have successfully logged out</p>
          <button onClick={handleBackToLogin}>Back to Login</button>
        </div>
      </div>
    </div>
  );
};

export default LogoutPage;
