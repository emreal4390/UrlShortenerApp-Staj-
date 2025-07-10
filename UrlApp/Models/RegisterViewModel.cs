using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter arasında olmalıdır")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email adresi gereklidir")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre gereklidir")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Cinsiyet seçimi gereklidir")]
    public string Gender { get; set; }

    [Required(ErrorMessage = "Doğum tarihi gereklidir")]
    [DataType(DataType.Date)]
    [Display(Name = "Doğum Tarihi")]
    public DateTime BirthDate { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    [Display(Name = "Telefon Numarası")]
    public string? PhoneNumber { get; set; }
} 