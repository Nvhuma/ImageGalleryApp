import React, { useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';
import './ForgotPassword.css';

function ForgotPassword() {
    const [email, setEmail] = useState('');
    const [message, setMessage] = useState('');
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);
        setMessage('');

        try {
            const response = await axios.post('http://localhost:5263/api/Account/forgot-password', { email });
            setMessage(response.data.message);
        } catch (error) {
            if (error.response) {
                setMessage(error.response.data.message || 'Error sending password reset link.');
            } else {
                setMessage('Error sending password reset link.');
            }
        } finally {
            setMessage('Password reset link has been sent to your email.');
            setLoading(false);
        }
    };

    return (
        <div className="forgot-password-container">
            <div className="forgot-password-box">
                <h2>Recover Password</h2>
                <form onSubmit={handleSubmit}>
                    <label>Email Address</label>
                    <input 
                        type="email" 
                        value={email} 
                        placeholder="Enter Email"
                        onChange={(e) => setEmail(e.target.value)} 
                        required 
                    />
                    <p className="forgot-password-login-link">
                        <Link to="/login">Back to Login</Link>
                    </p>
                    <button type="submit" disabled={loading}>
                        {loading ? 'Sending...' : 'Recover Password'}
                    </button>
                </form>
                {message && <p>{message}</p>}
            </div>
            <div className="forgot-password-background"></div>
        </div>
    );
}

export default ForgotPassword;
