using application.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Controllers;

namespace MVC_Project.Services.AuthServices
{
    public interface IAuthService
    {
        public _Services.Models.User.User_Basic Register(RegisterUserDto securityUserDto);
        public string Login(LoginUserDto securityUserDto);
    }
}
