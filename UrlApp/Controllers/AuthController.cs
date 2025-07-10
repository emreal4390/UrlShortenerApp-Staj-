using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using UrlApp.Data;
using UrlApp.Models;
using Microsoft.EntityFrameworkCore;

namespace UrlApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly UrlShortenerDbContext _context;
        private readonly IConfiguration _configuration;
        
        public AuthController(UrlShortenerDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            
            if (user == null || user.Password != model.Password)
            {
                ModelState.AddModelError("", "Geçersiz kullanıcı adı veya şifre");
                return View(model);
            }

            if (!user.IsVerified)
            {
                var verificationToken = GenerateVerificationToken();
                user.VerificationCode = verificationToken;
                await _context.SaveChangesAsync();

                await SendVerificationEmail(user.Email, verificationToken);

                TempData["InfoMessage"] = "Email adresiniz henüz doğrulanmamış. Yeni bir doğrulama linki email adresinize gönderildi.";
                return RedirectToAction("Login");
            }

            // Kullanıcı için claims oluştur
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email || u.Username == model.Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Bu email adresi veya kullanıcı adı zaten kayıtlı");
                return View(model);
            }

            var verificationToken = GenerateVerificationToken();
            
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                VerificationCode = verificationToken,
                IsVerified = false,
                CreatedAt = DateTime.UtcNow,
                Gender = model.Gender,
                BirthDate = model.BirthDate,
                PhoneNumber = model.PhoneNumber
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await SendVerificationEmail(user.Email, verificationToken);

            TempData["SuccessMessage"] = "Kayıt işleminiz başarılı. Lütfen email adresinize gönderilen doğrulama linkine tıklayın.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.VerificationCode == token);
            
            if (user == null)
            {
                TempData["ErrorMessage"] = "Geçersiz veya süresi dolmuş doğrulama linki.";
                return RedirectToAction("Login");
            }

            user.IsVerified = true;
            user.VerificationCode = null;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Email adresiniz başarıyla doğrulandı. Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }


       
        [HttpPost]
        [Route("api/auth/forgot-password")]
        public async Task<IActionResult> ForgotPasswordApi([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    // Güvenlik için kullanıcıya email'in var olup olmadığını söylemiyoruz
                    return Ok(new { message = "Eğer bu email kayıtlıysa, şifre sıfırlama linki gönderilecektir." });
                }

                // Benzersiz token oluştururyouz
                var resetToken = Guid.NewGuid().ToString("N");
                user.VerificationCode = resetToken;
                await _context.SaveChangesAsync();

                // Şifre sıfırlama linkini oluştururuyoruz
                var resetLink = $"{Request.Scheme}://{Request.Host}/Auth/ResetPassword?token={resetToken}";
                
                // Email gönder
                await SendPasswordResetEmail(user.Email, resetLink);

                return Ok(new { message = "Şifre sıfırlama linki email adresinize gönderildi." });
            }
            catch (Exception ex)
            {
                // Hata loglanabilir
                return StatusCode(500, new { message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin." });
            }
        }
        //şifre güncelleme
        [HttpPost]
        [Route("api/auth/reset-password")]
        public async Task<IActionResult> ResetPasswordApi([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationCode == request.Token);
                if (user == null)
                {
                    return BadRequest(new { message = "Geçersiz veya süresi dolmuş şifre sıfırlama linki." });
                }

                // Şifreyi güncelle
                user.Password = request.NewPassword; 
                user.VerificationCode = null; // Token'ı sıfırla
                await _context.SaveChangesAsync();

                return Ok(new { message = "Şifreniz başarıyla güncellendi. Giriş sayfasına yönlendiriliyorsunuz..." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin." });
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Geçersiz şifre sıfırlama linki.";
                return RedirectToAction("Login");
            }

            // Token'ı ViewBag'e ekle (opsiyonel)
            ViewBag.Token = token;
            return View();
        }

        private string GenerateVerificationToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        private async Task SendVerificationEmail(string email, string token)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            
            var verificationLink = $"{Request.Scheme}://{Request.Host}/Auth/VerifyEmail?email={email}&token={token}";
            
            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    emailSettings["SmtpUsername"],
                    emailSettings["SmtpPassword"]
                ),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SmtpUsername"]),
                Subject = "Email Doğrulama",
                Body = $@"
                    <h2>Email Doğrulama</h2>
                    <p>Merhaba,</p>
                    <p>Email adresinizi doğrulamak için aşağıdaki linke tıklayın:</p>
                    <p><a href='{verificationLink}'>Email Adresimi Doğrula</a></p>
                    <p>Bu link 30 dakika süreyle geçerlidir.</p>
                    <p>Eğer bu işlemi siz yapmadıysanız, bu emaili görmezden gelebilirsiniz.</p>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Email gönderimi başarısız oldu: " + ex.Message);
            }
        }

        private async Task SendPasswordResetEmail(string email, string resetLink)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            
            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    emailSettings["SmtpUsername"],
                    emailSettings["SmtpPassword"]
                ),
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SmtpUsername"]),
                Subject = "Şifre Sıfırlama",
                Body = $@"
                    <h2>Şifre Sıfırlama</h2>
                    <p>Merhaba,</p>
                    <p>Şifrenizi sıfırlamak için aşağıdaki linke tıklayın:</p>
                    <p><a href='{resetLink}'>Şifremi Sıfırla</a></p>
                    <p>Bu link 30 dakika süreyle geçerlidir.</p>
                    <p>Eğer bu isteği siz yapmadıysanız, bu emaili görmezden gelebilirsiniz.</p>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
} 