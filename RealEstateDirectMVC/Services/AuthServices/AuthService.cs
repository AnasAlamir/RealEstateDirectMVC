using _Services.Contracts;
using _Services.Models.User;
using application.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MVC_Project.Controllers;
using MVC_Project.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MVC_Project.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        protected readonly IUserService _userService;
        public AuthService(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }
        public void Register(RegisterViewModel securityUserDto)
        {
            _Services.Models.User.User_Basic user_Basic = _userService.GetUserByEmail(securityUserDto.Email);
            if (user_Basic?.Email == securityUserDto.Email)
            {
                throw new Exception("user already exists");
            }
            var hasher = new PasswordHasher<User_Basic>();
            string hashedPassword = hasher.HashPassword(user_Basic, securityUserDto.Password);
            User_Create user_Create = new User_Create
            {
                Email = securityUserDto.Email,
                PasswordHash = hashedPassword,
                F_Name = securityUserDto.F_Name,
                L_Name = securityUserDto.L_Name,
                PhoneNumber = securityUserDto.PhoneNumber,
            };
            _userService.CreateUser(user_Create);
            //return _userService.GetUserByEmail(user_Create.Email); ;///tmp
        }
        public string Login(LoginViewModel securityUserDto)
        {
            _Services.Models.User.User_Basic user_Basic = _userService.GetUserByEmail(securityUserDto.Email);
            if (user_Basic == null)
            {
                return null; // Invalid email
            }
            var hasher = new PasswordHasher<User_Basic>();
            // Verify a password
            var result = hasher.VerifyHashedPassword(user_Basic, user_Basic.PasswordHash, securityUserDto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return null; // Invalid password

            }
            return GenerateJwtToken(user_Basic); // Generate your JWT token here
        }
        private string GenerateJwtToken(User_Basic user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken
            (
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();               
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["AppSettings:Issuer"],
                    ValidAudience = _configuration["AppSettings:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["AppSettings:Token"]!)),
                    ValidateLifetime = true,
                    //ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
