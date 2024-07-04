import React, { useState } from "react";
import "./Login.css";
import { Link } from "react-router-dom";
import '@fortawesome/fontawesome-free/css/all.min.css'; // Import Font Awesome
import axios from "axios";

function Login() {
  // Define state variables for username and password
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  // Handle the login form submission
  const handleLogin = async (e) => {
    e.preventDefault();
    console.log("Login attempted with:", username, password);

    try {
      // Make an HTTP POST request to the backend login endpoint
      const response = await axios.post("http://localhost:5263/api/Account/login", {
        UserName: username,
        Password: password
      });

      // Log the response from the server
      console.log("Login response:", response.data);

      // Optionally, store the token and user info in local storage or context
      localStorage.setItem("token", response.data.token);
      localStorage.setItem("user", JSON.stringify(response.data));

      // Redirect or perform further actions based on the login success
      alert("Login successful!");
    } catch (error) {
      // Handle errors such as incorrect login credentials
      console.error("Login error:", error);
      alert("Login failed! Please check your username and password.");
    }
  };

  return (
    <div className="App">
      <div className="login-container">
        <h1>Image Gallery App</h1>
        <h2>Log in</h2>
        <form onSubmit={handleLogin}>
          <div className="input-group">
            <div className="input-icon">
              <i className="fas fa-user"></i>
              <input
                type="text"
                id="username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="          Enter Username"
                required // Make the field required
              />
            </div>
          </div>
          <div className="input-group">
            <div className="input-icon">
              <i className="fas fa-lock"></i>
              <input
                type="password"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="          Enter Password"
                required // Make the field required
              />
            </div>
          </div>
          <a href="#" className="forgot-password">
            Forgot Password?
          </a>
          <button type="submit" className="login-button">
            Login
          </button>
        </form>
        <p className="register-link">
          New to this platform? <Link to="/register">Register Here</Link>
        </p>
      </div>
    </div>
  );
}

export default Login;
