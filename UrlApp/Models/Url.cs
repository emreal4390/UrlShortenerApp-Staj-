using System;

namespace UrlApp.Models
{
    public class Url
    {
        public int Id { get; set; }          // Veritabanında her URL için benzersiz bir ID
        public string OriginalUrl { get; set; }   // Kullanıcının girdiği orijinal URL
        public string ShortUrl { get; set; }    // Kısa URL
        public DateTime CreatedAt { get; set; }  // URL'nin ne zaman oluşturulduğu
        public int ClickCount { get; set; } // Tıklanma sayısı için yeni alan
        public int UserId { get; set; } // Kullanıcı ID'si için yeni alan
        public User User { get; set; } // Navigation property
    }
}
