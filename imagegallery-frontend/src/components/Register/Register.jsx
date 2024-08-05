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

  const [error, setError] = useState(null);
  const [qrCode, setQrCode] = useState(''); // State for QR Code
  const [base32Secret, setBase32Secret] = useState(''); // State for Base32 Secret
  const [isQrDisplayed, setIsQrDisplayed] = useState(false); // State to manage QR display
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

    const payload = {
      userName: formData.fullName,   // This field maps to the "userName" in the backend
      names: formData.fullName,      // This field maps to the "names" in the backend
      emailAddress: formData.email,  // This field maps to the "emailAddress" in the backend
      password: formData.password    // This field maps to the "password" in the backend
    };

    try {
      const response = await axios.post('http://localhost:5263/api/Account/register', payload, {
        headers: {
          'Content-Type': 'application/json'
        }
      });
      console.log('Registration successful:', response.data);

      // Assuming the backend returns the QR code and base32 secret
      if (response.data.qrCode && response.data.base32Secret) {
        setQrCode(response.data.qrCode);
        setBase32Secret(response.data.base32Secret);
        setIsQrDisplayed(true);
        console.log('QR Code and Base32 Secret set:', response.data.qrCode, response.data.base32Secret);
      } else {
        setError('QR Code or Base32 Secret missing in response.');
        console.error('QR Code or Base32 Secret missing in response:', response.data);
      }
    } catch (error) {
      console.error('Error registering:', error);
      if (error.response) {
        console.error('Error response data:', error.response.data); // Loging the error response data
        setError(error.response.data.Message || 'Registration failed');
      } else if (error.request) {
        console.error('Error request:', error.request); // Logging the error request
        setError('Network error. Please try again later.');
      } else {
        console.error('Error message:', error.message); // Logging the error message
        setError(`Registration failed: ${error.message}`);
      }
    }
  };

  const handleNavigateToLogin = () => {
    navigate('/login');
  };

  return (
    <div className="register-container">
      <div className="form-section">
        <h1>Register Profile</h1>
        {error && <p className="error-message">{error}</p>}
        {!isQrDisplayed ? (
          <form onSubmit={handleSubmit}>
            <div className="registration-input-group">
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
            <div className="registration-input-group">
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
            <div className="registration-input-group">
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
            <div className="registration-input-group">
              <label htmlFor="confirmPassword">Confirm Password</label>
              <input 
                type="password" 
                id="confirmPassword" 
                name="confirmPassword" 
                value={formData.confirmPassword}
                placeholder="Confirm Password" 
                onChange={handleChange} 
              />
            </div>
            <button type="submit" className="register-button">Register</button>
          </form>
        ) : (
          <div className="qr-code-container">
            <h3>Scan the QR Code with your authenticator app</h3>
            <img src={`data:image/png;base64,${qrCode}`} alt="QR Code" />
            <p>Secret Key: {base32Secret}</p>
            <button className="register-button" onClick={handleNavigateToLogin}>Continue to Login</button>
          </div>
          //hanlding the image generation from the backend data 
        )} 
        {!isQrDisplayed && (
          <>
            <div className="social-login">
              <p>or</p>
              <button className="google-btn"><img src={googleIcon} alt="Google" /> Sign in with Google</button>
              <button className="facebook-btn"><img src={facebookIcon} alt="Facebook" /> Sign in with Facebook</button>
            </div>
            <p className="login-link">Already have an account? <Link to="/login">Login here</Link></p>
          </>
        )}
      </div>
      <div className="image-section"></div>
    </div>
  );
}

export default Register;
