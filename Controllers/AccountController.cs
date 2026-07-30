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
            
            if (!_userService.EmailExists(email))
            {
                ModelState.AddModelError("email", "No account found with this email address");
                return View();
            }

           
            var user = _userService.GetUserByEmailAndPassword(email, password);

            if (user == null)
            {
                
                ModelState.AddModelError("password", "Incorrect password. Please try again.");
                return View();
            }

           
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