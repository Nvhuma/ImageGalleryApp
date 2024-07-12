import React, { useState } from "react";
import "./Login.css";
import { Link, useNavigate } from "react-router-dom";
import '@fortawesome/fontawesome-free/css/all.min.css'; // Import Font Awesome
import axios from "axios";


function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [isUsernameActive, setIsUsernameActive] = useState(false);
  const [isPasswordActive, setIsPasswordActive] = useState(false);

  const navigate = useNavigate(); // Get the navigate function

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
      navigate("/home"); // Navigate to the home page
    } catch (error) {
      console.error("Login error:", error);
      alert("Login failed! Please check your username and/or password.");
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
                placeholder="     Enter Username"
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
                placeholder="    Enter Password"
                required
              />
              <i className="fas fa-lock"></i>
            </div>
          </div>
          <Link to="/ForgotPassword" className="forgot-password">Forgot Password?</Link>
          <button type="submit" className="login-button">Login</button>
        </form>
        <p className="register-link">
          New to this platform? <Link to="/register">Register Here</Link>
        </p>
      </div>
    </div>
  );
}

export default Login;
