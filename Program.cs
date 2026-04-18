using Microsoft.EntityFrameworkCore;
using MVC_Project.Data;
using MVC_Project.Services;
using MVC_Project.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Tell Npgsql to accept DateTime.Now (local time) without complaining
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ─── Read PORT from Railway environment (Railway assigns a random port) ───
// If PORT env variable exists (on Railway), use it. Otherwise use 8080 locally.
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ─── Services ─────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// ─── Database Setup ───────────────────────────────────────────────────────
// Specifically: Railway gives us DATABASE_URL as an environment variable.
// If that exists, use it. If not (running locally), fall back to appsettings.json.
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));  // UseNpgsql instead of UseSqlServer

// ─── Your Custom Services ─────────────────────────────────────────────────
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IComplaintService, ComplaintService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// ─── Cookie Authentication ────────────────────────────────────────────────
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var app = builder.Build();

// ─── Auto-Apply Migrations on Startup ────────────────────────────────────
// Specifically: When app starts on Railway, this block automatically creates
// all your tables in the PostgreSQL database. You don't need to run
// "dotnet ef database update" manually on the server.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ─── Middleware Pipeline ───────────────────────────────────────────────────
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ─── Routes ───────────────────────────────────────────────────────────────
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Complaints}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notificationHub");

app.Run();