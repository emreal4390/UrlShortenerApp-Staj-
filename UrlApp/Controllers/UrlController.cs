using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UrlApp.Data;
using UrlApp.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;

namespace UrlApp.Controllers
{
    [Authorize]
    public class UrlController : Controller
    {
        private readonly UrlShortenerDbContext _db;

        public UrlController(UrlShortenerDbContext db)
        {
            _db = db;
        }

        // GET: Url/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: Url/Shorten
        [HttpPost]
        public async Task<IActionResult> Shorten([FromForm] string originalUrl)
        {
            if (string.IsNullOrEmpty(originalUrl))
            {
                return BadRequest(new { error = "URL boş olamaz." });
            }

            // Giriş yapmış kullanıcının ID'sini al
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // URL'nin bu kullanıcı tarafından daha önce kısaltılıp kısaltılmadığını kontrol et
            var existingUrl = await _db.Urls.FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl && u.UserId == userId);
            if (existingUrl != null)
            {
                return Json(new { shortUrl = existingUrl.ShortUrl });
            }

            string shortUrl = GenerateShortUrl();

            var url = new Url
            {
                OriginalUrl = originalUrl,
                ShortUrl = shortUrl,
                CreatedAt = DateTime.Now,
                UserId = userId,
                ClickCount = 0
            };

            _db.Urls.Add(url);
            await _db.SaveChangesAsync();

            return Json(new { shortUrl });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> RedirectToOriginal(string shortUrl)
        {
            var url = await _db.Urls.FirstOrDefaultAsync(u => u.ShortUrl == shortUrl);

            if (url != null)
            {
                // Tıklanma sayısını artır
                url.ClickCount++;
                await _db.SaveChangesAsync();

                return Redirect(url.OriginalUrl);
            }

            ViewBag.ErrorMessage = "The shortened URL could not be found.";
            return View();
        }

        private string GenerateShortUrl() // url'yi random bir şekilde seçtiği 6 haneli koda çeviren metod
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
