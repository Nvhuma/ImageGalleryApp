import React, { useState, useEffect } from "react";
import { FaHome, FaUpload, FaSignOutAlt, FaBook } from "react-icons/fa";
import { MdSearch, MdFilterList } from "react-icons/md";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import UpdateImageForm from "./UpdateImageForm.jsx";
import Comments from "./Comments.jsx";
import "./home.css";

const Home = () => {
  const [currentPage, setCurrentPage] = useState(1);
  const [libraryPage, setLibraryPage] = useState(1);
  const [images, setImages] = useState([]);
  const [userImages, setUserImages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [viewLibrary, setViewLibrary] = useState(false);
  const [dropdownOpen, setDropdownOpen] = useState(false);
  const [selectedImage, setSelectedImage] = useState(null);
  const [showUpdateForm, setShowUpdateForm] = useState(false);
  const [imageToUpdate, setImageToUpdate] = useState(null);
  const [showComments, setShowComments] = useState(false); // state for comments visibility
  const [imageForComments, setImageForComments] = useState(null); //  state for selected image for comments
  const navigate = useNavigate();

  useEffect(() => {
    const fetchImages = async () => {
      try {
        const response = await axios.get("http://localhost:5263/api/image");
        setImages(response.data);
      } catch (error) {
        console.error("Error fetching images:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchImages();

    const storedUsername = localStorage.getItem("username");
    const storedEmail = localStorage.getItem("email");

    if (storedUsername) {
      setUsername(storedUsername);
      setEmail(storedEmail || "");

      const userImagesFromStorage =
        JSON.parse(localStorage.getItem(storedUsername)) || [];
      setUserImages(userImagesFromStorage);
    } else {
      navigate("/login");
    }
  }, [navigate]);

  const toggleDropdown = () => {
    setDropdownOpen(!dropdownOpen);
  };

  const handleLogout = async () => {
    try {
      await axios.post("http://localhost:5263/api/Account/logout");

      localStorage.removeItem("token");
      localStorage.removeItem("username");
      localStorage.removeItem("email");
      localStorage.removeItem("userId");

      navigate("/logout");
    } catch (error) {
      console.error("Logout failed", error);
    }
  };

  const handleResetPassword = async () => {
    try {
      const email = localStorage.getItem("email");

      if (!email) {
        alert("Email not found. Please log in again.");
        return;
      }

      const response = await axios.post(
        "http://localhost:5263/api/Account/forgot-password",
        { email }
      );
      console.log("Response received:", response.data);
      const { resetLink } = response.data;

      if (resetLink) {
        console.log("Navigating to reset link:", resetLink);
        window.location.href = resetLink;
      } else {
        alert("Reset link not received from the server. Please try again.");
      }
    } catch (error) {
      console.error("Error resetting password:", error);
      alert("Failed to request password reset. Please try again.");
    }
  };

  const handlePageChange = (page) => {
    setCurrentPage(page);
  };

  const handleLibraryPageChange = (page) => {
    setLibraryPage(page);
  };

  const handleUpdateImageClick = (image) => {
    setImageToUpdate(image);
    setShowUpdateForm(true);
  };

  const handleUpdateSuccess = (updatedImage) => {
    setImages(
      images.map((image) =>
        image.imageId === updatedImage.imageId ? updatedImage : image
      )
    );
    setShowUpdateForm(false);
    setImageToUpdate(null);
  };

  const handleCancelUpdate = () => {
    setShowUpdateForm(false);
    setImageToUpdate(null);
  };

  const handleDeleteImage = async (imageId, imageUserId, imageURL) => {
    const storedUserId = localStorage.getItem("userId");

    if (storedUserId !== imageUserId) {
      alert("You can only delete images that you have uploaded.");
      return;
    }

    try {
      await axios.delete(`http://localhost:5263/api/image/${imageId}`, {
        params: { loggedInUserId: storedUserId },
      });

      if (viewLibrary) {
        setUserImages((prevImages) =>
          prevImages.filter((image) => image.imageURL !== imageURL)
        );
      } else {
        setImages((prevImages) =>
          prevImages.filter((image) => image.imageId !== imageId)
        );
      }

      alert("Image deleted successfully!");
    } catch (error) {
      console.error("Error deleting image:", error);
      alert("Failed to delete the image. Please try again.");
    }
  };

  const handleImageClick = (image) => {
    setSelectedImage(image);
  };

  const handleCloseImage = () => {
    setSelectedImage(null);
  };

  const handleShowComments = (image) => {
    setImageForComments(image);
    setShowComments(true);
  };

  const handleCloseComments = () => {
    setShowComments(false);
    setImageForComments(null);
  };

  const imagesPerPage = 4;
  const totalPages = Math.ceil(images.length / imagesPerPage);
  const totalLibraryPages = Math.ceil(userImages.length / imagesPerPage);

  const filteredImages = images.slice(
    (currentPage - 1) * imagesPerPage,
    currentPage * imagesPerPage
  );

  const filteredUserImages = userImages.slice(
    (libraryPage - 1) * imagesPerPage,
    libraryPage * imagesPerPage
  );

  return (
    <div className="container">
      <div className="sidebar">
        <div className="logo-section">
          <img src="/src/assets/GAEL.svg" alt="Logo" className="logo-img" />
        </div>
        <div className="menu">
          <div
            className={`menu-item ${!viewLibrary ? "active" : ""}`}
            onClick={() => setViewLibrary(false)}
          >
            <FaHome />
            <span>Home</span>
          </div>
          <div className="menu-item" onClick={() => navigate("/ImageUpload")}>
            <FaUpload />
            <span>Image Upload</span>
          </div>
          <div
            className={`menu-item ${viewLibrary ? "active" : ""}`}
            onClick={() => setViewLibrary(true)}
          >
            <FaBook />
            <span>My Library</span>
          </div>
        </div>
        <div className="logout" onClick={handleLogout}>
          <FaSignOutAlt />
          <span>Logout</span>
        </div>
      </div>
      <div className="content">
        <div className="header">
          <h2>{viewLibrary ? "My Library" : "Home"}</h2>
          <div className="user" onClick={toggleDropdown}>
            <span>{username || "User"}</span>
            <img src="/src/assets/user.png" alt="User Avatar" />
            <div className={`dropdown ${dropdownOpen ? "show" : ""}`}>
              <p>{email || "No email provided"}</p>
              <button onClick={handleResetPassword}>Reset Password</button>
            </div>
          </div>
        </div>
        <div className="search-filter">
          <div className="search-bar">
            <MdSearch />
            <input type="text" placeholder="Search for..." />
          </div>
          <button className="filter-button">
            <MdFilterList />
            Filters
          </button>
        </div>
        {showUpdateForm && imageToUpdate ? (
          <UpdateImageForm
            image={imageToUpdate}
            onCancel={handleCancelUpdate}
            onUpdateSuccess={handleUpdateSuccess}
          />
        ) : selectedImage ? (
          <div className="modal-overlay" onClick={handleCloseImage}>
            <div className="modal-content" onClick={(e) => e.stopPropagation()}>
              <img
                src={selectedImage.imageURL}
                className="modal-image"
                alt={selectedImage.title}
              />
              <button className="modal-close" onClick={handleCloseImage}>
                &times;
              </button>
              <div className="tags-container">
                {selectedImage.tags?.map((tag, index) => (
                  <span key={index} className="tag">
                    #{tag}
                  </span>
                ))}
              </div>
              <div className="modal-description">
                <div className="modal-title">
                  {selectedImage.title}
                  <i className="fas fa-tag"></i>
                </div>
                <p className="modal-text">{selectedImage.description}</p>
              </div>
              <div className="icon-container">
                <i className="fas fa-heart"></i>
                <i
                  className="fas fa-comment"
                  onClick={() => handleShowComments(selectedImage)}
                ></i>
              </div>
            </div>
          </div>
        ) : (
          <div className="image-grid">
            {loading ? (
              <p>Loading...</p>
            ) : viewLibrary ? (
              filteredUserImages.map((image, index) => (
                <div
                  key={image.imageId || index}
                  className="image-item"
                  onClick={() => handleImageClick(image)}
                >
                  {image.imageURL ? (
                    <img src={image.imageURL} alt={image.title} />
                  ) : (
                    <div className="placeholder">Image not available</div>
                  )}
                  {image.userId === localStorage.getItem("userId") && (
                    <div className="image-actions">
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          handleDeleteImage(
                            image.imageId,
                            image.userId,
                            image.imageURL
                          );
                        }}
                      >
                        Delete
                      </button>
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          handleUpdateImageClick(image);
                        }}
                      >
                        Update
                      </button>
                    </div>
                  )}
                </div>
              ))
            ) : (
              filteredImages.map((image, index) => (
                <div
                  key={image.imageId || index}
                  className="image-item"
                  onClick={() => handleImageClick(image)}
                >
                  {image.imageURL ? (
                    <img src={image.imageURL} alt={image.title} />
                  ) : (
                    <div className="placeholder">Image not available</div>
                  )}
                  <div className="item-details">
                    <h4 className="name">{image.title}</h4>
                    <p className="description">{image.description}</p>
                  </div>
                  {image.userId === localStorage.getItem("userId") && (
                    <div className="image-actions">
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          handleDeleteImage(
                            image.imageId,
                            image.userId,
                            image.imageURL
                          );
                        }}
                      >
                        Delete
                      </button>
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          handleUpdateImageClick(image);
                        }}
                      >
                        Update
                      </button>
                    </div>
                  )}
                </div>
              ))
            )}
          </div>
        )}
        <div className="pagination">
          {Array.from({
            length: viewLibrary ? totalLibraryPages : totalPages,
          }).map((_, index) => (
            <button
              key={index}
              className={`page-button ${
                index + 1 === (viewLibrary ? libraryPage : currentPage)
                  ? "active"
                  : ""
              }`}
              onClick={() =>
                viewLibrary
                  ? handleLibraryPageChange(index + 1)
                  : handlePageChange(index + 1)
              }
            >
              {index + 1}
            </button>
          ))}
        </div>
        {showComments && imageForComments && (
          <div className="comments-section">
            <Comments
              imageId={imageForComments.imageId}
              onClose={handleCloseComments}
            />
          </div>
        )}
      </div>
    </div>
  );
};

export default Home;
