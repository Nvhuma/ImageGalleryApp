// Home.jsx
import React, { useState, useEffect } from 'react';
import { FaHome, FaUpload, FaSignOutAlt, FaBook } from 'react-icons/fa';
import { MdSearch, MdFilterList } from 'react-icons/md';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import UpdateImageForm from './UpdateImageForm.jsx';

 // Importing the UpdateImageForm component
import './home.css';

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
  const navigate = useNavigate();

  useEffect(() => {
    const fetchImages = async () => {
      try {
        const response = await axios.get('http://localhost:5263/api/image');
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

      const userImagesFromStorage = JSON.parse(localStorage.getItem(storedUsername)) || [];
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
      await axios.post('http://localhost:5263/api/Account/logout');

      localStorage.removeItem('token');
      localStorage.removeItem('username');
      localStorage.removeItem('email');
      localStorage.removeItem('userId');

      navigate('/logout');
    } catch (error) {
      console.error('Logout failed', error);
    }
  };

  const handleResetPassword = async () => {
    try {
      const response = await axios.post('http://localhost:5263/api/Account/reset-password', {
        email: email,
      });

      const resetLink = response.data.resetLink;
      window.location.href = resetLink;
    } catch (error) {
      console.error('Error resetting password:', error);
      alert('Failed to request password reset. Please try again.');
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
    setImages(images.map(image => (image.imageId === updatedImage.imageId ? updatedImage : image)));
    setShowUpdateForm(false);
    setImageToUpdate(null);
  };

  const handleCancelUpdate = () => {
    setShowUpdateForm(false);
    setImageToUpdate(null);
  };

  const handleDeleteImage = async (imageId, imageUserId) => {
    const storedUserId = localStorage.getItem('userId');

    if (storedUserId !== imageUserId) {
      alert("You can only delete images that you have uploaded.");
      return;
    }

    try {
      await axios.delete(`http://localhost:5263/api/image/${imageId}`, {
        params: { loggedInUserId: storedUserId }
      });

      if (viewLibrary) {
        setUserImages(userImages.filter(image => image.imageId !== imageId));
      } else {
        setImages(images.filter(image => image.imageId !== imageId));
      }

      alert("Image deleted successfully!");
    } catch (error) {
      console.error("Error deleting image:", error);
      alert('Failed to delete the image. Please try again.');
    }
  };

  const handleImageClick = (image) => {
    setSelectedImage(image);
  };

  const handleCloseImage = () => {
    setSelectedImage(null);
  };

  const imagesPerPage = 6;
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
          <div className={`menu-item ${!viewLibrary ? 'active' : ''}`} onClick={() => setViewLibrary(false)}>
            <FaHome />
            <span>Home</span>
          </div>
          <div className="menu-item" onClick={() => navigate('/ImageUpload')}>
            <FaUpload />
            <span>Image Upload</span>
          </div>
          <div className={`menu-item ${viewLibrary ? 'active' : ''}`} onClick={() => setViewLibrary(true)}>
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
          <h2>{viewLibrary ? 'My Library' : 'Home'}</h2>
          <div className="user" onClick={toggleDropdown}>
            <span>{username || "User"}</span>
            <img src="/src/assets/user.png" alt="User Avatar" />
            <div className={`dropdown ${dropdownOpen ? 'show' : ''}`}>
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
          <div className="single-image-view">
            <button className="close-button" onClick={handleCloseImage}>×</button>
            <img src={selectedImage.imageURL} alt={selectedImage.title} className="single-image" />
            <div className="image-details">
              <h3>{selectedImage.title}</h3>
              <p>{selectedImage.description}</p>
            </div>
          </div>
        ) : (
          <div className="image-grid">
            {loading ? (
              <p>Loading...</p>
            ) : viewLibrary ? (
              filteredUserImages.map((image, index) => (
                <div key={image.imageId || index} className="image-item" onClick={() => handleImageClick(image)}>
                  {image.imageURL ? (
                    <img src={image.imageURL} alt={image.title} />
                  ) : (
                    <div className="placeholder">Image not available</div>
                  )}
                  <div className="image-actions">
                    <button onClick={(e) => { e.stopPropagation(); handleDeleteImage(image.imageId, image.userId) }}>Delete</button>
                    <button onClick={(e) => { e.stopPropagation(); handleUpdateImageClick(image) }}>Update</button>
                  </div>
                </div>
              ))
            ) : (
              filteredImages.map((image, index) => (
                <div key={image.imageId || index} className="image-item" onClick={() => handleImageClick(image)}>
                  {image.imageURL ? (
                    <img src={image.imageURL} alt={image.title} />
                  ) : (
                    <div className="placeholder">Image not available</div>
                  )}
                  <div className="item-details">
                    <h4 className="name">{image.title}</h4>
                    <p className="description">{image.description}</p>
                  </div>
                  <div className="image-actions">
                    {image.userId === localStorage.getItem('userId') && (
                      <>
                        <button onClick={(e) => { e.stopPropagation(); handleDeleteImage(image.imageId, image.userId) }}>Delete</button>
                        <button onClick={(e) => { e.stopPropagation(); handleUpdateImageClick(image) }}>Update</button>
                      </>
                    )}
                  </div>
                </div>
              ))
            )}
          </div>
        )}
        {viewLibrary ? (
          <div className="pagination">
            {Array.from({ length: totalLibraryPages }, (_, index) => (
              <button
                key={index + 1}
                onClick={() => handleLibraryPageChange(index + 1)}
                className={libraryPage === index + 1 ? 'active' : ''}
              >
                {index + 1}
              </button>
            ))}
          </div>
        ) : (
          <div className="pagination">
            {Array.from({ length: totalPages }, (_, index) => (
              <button
                key={index + 1}
                onClick={() => handlePageChange(index + 1)}
                className={currentPage === index + 1 ? 'active' : ''}
              >
                {index + 1}
              </button>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default Home;
