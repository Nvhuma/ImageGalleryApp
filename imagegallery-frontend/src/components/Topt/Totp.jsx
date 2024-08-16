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

    // Fetch credentials from localStorage
    const username = localStorage.getItem("username");
    const password = localStorage.getItem("password");

    // Log fetched credentials for debugging
    console.log("Fetched Credentials:", { username, password, totpCode });

    // If credentials or TOTP code is missing, display error
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

      // Log the full response for debugging
      console.log("TOTP verification response:", response.data);

      // Check if the response indicates success
      if (response.data && response.data.token) {
        alert("Login successful!");

        // Store the auth token if provided
        localStorage.setItem("authToken", response.data.token);

        navigate("/home");
      } else {
        // Log the exact data returned when the code is invalid
        console.error("Invalid TOTP code. Response data:", response.data);
        setErrorMessage("Invalid TOTP code. Please try again.");
      }
    } catch (error) {
      console.error("TOTP verification error:", error);

      // Handle specific errors
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
    <div className="totp-page">
      <div className="totp-container">
        <h1>TOTP Verification</h1>
        <form onSubmit={handleTotpSubmit}>
          <div className="input-group">
            <label htmlFor="totpCode" className="input-label">Authenticator Code</label>
            <div className="input-icon">
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
          <button type="submit" className="totp-button">Login</button>
        </form>
        {errorMessage && <p className="error-message">{errorMessage}</p>}
      </div>
    </div>
  );
}

export default Totp;
