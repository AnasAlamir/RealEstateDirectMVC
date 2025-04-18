using application.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MVC_Project.Models;
using MVC_Project.Services.API_Services;
using MVC_Project.Services.AuthServices;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MVC_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IAuthService _authService;

        public SecurityController(IAuthService authService)
        {
            _authService = authService;
        }
        //public static User user = new User ();
        [HttpPost("register")]
        public IActionResult Register(RegisterUserDto securityUserDto)
        {
            var user = _authService.Register(securityUserDto);
            if (user == null)
            {
                return BadRequest("User already exists");
            }
            return Ok(user);
        }
        [HttpPost("login")]
        public ActionResult<string> Login(LoginUserDto securityUserDto)
        {
            var token = _authService.Login(securityUserDto);
            if (token == null)
            {
                return BadRequest("Invalid email or password");
            }
            return Ok(token);
        }
    }
    public class LoginUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class RegisterUserDto
    {
        public string F_Name { get; set; }
        public string L_Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }

}
