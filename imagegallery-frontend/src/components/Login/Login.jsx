import React, { useState } from "react";
import "./Login.css";
import { Link, useNavigate } from "react-router-dom";
import '@fortawesome/fontawesome-free/css/all.min.css';
import axios from "axios";

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [isUsernameActive, setIsUsernameActive] = useState(false);
  const [isPasswordActive, setIsPasswordActive] = useState(false);

  const navigate = useNavigate();

  const handleLogin = async (e) => {
    e.preventDefault();
    console.log("Login attempted with:", username, password);

    try {
      const response = await axios.post("http://localhost:5263/api/Account/login", {
        UserName: username,
        Password: password
      });

      console.log("Login response:", response.data);

      localStorage.setItem("token", response.data.token);
      localStorage.setItem("user", JSON.stringify(response.data));

      alert("Login successful!");
      navigate("/home");
    } catch (error) {
      console.error("Login error:", error);

      if (error.response && error.response.status === 401) {
        if (error.response.data.Message && error.response.data.Message.includes("Account is locked")) {
          const remainingTime = Math.ceil(error.response.data.RemainingLockoutTime / 3);
          setErrorMessage(`${error.response.data.Message} You can try again in ${remainingTime} minute${remainingTime > 1 ? 's' : ''}.`);
        } else {
          setErrorMessage(error.response.data);
        }
      } else {
        setErrorMessage("An unexpected error occurred. Please try again later.");
      }
    }
  };

  return (
    <div className="login-page">
      <div className="login-container">
        <h1>Image Gallery App</h1>
        <h2>Login</h2>
        <form onSubmit={handleLogin}>
          <div className="input-group">
            <label htmlFor="username" className="input-label">Username</label>
            <div className={`input-icon ${isUsernameActive ? 'active' : ''}`}>
              <input
                type="text"
                id="username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                onFocus={() => setIsUsernameActive(true)}
                onBlur={() => setIsUsernameActive(username !== "")}
                placeholder="Enter Username"
                required
              />
              <i className="fas fa-user"></i>
            </div>
          </div>
          <div className="input-group">
            <label htmlFor="password" className="input-label">Password</label>
            <div className={`input-icon ${isPasswordActive ? 'active' : ''}`}>
              <input
                type="password"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                onFocus={() => setIsPasswordActive(true)}
                onBlur={() => setIsPasswordActive(password !== "")}
                placeholder="Enter Password"
                required
              />
              <i className="fas fa-lock"></i>
            </div>
          </div>
          <Link to="/ForgotPassword" className="forgot-password">Forgot Password?</Link>
          <button type="submit" className="login-button">Login</button>
        </form>
        {errorMessage && <p className="error-message">{errorMessage}</p>}
        <p className="register-link">
          New to this platform? <Link to="/register">Register Here</Link>
        </p>
      </div>
    </div>
  );
}

export default Login;