import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Login from './components/Login/Login';
import Home from './components/Home/home';
import Register from './components/Register/Register';
import ResetPassword from './components/ResetPassword/ResetPassword';
import '@fortawesome/fontawesome-free/css/all.min.css'; // Import Font Awesome
import ForgotPassword from './components/ForgotPassword/ForgotPassword';
import Logout from './components/Logout/logout';
import axios from "axios";
import ImageUpload from './components/UploadImages/ImageUpload';
import PasswordChanged from './components/PasswordChanged/PasswordChanged';
import Totp from './components/Topt/Totp';


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
          <Route path = "/ImageUpload" element ={<ImageUpload/>} />
          <Route path="/logout" element={<Logout />} />
          <Route path="/PasswordChanged" element={< PasswordChanged />} />
          <Route path="/Totp" element={< Totp />} />
          {/* i can  more routes here  */}
          <Route path="/" element={<Navigate replace to="/login" />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
