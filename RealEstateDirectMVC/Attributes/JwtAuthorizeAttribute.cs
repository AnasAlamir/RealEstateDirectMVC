using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MVC_Project.Services.AuthServices;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace MVC_Project.Attributes
{
    public class JwtAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly IAuthService _authService;

        public JwtAuthorizeAttribute(IAuthService authService)
        {
            _authService = authService;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            var principal = _authService.ValidateToken(token);
            if (principal == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            context.HttpContext.User = principal;
            base.OnActionExecuting(context);
        }
    }
}
