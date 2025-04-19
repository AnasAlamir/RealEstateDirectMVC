using application.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MVC_Project.Models;
using MVC_Project.Services.API_Services;
using MVC_Project.Services.AuthServices;
using MVC_Project.ViewModel;
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
        [HttpPost("register")]
        public IActionResult Register(RegisterViewModel securityUserDto)
        {
            try
            {
                _authService.Register(securityUserDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("login")]
        public ActionResult<string> Login(LoginViewModel securityUserDto)
        {
            var token = _authService.Login(securityUserDto);
            if (token == null)
            {
                return BadRequest("Invalid email or password");
            }
            return Ok(token);
        }
        [Authorize]
        [HttpGet("authEndPoint")]
        public IActionResult AuthenticatedOnlyEndPoint()
        {
            return Ok("you are authenticated");
        }
    }
    //public class LoginUserDto
    //{
    //    public string Email { get; set; }
    //    public string Password { get; set; }
    //}
    //public class RegisterUserDto
    //{
    //    public string F_Name { get; set; }
    //    public string L_Name { get; set; }
    //    public string Email { get; set; }
    //    public string Password { get; set; }
    //    public string PhoneNumber { get; set; }
    //}

}
