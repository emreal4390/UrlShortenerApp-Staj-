using Microsoft.EntityFrameworkCore;
using UrlApp.Data; // DbContext sınıfı buraya ekledik
using UrlApp.Controllers; // Gerekli controller referansını ekleyelim
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container (dependency injection için gerekli servisler ekleniyor).
builder.Services.AddControllersWithViews(); // MVC desteği ekleniyor.

// Veritabanı bağlantısı için DbContext ekleniyor:
builder.Services.AddDbContext<UrlShortenerDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("UrlApp")
    ));

// Gerekli Controller ve Model'in eklenmesi
builder.Services.AddScoped<UrlController>(); // UrlController'a bağımlılığı ekle

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "UrlShortener.Auth";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

// Build the application.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Geliştirme ortamı için hata sayfası
    app.UseDeveloperExceptionPage();
}
else
{
    // Üretim ortamı için hata ynetimi ve HSTS
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTP isteklerini HTTPS'e yönlendirme
app.UseHttpsRedirection();

// Statik dosyalar (CSS, JS, img vb.) için middleware
app.UseStaticFiles();

// Routing işlemleri
app.UseRouting();

// Yetkilendirme middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();