import React, { useState, useEffect } from 'react';
import { FaHome, FaUpload, FaSignOutAlt } from 'react-icons/fa';
import { MdSearch, MdFilterList } from 'react-icons/md';
import { useNavigate } from 'react-router-dom'; // Import useNavigate
import axios from 'axios';
import './home.css';

const Home = () => {
  const [currentPage, setCurrentPage] = useState(1);
  const totalPages = 3; // Example total number of pages
  const [images, setImages] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate(); // Initialize useNavigate

  useEffect(() => {
    const fetchImages = async () => {
      try {
        const response = await axios.get('http://localhost:5263/api/image');
        console.log("Fetched images:", response.data); // Check the fetched data
        setImages(response.data);
        setLoading(false);
      } catch (error) {
        console.error("Error fetching images:", error);
        setLoading(false);
      }
    };

    fetchImages();
  }, []);

  const handlePageChange = (page) => {
    setCurrentPage(page);
  };

  const filteredImages = images.slice(
    (currentPage - 1) * 4,
    currentPage * 4
  );

  const handleLogout = async () => {
    try {
      await axios.post('http://localhost:5263/api/Account/logout');
      navigate('/logout');  // Navigate to the logout page after logout
    } catch (error) {
      console.error('Logout failed', error);
    }
  };

  return (
    <div className="container">
      <div className="sidebar">
        <div className="logo-section">
          <div className="logo">Logo</div>
        </div>
        <div className="menu">
          <div className="menu-item active">
            <FaHome />
            <span>Home</span>
          </div>
          <div className="menu-item">
            <FaUpload />
            <span>Image Upload</span>
          </div>
        </div>
        <div className="logout">
          <FaSignOutAlt />
          <span>Logout</span>
          <button onClick={handleLogout}>Logout</button>
        </div>
      </div>
      <div className="content">
        <div className="header">
          <h2>Home</h2>
          <div className="user">
            <span>User</span>
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
        </div>
      </div>
    </div>
  );
};

export default Home;
