using _Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Attributes;
using MVC_Project.Models.Token;
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
            var tokenResponse = _authService.Login(model);
            if (tokenResponse?.AccessToken == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Response.Cookies.Append("AccessToken", tokenResponse?.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(10)
            });
            Response.Cookies.Append("RefreshToken", tokenResponse?.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AccessToken");
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
        [ServiceFilter(typeof(JwtAuthorizeAttribute))]
        public IActionResult RefreshToken()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var refreshToken = Request.Cookies["RefreshToken"];
            var request = new RefreshTokenRequestDto
            {
                UserId = int.Parse(userId),
                RefreshToken = refreshToken
            };
            var tokenResponse = _authService.RefreshTokens(request);
            if (tokenResponse?.AccessToken == null || tokenResponse?.RefreshToken == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Response.Cookies.Append("AccessToken", tokenResponse?.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(10)
            });
            Response.Cookies.Append("RefreshToken", tokenResponse?.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
            return RedirectToAction("Index", "Home");
        }

    }
}
