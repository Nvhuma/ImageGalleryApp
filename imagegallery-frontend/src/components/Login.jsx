import React, { useState } from 'react';
import './Login.css';

function Login() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');

  const handleLogin = (e) => {
    e.preventDefault();
    // Add login logic here
    console.log('Login attempted with:', username, password);
  };

  return (
    <div className="App">
      <div className="login-container">
        <h1>Image Gallery App</h1>
        <h2>Log in</h2>
        <form onSubmit={handleLogin}>
          <div className="input-group">
            <label htmlFor="username">Username</label>
            <input
              type="text"
              id="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="Enter Username"
            />
          </div>
          <div className="input-group">
            <label htmlFor="password">Password</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter Password"
            />
          </div>
          <a href="#" className="forgot-password">Forgot Password?</a>
          <button type="submit" className="login-button">Login</button>
        </form>
        <p className="register-link">New to this platform? <a href="#">Register Here</a></p>
      </div>
	  
    </div>
  );
}

export default Login;