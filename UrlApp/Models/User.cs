namespace UrlApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? VerificationCode { get; set; }
        public bool IsVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Gender { get; set; }  // "M" veya "F"
        public DateTime? BirthDate { get; set; }
        public string? PhoneNumber { get; set; }
    }
} 