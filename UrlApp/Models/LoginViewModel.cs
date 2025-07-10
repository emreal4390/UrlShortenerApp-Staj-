using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
    [Display(Name = "Kullanıcı Adı")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Şifre gereklidir")]
    [Display(Name = "Şifre")]
    public string Password { get; set; }
} 