import React, { useState, useEffect } from "react";
import "./ImageUpload.css";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { FaHome, FaUpload, FaSignOutAlt } from "react-icons/fa";

const ImageUpload = () => {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [username, setUsername] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    // Get username from localStorage inside useEffect
    const storedUsername = localStorage.getItem("username");
    if (storedUsername) {
      setUsername(storedUsername);
    }
  }, []);

  const handleUpload = (e) => {
    e.preventDefault();
    // Handle the upload logic here
  };

  const handleLogout = async () => {
    try {
      await axios.post("http://localhost:5263/api/Account/logout");
      navigate("/logout");
    } catch (error) {
      console.error("Logout failed", error);
    }
  };

  return (
    <div className="container">
      <div className="Sidebar">
        <div className="logo-section">
          <img src="\src\assets\GAEL.svg" alt="Logo" className="logo-img" />
        </div>
        <div className="menu">
          <div className="menu-item" onClick={() => navigate("/home")}>
            <FaHome />
            <span>Home</span>
          </div>
          <div className="menu-item active">
            <FaUpload />
            <span>Image Upload</span>
          </div>
        </div>
        <div className="logout" onClick={handleLogout}>
          <FaSignOutAlt />
          <span>Logout</span>
        </div>
      </div>
      <div className="content-upload">
        <div className="header">
          <div className="nav">
            <span className="nav-item">Image Upload</span>
          </div>
          <div className="user">
            <span>{username || "User"}</span>
            <img src="/src/assets/user.png" alt="User Avatar" />
          </div>
        </div>
        <div className="upload-container">
          <h2>Image Upload</h2>
          <form className="upload-form" onSubmit={handleUpload}>
            <input
              type="text"
              placeholder="Image Title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
            />
            <textarea
              placeholder="Image Description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />
            <div className="upload-dropzone">
              <p>Drag and Drop Files</p>
              <p>or</p>
              <button type="button" className="upload-button">
                Upload
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default ImageUpload;
