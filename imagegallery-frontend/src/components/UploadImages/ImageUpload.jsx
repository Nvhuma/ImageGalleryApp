import React, { useState, useEffect } from "react";
import "./ImageUpload.css";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { FaHome, FaUpload, FaSignOutAlt } from "react-icons/fa";

const ImageUpload = () => {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [imageSelected, setImageSelected] = useState(null);
  const [uploadProgress, setUploadProgress] = useState(0);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [username, setUsername] = useState("");
  const [isDragging, setIsDragging] = useState(false); // State to manage drag-and-drop UI effects
  const navigate = useNavigate();

  // Load the username and token from local storage when the component mounts
  useEffect(() => {
    const storedUsername = localStorage.getItem("username");
    if (storedUsername) {
      setUsername(storedUsername);
    }
  }, []);

  // Handle file input change (when user selects a file)
  const handleFileChange = (e) => {
    const file = e.target.files[0];
    setImageSelected(file);
  };

  // Handle drag over event (when the file is dragged over the dropzone)
  const handleDragOver = (e) => {
    e.preventDefault();
    setIsDragging(true); // Update state to indicate dragging is in progress
  };

  // Handle drag leave event (when the file is dragged out of the dropzone)
  const handleDragLeave = () => {
    setIsDragging(false); // Update state to indicate dragging has stopped
  };

  // Handle drop event (when the file is dropped into the dropzone)
  const handleDrop = (e) => {
    e.preventDefault();
    const file = e.dataTransfer.files[0];
    setImageSelected(file);
    setIsDragging(false); // Reset dragging state
  };

  // Handle form submission (when user clicks "Submit")
  const handleUpload = async (e) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    // Validation checks
    if (!imageSelected) {
      alert("Please select an image file to upload.");
      setLoading(false);
      return;
    }

    if (title.length < 5) {
      alert("Title must be at least 5 characters long.");
      setLoading(false);
      return;
    }

    if (description.length < 5) {
      alert("Description must be at least 5 characters long.");
      setLoading(false);
      return;
    }

    try {
      const token = localStorage.getItem("authToken");

      if (!token) {
        alert("You are not authorized. Please log in.");
        navigate("/login");
        setLoading(false);
        return;
      }

      // Step 1: Upload the image to Cloudinary
      const cloudinaryFormData = new FormData();
      cloudinaryFormData.append("file", imageSelected);
      cloudinaryFormData.append("upload_preset", "a07u5tsj");

      const cloudinaryResult = await axios.post(
        "https://api.cloudinary.com/v1_1/decx7vro8/image/upload",
        cloudinaryFormData,
        {
          onUploadProgress: (progressEvent) => {
            const percentCompleted = Math.round(
              (progressEvent.loaded * 100) / progressEvent.total
            );
            setUploadProgress(percentCompleted); // Update upload progress state
          },
        }
      );

      const cloudinaryUrl = cloudinaryResult.data.secure_url;

      // Step 2: Send the image metadata and Cloudinary URL to your backend API
      const formData = new FormData();
      formData.append("Title", title);
      formData.append("Description", description);
      formData.append("ImageURL", cloudinaryUrl); // Use the Cloudinary URL
      formData.append("Image", imageSelected);

      const result = await axios.post(
        "http://localhost:5263/api/image",
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
            Authorization: `Bearer ${token}`,
          },
        }
      );

      // Save image to local storage
      const newImage = {
        title,
        description,
        imageURL: cloudinaryUrl,
      };

      // Retrieve existing images
      const storedImages = JSON.parse(localStorage.getItem(username)) || [];
      storedImages.push(newImage);
      localStorage.setItem(username, JSON.stringify(storedImages));

      console.log("Upload successful", result.data);
      alert("Image uploaded successfully!");

      // Reset the form fields after successful upload
      setTitle("");
      setDescription("");
      setImageSelected(null);
      setUploadProgress(0);
      setLoading(false);
    } catch (error) {
      console.error("Upload failed", error.response?.data || error.message);
      setError("Upload failed. Please try again.");
      setLoading(false);
    }
  };

  // Handle logout (clear user token and navigate to login)
  const handleLogout = async () => {
    try {
      await axios.post("http://localhost:5263/api/Account/logout");
      localStorage.removeItem("authToken");
      localStorage.removeItem("username");
      navigate("/login");
    } catch (error) {
      console.error("Logout failed", error.response?.data || error.message);
    }
  };

  return (
    <div className="image-upload-container">
      <div className="image-upload-sidebar">
        <div className="image-upload-logo-section">
          <img
            src="\src\assets\GAEL.svg"
            alt="Logo"
            className="image-upload-logo-img"
          />
        </div>
        <div className="image-upload-menu">
          <div
            className="image-upload-menu-item"
            onClick={() => navigate("/home")}
          >
            <FaHome />
            <span>Home</span>
          </div>
          <div className="image-upload-menu-item active">
            <FaUpload />
            <span>Image Upload</span>
          </div>
        </div>
        <div className="image-upload-logout" onClick={handleLogout}>
          <FaSignOutAlt />
          <span>Logout</span>
        </div>
      </div>
      <div className="image-upload-content">
        <div className="image-upload-header">
          <div className="nav">
            <span className="nav-item">Image Upload</span>
          </div>
          <div className="user">
            <span>{username || "User"}</span>
            <img src="/src/assets/user.png" alt="User Avatar" />
          </div>
        </div>
        <div className="image-upload-form-container">
          <h2>Image Upload</h2>
          <form className="image-upload-form" onSubmit={handleUpload}>
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
            <div
              className={`image-upload-dropzone ${
                isDragging ? "dragging" : ""
              }`} // Apply a special class when dragging
              onDragOver={handleDragOver}
              onDragLeave={handleDragLeave}
              onDrop={handleDrop}
              onClick={() => document.getElementById("fileInput").click()}
            >
              <input
                type="file"
                id="fileInput"
                style={{ display: "none" }}
                onChange={handleFileChange}
              />
              <p>
                {imageSelected
                  ? imageSelected.name
                  : "Drag and Drop Files or Click to Select"}
              </p>
            </div>
            {uploadProgress > 0 && (
              <p>Upload Progress: {uploadProgress}%</p>
            )}
            <button
              type="submit"
              className="image-upload-submit-button"
              disabled={loading}
            >
              {loading ? "Uploading..." : "Submit"}
            </button>
            {error && <p className="image-upload-error">{error}</p>}
          </form>
        </div>
      </div>
    </div>
  );
};

export default ImageUpload;
