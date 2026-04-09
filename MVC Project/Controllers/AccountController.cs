using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Models;
using MVC_Project.Services;
using System.Security.Claims;

namespace MVC_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User user, string password)
        {
            ModelState.Remove("PasswordHash");
            ModelState.Remove("CreatedOn");

            if (!ModelState.IsValid)
                return View(user);

            if (_userService.EmailExists(user.Email))
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return View(user);
            }

            _userService.RegisterUser(user, password);

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Step 1: Check if the email even exists in the database
            if (!_userService.EmailExists(email))
            {
                ModelState.AddModelError("email", "No account found with this email address.");
                return View();
            }

            // Step 2: Email exists — now check if password is correct
            var user = _userService.GetUserByEmailAndPassword(email, password);

            if (user == null)
            {
                // Email was fine, so the password must be wrong
                ModelState.AddModelError("password", "Incorrect password. Please try again.");
                return View();
            }

            // Step 3: Both correct — sign in the user
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Complaints");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}