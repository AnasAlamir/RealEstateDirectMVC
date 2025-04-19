using application.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using MVC_Project.Controllers;
using MVC_Project.ViewModel;

namespace MVC_Project.Services.AuthServices
{
    public interface IAuthService
    {
        public void Register(RegisterViewModel securityUserDto);
        public string Login(LoginViewModel securityUserDto);
    }
}
