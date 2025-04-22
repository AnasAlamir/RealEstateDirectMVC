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
using System.Security.Cryptography;
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
            User_Basic user_Basic = _userService.GetUserByEmail(securityUserDto.Email);
            if (user_Basic?.Email == securityUserDto.Email)
            {
                throw new Exception("User already exists");
            }

            // Generate salt and hash password (SHA-256)
            string salt = GenerateSalt();
            string hashedPassword = HashWithSHA256(securityUserDto.Password, salt);

            // Combine salt + hash (format: "salt:hash")
            string combinedHash = $"{salt}:{hashedPassword}";

            User_Create user_Create = new User_Create
            {
                Email = securityUserDto.Email,
                PasswordHash = combinedHash, // Store in existing column
                F_Name = securityUserDto.F_Name,
                L_Name = securityUserDto.L_Name,
                PhoneNumber = securityUserDto.PhoneNumber,
            };

            _userService.CreateUser(user_Create);
        }

        public string Login(LoginViewModel securityUserDto)
        {
            User_Basic user_Basic = _userService.GetUserByEmail(securityUserDto.Email);
            if (user_Basic == null)
            {
                return null; // Invalid email
            }

            // Split stored "salt:hash"
            string[] parts = user_Basic.PasswordHash.Split(':');
            if (parts.Length != 2) return null; // Invalid format

            string salt = parts[0];
            string storedHash = parts[1];

            // Verify password
            string hashedInput = HashWithSHA256(securityUserDto.Password, salt);
            if (hashedInput != storedHash)
            {
                return null; // Invalid password
            }

            return GenerateJwtToken(user_Basic);
        }

        //====== Helper Methods ======
        private string GenerateSalt(int size = 16)
        {
            byte[] saltBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        private string HashWithSHA256(string password, string salt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                string saltedPassword = password + salt;
                byte[] bytes = Encoding.UTF8.GetBytes(saltedPassword);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
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
