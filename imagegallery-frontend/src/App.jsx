import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Login from './components/Login';
import Home from './components/home';
import Register from './components/Register';
import ResetPassword from './components/ResetPassword';
import '@fortawesome/fontawesome-free/css/all.min.css'; // Import Font Awesome
import ForgotPassword from './components/ForgotPassword';
import axios from "axios";


function App() {
     
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/Login" element={<Login />} />
          <Route path="/home" element={<Home />} />
          <Route path="/Register" element={<Register />} />
          <Route path="/reset-password" element={< ResetPassword />} />
          <Route path="/ForgotPassword" element={< ForgotPassword />} />
          {/* Add more routes here if needed */}
          <Route path="/" element={<Navigate replace to="/login" />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
