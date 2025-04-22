using application.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Controllers;
using MVC_Project.Models.Token;
using MVC_Project.ViewModel;
using System.Security.Claims;

namespace MVC_Project.Services.AuthServices
{
    public interface IAuthService
    {
        public void Register(RegisterViewModel securityUserDto);
        public TokenResponseDto Login(LoginViewModel securityUserDto);
        public TokenResponseDto RefreshTokens(RefreshTokenRequestDto refreshTokenRequestDto);
        public ClaimsPrincipal? ValidateToken(string token);
    }
}
