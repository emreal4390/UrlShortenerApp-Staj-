import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const Login = () => {
    const [formData, setFormData] = useState({
        username: '',
        password: ''
    });
    const [error, setError] = useState('');
    const [successMessage, setSuccessMessage] = useState('');
    const navigate = useNavigate();

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prevState => ({
            ...prevState,
            [name]: value
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post('/Auth/Login', formData);
            if (response.data.success) {
                setSuccessMessage('Giriş başarılı!');
                navigate('/dashboard'); // Başarılı girişten sonra yönlendirilecek sayfa
            }
        } catch (err) {
            setError('Kullanıcı adı veya şifre hatalı!');
        }
    };

    return (
        <div className="row">
            <div className="col-md-6 offset-md-3">
                <div className="card shadow-lg">
                    <div className="card-body">
                        <h2 className="text-center mb-4">Giriş Yap</h2>

                        {successMessage && (
                            <div className="alert alert-success">
                                {successMessage}
                            </div>
                        )}

                        {error && (
                            <div className="alert alert-danger">
                                {error}
                            </div>
                        )}

                        <form onSubmit={handleSubmit}>
                            <div className="form-group mb-3">
                                <label htmlFor="username">Kullanıcı Adı</label>
                                <input
                                    type="text"
                                    className="form-control"
                                    id="username"
                                    name="username"
                                    value={formData.username}
                                    onChange={handleChange}
                                    required
                                />
                            </div>

                            <div className="form-group mb-3">
                                <label htmlFor="password">Şifre</label>
                                <input
                                    type="password"
                                    className="form-control"
                                    id="password"
                                    name="password"
                                    value={formData.password}
                                    onChange={handleChange}
                                    required
                                />
                            </div>

                            <div className="form-group d-flex justify-content-between align-items-center">
                                <button type="submit" className="btn btn-primary">
                                    Giriş Yap
                                </button>
                                <a href="/Home/ForgotPassword" className="text-primary">
                                    Şifremi Unuttum
                                </a>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Login; 