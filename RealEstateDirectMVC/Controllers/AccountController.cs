using Microsoft.AspNetCore.Mvc;
using MVC_Project.Services.AuthServices;
using MVC_Project.ViewModel;

namespace MVC_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Login2(LoginViewModel model)
        {
            var token = _authService.Login(model);
            if (token == null)
            {
                return RedirectToAction("Properties", "Home");
            }
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // set to false only if you're testing without HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login", "Account");
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Register2(RegisterViewModel securityUserDto)
        {
            try
            {
                _authService.Register(securityUserDto);
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }

        }
        
    }
}
