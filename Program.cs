using Microsoft.EntityFrameworkCore;
using MVC_Project.Data;
using MVC_Project.Services;
using MVC_Project.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;

// Prevent Linux inotify limit crash on container hosts like Render
Environment.SetEnvironmentVariable("DOTNET_USE_POLLING_FILE_WATCHER", "true");

var builder = WebApplication.CreateBuilder(args);

// Tell Npgsql to accept DateTime.Now (local time) without complaining
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ─── PORT SETUP ───────────────────────────────────────────────────────────
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ─── SERVICES ─────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// ─── DATABASE SETUP ───────────────────────────────────────────────────────
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(databaseUrl))
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        var username = Uri.UnescapeDataString(userInfo[0]);
        var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
        var port = uri.Port > 0 ? uri.Port : 5432;
        var npgsqlConnection =
            $"Host={uri.Host};" +
            $"Port={port};" +
            $"Database={uri.AbsolutePath.TrimStart('/')};" +
            $"Username={username};" +
            $"Password={password};" +
            $"SSL Mode=Require;Trust Server Certificate=true;";
        options.UseNpgsql(npgsqlConnection);
    }
    else
    {
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"));
    }
});

// ─── YOUR CUSTOM SERVICES ─────────────────────────────────────────────────
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IComplaintService, ComplaintService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// ─── COOKIE AUTHENTICATION ────────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var app = builder.Build();

// ─── AUTO-APPLY MIGRATIONS ON STARTUP ─────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration error: {ex.Message}");
    }
}

// ─── MIDDLEWARE PIPELINE ───────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ─── ROUTES ───────────────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Complaints}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notificationHub");

app.Run();