import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './Comments.css'; 

const Comments = ({ imageId, onClose }) => {
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [editingCommentId, setEditingCommentId] = useState(null); 
  const [editedContent, setEditedContent] = useState(''); 

  
  useEffect(() => {
    const fetchComments = async () => {
      const token = localStorage.getItem('authToken');

      if (!token) {
        console.error("Token is missing. User might not be authenticated.");
        setErrorMessage("You must be logged in to view comments.");
        return;
      }

      try {
        const response = await axios.get(`http://localhost:5263/api/comment/image/${imageId}`, {
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        });

        setComments(response.data);
      } catch (error) {
        console.error("There is no commeents for this image ", error);
        setErrorMessage("Error fetching comments.");
      }
    };

    if (imageId) {
      fetchComments();
    }
  }, [imageId]);

  // Adding  a new comment
  const handleAddComment = async (e) => {
    e.preventDefault();

    const token = localStorage.getItem("authToken");

    if (!token) {
      console.error("Token is undefined. User is not authenticated.");
      setErrorMessage("You must be logged in to add comments.");
      return;
    }

    const commentData = {
      content: newComment,
    };

    try {
      const response = await axios.post(
        `http://localhost:5263/api/comment/${imageId}`, 
        commentData, 
        {
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
          },
        }
      );

      setComments((prevComments) => [...prevComments, response.data]);
      setNewComment("");
    } catch (error) {
      console.error("Error adding comment:", error);
      setErrorMessage("Error adding comment: " + (error.response?.data?.error || error.message));
    }
  };

  // Delete a comment
  const handleDeleteComment = async (commentId) => {
    const token = localStorage.getItem("authToken");

    if (!token) {
      setErrorMessage("You must be logged in to delete comments.");
      return;
    }

    try {
      await axios.delete(`http://localhost:5263/api/comment/${commentId}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      setComments(comments.filter(comment => comment.commentId !== commentId));
    } catch (error) {
      console.error("Error deleting comment:", error);
      setErrorMessage("Error deleting comment: " + (error.response?.data?.error || error.message));
    }
  };

  // Starts editing a comment and Set the existing content for editing
  const handleEditComment = (commentId, content) => {
    setEditingCommentId(commentId);
    setEditedContent(content); 
  };

  // Update a comment
  const handleUpdateComment = async (commentId) => {
    const token = localStorage.getItem("authToken");

    if (!token) {
      setErrorMessage("You must be logged in to update comments.");
      return;
    }

    const updatedCommentData = {
      content: editedContent,
    };

    try {
      const response = await axios.put(
        `http://localhost:5263/api/comment/${commentId}`,
        updatedCommentData,
        {
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        }
      );

      // Update the comments array with the edited comment
      setComments(comments.map(comment => 
        comment.commentId === commentId ? { ...comment, content: response.data.content } : comment
      ));

      // Reset the editing state
      setEditingCommentId(null);
      setEditedContent("");
    } catch (error) {
      console.error("Error updating comment:", error);
      setErrorMessage("Error updating comment: " + (error.response?.data?.error || error.message));
    }
  };

  return (
    <div className="comments-section">
      {/* Close button at the top right corner */}
      <button className="close-button" onClick={onClose}>X</button>

      <h3>Comments</h3>
      {comments.length > 0 ? (
        <ul>
          {comments.map((comment) => (
            <li key={comment.commentId}>
              {/* Display edit input or the comment content */}
              {editingCommentId === comment.commentId ? (
                <input 
                  type="text" 
                  value={editedContent}
                  onChange={(e) => setEditedContent(e.target.value)}
                />
              ) : (
                <p>{comment.content} - by {comment.createdBy}</p>
              )}

              <button className="delete-button" onClick={() => handleDeleteComment(comment.commentId)}>Delete</button>

              {/* Conditionally render "Update" or "Edit" button */}
              {editingCommentId === comment.commentId ? (
                <button className="update-button" onClick={() => handleUpdateComment(comment.commentId)}>Update</button>
              ) : (
                <button className="edit-button" onClick={() => handleEditComment(comment.commentId, comment.content)}>Edit</button>
              )}
            </li>
          ))}
        </ul>
      ) : (
        <p>{errorMessage || "No comments yet."}</p>
      )}

      {errorMessage && <p className="error">{errorMessage}</p>}

      <form onSubmit={handleAddComment}>
        <textarea
          value={newComment}
          onChange={(e) => setNewComment(e.target.value)}
          placeholder="Add a comment..."
        />
        <button type="submit">Add Comment</button>
      </form>
    </div>
  );
};

export default Comments;
