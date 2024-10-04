import React, { useState } from 'react';
import axios from 'axios';
import './UpdateImageForm.css';


const UpdateImageForm = ({ image, onCancel, onUpdateSuccess }) => {
  const [title, setTitle] = useState(image.title || '');
  const [description, setDescription] = useState(image.description || '');

  const handleSubmit = async (e) => {
    e.preventDefault();

    const updatedData = {
      imageId: image.imageId, 
      title: title,
      description: description,
      imageURL: image.imageURL, 
      createdDate: image.createdDate,
    };

    try {
      const response = await axios.put(`http://localhost:5263/api/image/${image.imageId}`, updatedData);

      
      onUpdateSuccess(response.data);

      alert('Image updated successfully!');
    } catch (error) {
      console.error('Error updating image:', error);
      alert('Failed to update the image. Please try again.');
    }
  };

  return (
    <div className="update-image-form">
      <h3>Update Image</h3>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="title">Title</label>
          <input
            type="text"
            id="title"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
          />
        </div>
        <div>
          <label htmlFor="description">Description</label>
          <textarea
            id="description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            required
          />
        </div>
        <button type="submit">Submit</button>
        <button type="button" onClick={onCancel}>
          Cancel
        </button>
      </form>
    </div>
  );
};

export default UpdateImageForm;
