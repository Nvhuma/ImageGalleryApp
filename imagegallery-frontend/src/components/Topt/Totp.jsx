import React, { useState } from "react";
import "./Totp.css";
import { useNavigate } from "react-router-dom";
import axios from "axios";

function Totp() {
  const [totpCode, setTotpCode] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const navigate = useNavigate();

  const handleTotpSubmit = async (e) => {
    e.preventDefault();

    const username = localStorage.getItem("username");
    const password = localStorage.getItem("password");

    console.log("Fetched Credentials:", { username, password, totpCode });

    if (!username || !password || !totpCode) {
      console.error("Username, password, or TOTP code is missing.");
      setErrorMessage("Username, password, or TOTP code is missing. Please log in again.");
      return;
    }

    try {
      const response = await axios.post(
        "http://localhost:5263/api/Account/totp",
        {
          userName: username,
          password: password,
          totpCode: totpCode
        },
        {
          headers: {
            'Content-Type': 'application/json'
          }
        }
      );

      console.log("TOTP verification response:", response.data);

      if (response.data && response.data.token) {
        alert("Login successful!");

        // Store the entire response in local storage
        localStorage.setItem("authToken", response.data.token);
        localStorage.setItem("userId", response.data.userId); // Store the userId
        localStorage.setItem("email", response.data.emailAddress);
        localStorage.setItem("username", response.data.userName);
        
        // Optionally, you can save the entire response object if needed
        localStorage.setItem("totpResponse", JSON.stringify(response.data));

        // Navigate to the home page
        navigate("/home");
      } else {
        console.error("Invalid TOTP code. Response data:", response.data);
        setErrorMessage("Invalid TOTP code. Please try again.");
      }
    } catch (error) {
      console.error("TOTP verification error:", error);

      if (error.response && error.response.status === 401) {
        setErrorMessage("Unauthorized: Check your credentials and TOTP code.");
      } else if (error.response && error.response.data && error.response.data.error) {
        setErrorMessage(`Error: ${error.response.data.error}`);
      } else {
        setErrorMessage("An unexpected error occurred. Please try again later.");
      }
    }
  };

  return (
    <div className="totp-auth-page">
      <div className="totp-auth-container">
        <h1>Image Gallery App</h1>
        <p>Your login is protected with an authenticator app. Enter your authenticator code below.</p>
        <form onSubmit={handleTotpSubmit}>
          <div className="totp-auth-input-group">
            <label htmlFor="totpCode" className="totp-auth-input-label">Authenticator Code</label>
            <div className="totp-auth-input-icon">
              <input
                type="text"
                id="totpCode"
                value={totpCode}
                onChange={(e) => setTotpCode(e.target.value)}
                placeholder="Enter TOTP Code"
                required
              />
              <i className="fas fa-key"></i>
            </div>
          </div>
          <button type="submit" className="totp-auth-button">Login</button>
        </form>
        {errorMessage && <p className="totp-auth-error-message">{errorMessage}</p>}
      </div>
    </div>
  );
}

export default Totp;
