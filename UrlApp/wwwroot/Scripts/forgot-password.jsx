import React, { useState } from 'react';

const ForgotPassword = () => {
    const [email, setEmail] = useState('');  //kullanıcıdan aldığımız e posta , setEmail ile de email'i güncelliyoruz.
    const [message, setMessage] = useState(''); // ekranda gösterdiğimiz mesaj
    const [isLoading, setIsLoading] = useState(false); 

    const handleSubmit = async (e) => {  //form gönderildiğinde çalışıcak fonksiyonumuz
        e.preventDefault();   // formun default olarak sayfanın yeniden yüklenmesini engeller.
        setIsLoading(true);

        // backend'e kullancının girdiği e postayı gönderiyoruz.
        try {  
            const response = await fetch('/api/auth/forgot-password', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ email })  // eposta adresini json formatına dönüştürerek gönderiyoruz.
            });
            
            const data = await response.json();
            setMessage(data.message);  // API'den gelen mesajı message state'ine atıyoruz (örneğin, başarı veya hata mesajı).
            
            if (response.ok) {
                setEmail(''); // Formu temizliyoruz
            }
        } catch (error) {
            setMessage('Bir hata oluştu. Lütfen daha sonra tekrar deneyin.');
            console.error('Error:', error);
        } finally {
            setIsLoading(false);
        }
    };
    //Formu render ediyoruz
    return (
        <div className="container mt-5">
            <div className="row justify-content-center">
                <div className="col-md-6">
                    <div className="card">
                        <div className="card-body">
                            <h2 className="card-title text-center mb-4">Şifremi Unuttum</h2>
                            
                            {message && (
                                <div className="alert alert-info" role="alert">
                                    {message}
                                </div>
                            )}
                            
                            <form onSubmit={handleSubmit}>  
                                <div className="form-group mb-3">
                                    <label htmlFor="email">E-posta Adresi</label>
                                    <input
                                        type="email"
                                        className="form-control"
                                        id="email"
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}  //Kullanıcı her tuşa bastığında, setEmail fonksiyonu ile e-posta adresi güncelleniyor.
                                        required
                                        disabled={isLoading}
                                    />
                                </div>
                                
                                <button 
                                    type="submit" 
                                    className="btn btn-primary w-100"
                                    disabled={isLoading}
                                >
                                    {isLoading ? 'Gönderiliyor...' : 'Şifre Sıfırlama Linki Gönder'}
                                </button>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ForgotPassword;
