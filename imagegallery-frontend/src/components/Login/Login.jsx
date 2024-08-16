import React, { useState, useEffect } from "react";
import "./Login.css";
import { Link, useNavigate } from "react-router-dom";
import '@fortawesome/fontawesome-free/css/all.min.css';
import axios from "axios";
import VisibilityIcon from '@mui/icons-material/Visibility';
import VisibilityOffIcon from '@mui/icons-material/VisibilityOff';

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [rememberMe, setRememberMe] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [isUsernameActive, setIsUsernameActive] = useState(false);
  const [isPasswordActive, setIsPasswordActive] = useState(false);
  const [showPassword, setShowPassword] = useState(false);

  const navigate = useNavigate();

  useEffect(() => {
    const savedUsername = localStorage.getItem("rememberedUsername");
    const savedPassword = localStorage.getItem("rememberedPassword");
    if (savedUsername) setUsername(savedUsername);
    if (savedPassword) setPassword(savedPassword);
    if (savedUsername || savedPassword) setRememberMe(true);
  }, []);

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

      if (rememberMe) {
        localStorage.setItem("rememberedUsername", username);
        localStorage.setItem("rememberedPassword", password);
      } else {
        localStorage.removeItem("rememberedUsername");
        localStorage.removeItem("rememberedPassword");
      }

      localStorage.setItem("username", username);
      localStorage.setItem("password", password);
      localStorage.setItem("email", response.data.emailAddress); // Ensure email is stored

      alert("Login successful! Please enter your TOTP code.");
      navigate("/totp");
    } catch (error) {
      console.error("Login error:", error);

      if (error.response && error.response.status === 401) {
        const message = error.response.data.Message || error.response.data.message;

        if (message) {
          if (message.includes("Account is locked")) {
            const remainingTime = Math.ceil(error.response.data.remainingLockoutTime);
            setErrorMessage(`${message} You can try again in ${remainingTime} Seconds`);
          } else {
            setErrorMessage(message);
          }
        } else {
          setErrorMessage("Invalid username or password.");
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
                type={showPassword ? "text" : "password"}
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                onFocus={() => setIsPasswordActive(true)}
                onBlur={() => setIsPasswordActive(password !== "")}
                placeholder="Enter Password"
                required
              />
              <span
                className="password-toggle-icon"
                onClick={() => setShowPassword(!showPassword)}
              >
                {showPassword ? <VisibilityOffIcon /> : <VisibilityIcon />}
              </span>
              <i className="fas fa-lock"></i>
            </div>
          </div>
          <div className="remember-me">
            <input
              type="checkbox"
              id="rememberMe"
              checked={rememberMe}
              onChange={(e) => setRememberMe(e.target.checked)}
            />
            <label htmlFor="rememberMe">Remember Me</label>
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
