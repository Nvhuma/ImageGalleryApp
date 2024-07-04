// Register.js

import React, { useState } from 'react';
import './Register.css';
import googleIcon from '/src/assets/icons8.png';
import facebookIcon from '/src/assets/icons7.png';
import { Link } from 'react-router-dom';

function Register() {
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prevState => ({
      ...prevState,
      [name]: value
    }));
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log('Form submitted:', formData);
  };

  return (
    <div className="register-container">
      <div className="form-section">
        <h1>Register Profile</h1>
        <p>Lorem ipsum dolor sit .</p>
        <form onSubmit={handleSubmit}>
          <div className="input-group">
            <label htmlFor="fullName">Full Name</label>
            <input 
              type="text" 
              id="fullName" 
              name="fullName" 
              value={formData.fullName}
              placeholder="Enter Name" 
              onChange={handleChange} 
            />
          </div>
          <div className="input-group">
            <label htmlFor="email">Email Address</label>
            <input 
              type="email" 
              id="email" 
              name="email" 
              value={formData.email}
              placeholder="Enter Email" 
              onChange={handleChange} 
            />
          </div>
          <div className="input-group">
            <label htmlFor="password">Password</label>
            <input 
              type="password" 
              id="password" 
              name="password" 
              value={formData.password}
              placeholder="Enter Password" 
              onChange={handleChange} 
            />
          </div>
          <div className="input-group">
            <label htmlFor="confirmPassword">Confirm Password</label>
            <input 
              type="password" 
              id="confirmPassword" 
              name="confirmPassword" 
              value={formData.confirmPassword}
              placeholder="Enter Password" 
              onChange={handleChange} 
            />
          </div>
          <button type="submit" className="register-button">Register</button>
        </form>
        <p className="login-link">Already have an account? <Link to="/login">Login here</Link></p>
        <div className="social-login">
          <p>or</p>
          <button className="google-btn"><img src={googleIcon} alt="Google" /> Sign in with Google</button>
          <button className="facebook-btn"><img src={facebookIcon} alt="Facebook" /> Sign in with Facebook</button>
        </div>
      </div>
    </div>
  );
}

export default Register;
