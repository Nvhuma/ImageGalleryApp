import React, { useState } from 'react';
import './Register.css';
import googleIcon from '/src/assets/icons8.png';
import facebookIcon from '/src/assets/icons7.png';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';



function Register() {
  const [formData, setFormData] = useState({
    fullName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });

  const [error, setError] = useState(null); // State to handle errors, initialized to null
  const navigate = useNavigate();

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prevState => ({
      ...prevState,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    // Basic validation
    if (!formData.fullName || !formData.email || !formData.password || !formData.confirmPassword) {
      setError('All fields are required.');
      return;
    }

    if (formData.password !== formData.confirmPassword) {
      setError('Passwords do not match.');
      return;
    }

    // Prepare payload to match backend's RegisterDto structure
    const payload = {
      UserName: formData.fullName,
      EmailAddress: formData.email,
      Names: formData.fullName,
      Password: formData.password,
      ConfirmPassword: formData.confirmPassword
    };

    try {
      const response = await axios.post('http://localhost:5263/api/Account/register', payload);
      console.log('Registration successful:', response.data);
      navigate('/login'); // Navigate to login page on success
    } catch (error) {
      console.error('Error registering:', error);
      if (error.response) {
        setError(`Registration failed: ${error.response.data.message || error.response.statusText}`);
      } else if (error.request) {
        setError('Registration failed: No response from server. Please try again later.');
      } else {
        setError(`Registration failed: ${error.message}`);
      }
    }
  };

  return (
    <div className="register-container">
      <div className="form-section">
        <h1>Register Profile</h1>
        <p>Lorem ipsum dolor sit amet consectetur sit amet consectetur.</p>
        {error && <p className="error-message">{error}</p>} {/* Display error message */}
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
        <div className="social-login">
          <p>or</p>
          <button className="google-btn"><img src={googleIcon} alt="Google" /> Sign in with Google</button>
          <button className="facebook-btn"><img src={facebookIcon} alt="Facebook" /> Sign in with Facebook</button>
        </div>
        <p className="login-link">Already have an account? <Link to="/login">Login here</Link></p>
      </div>
      <div className="image-section"></div>
    </div>
  );
}

export default Register;
