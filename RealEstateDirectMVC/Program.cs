using _DataAccess;
using _Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MVC_Project.Attributes;
using MVC_Project.Services.API_Services;
using MVC_Project.Services.AuthServices;
using System.Text;
namespace MVC_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.RegisterDataAccess();
            builder.Services.RegisterService();

            builder.Services.AddScoped<IBase_API_Call, Base_API_Call>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidIssuer = builder.Configuration["AppSettings:Issuer"],
            //            ValidateAudience = true,
            //            ValidAudience = builder.Configuration["AppSettings:Audience"],
            //            ValidateLifetime = true,
            //            ValidateIssuerSigningKey = true,
            //            IssuerSigningKey = new SymmetricSecurityKey(
            //                Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)
            //                )
            //        };
            //    });

            builder.Services.AddScoped<JwtAuthorizeAttribute>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
