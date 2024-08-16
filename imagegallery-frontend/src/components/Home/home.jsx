import React, { useState, useEffect } from 'react';
import { FaHome, FaUpload, FaSignOutAlt, FaBook } from 'react-icons/fa';
import { MdSearch, MdFilterList } from 'react-icons/md';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import './home.css';

const Home = () => {
  const [currentPage, setCurrentPage] = useState(1);
  const [libraryPage, setLibraryPage] = useState(1);
  const [images, setImages] = useState([]);
  const [userImages, setUserImages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [username, setUsername] = useState("");
  const [viewLibrary, setViewLibrary] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    // Fetch all images from the API
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

    // Get username from localStorage
    const storedUsername = localStorage.getItem("username");
    if (storedUsername) {
      setUsername(storedUsername);

      // Fetch user-specific images from localStorage
      const userImagesFromStorage = JSON.parse(localStorage.getItem(storedUsername)) || [];
      setUserImages(userImagesFromStorage);
    } else {
      navigate("/login");
    }
  }, [navigate]);

  const handlePageChange = (page) => {
    setCurrentPage(page);
  };

  const handleLibraryPageChange = (page) => {
    setLibraryPage(page);
  };

  const handleLogout = async () => {
    try {
      await axios.post('http://localhost:5263/api/Account/logout');
      localStorage.removeItem('token');
      localStorage.removeItem('username');
      navigate('/logout');
    } catch (error) {
      console.error('Logout failed', error);
    }
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
          <div className="user">
            <span>{username || "User"}</span>
            <img src="/src/assets/user.png" alt="User Avatar" />
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
        <div className="image-grid">
          {loading ? (
            <p>Loading...</p>
          ) : viewLibrary ? (
            filteredUserImages.map((image, index) => (
              <div key={index} className="image-item">
                {image.imageURL ? (
                  <img src={image.imageURL} alt={image.title} />
                ) : (
                  <div className="placeholder">Image not available</div>
                )}
                <div className="item-details">
                  <h4 className="name">{image.title}</h4>
                  <p className="description">{image.description}</p>
                </div>
              </div>
            ))
          ) : (
            filteredImages.map((image, index) => (
              <div key={image.id || index} className="image-item">
                {image.imageURL ? (
                  <img src={image.imageURL} alt={image.title} />
                ) : (
                  <div className="placeholder">Image not available</div>
                )}
                <div className="item-details">
                  <h4 className="name">{image.title}</h4>
                  <p className="description">{image.description}</p>
                </div>
              </div>
            ))
          )}
        </div>
        <div className="pagination">
          {viewLibrary ? (
            <>
              <button onClick={() => handleLibraryPageChange(libraryPage - 1)} disabled={libraryPage === 1}>&lt;</button>
              {[...Array(totalLibraryPages)].map((_, index) => (
                <button
                  key={index}
                  onClick={() => handleLibraryPageChange(index + 1)}
                  className={libraryPage === index + 1 ? 'active' : ''}
                >
                  {index + 1}
                </button>
              ))}
              <button onClick={() => handleLibraryPageChange(libraryPage + 1)} disabled={libraryPage === totalLibraryPages}>&gt;</button>
            </>
          ) : (
            <>
              <button onClick={() => handlePageChange(currentPage - 1)} disabled={currentPage === 1}>&lt;</button>
              {[...Array(totalPages)].map((_, index) => (
                <button
                  key={index}
                  onClick={() => handlePageChange(index + 1)}
                  className={currentPage === index + 1 ? 'active' : ''}
                >
                  {index + 1}
                </button>
              ))}
              <button onClick={() => handlePageChange(currentPage + 1)} disabled={currentPage === totalPages}>&gt;</button>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default Home;
