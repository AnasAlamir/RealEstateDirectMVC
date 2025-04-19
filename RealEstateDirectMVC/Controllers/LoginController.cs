using Microsoft.AspNetCore.Mvc;
using MVC_Project.Services.AuthServices;
using MVC_Project.ViewModel;

namespace MVC_Project.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
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
            return RedirectToAction("Index", "Home");
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
                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }

        }
        //[HttpPost("login")]
        //public ActionResult<string> Login(LoginUserDto securityUserDto)
        //{
        //    var token = _authService.Login(securityUserDto);
        //    if (token == null)
        //    {
        //        return BadRequest("Invalid email or password");
        //    }
        //    return Ok(token);
        //}
    }
}
