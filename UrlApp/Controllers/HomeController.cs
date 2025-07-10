using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UrlApp.Models;
using System.Security.Claims;

using UrlApp.Data;


namespace UrlApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UrlShortenerDbContext _context;

        public HomeController(ILogger<HomeController> logger, UrlShortenerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View("Welcome");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Sadece bu kullanıcıya ait URL'leri getir
            var urls = _context.Urls
                .Where(u => u.UserId == userId)
                .OrderByDescending(u => u.CreatedAt)
                .ToList();

            return View(urls);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
