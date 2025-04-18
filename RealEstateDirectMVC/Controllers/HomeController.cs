using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using _Service;
using MVC_Project.Models;
using MVC_Project.ViewModel;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using MVC_Project.Services.API_Services;

namespace MVC_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBase_API_Call _base_API_Call;

        public HomeController(ILogger<HomeController> logger, IBase_API_Call base_API_Call)
        {
            _logger = logger;
            _base_API_Call = base_API_Call;
        }

        public IActionResult Index()
        {
            var cityList = _base_API_Call.GetAllCity();
            return View(cityList);
        }

        public IActionResult Properties()
        {
            var cityList =  _base_API_Call.GetAllCity();
            var propertyList = _base_API_Call.GetPropertyList();
            var viewModel = new PropertyViewModel
            {
                Properties = propertyList,
                Cites = cityList
            };

            ViewData["MaxPrice"] = propertyList.Max(x => x.Price);
            ViewData["MinPrice"] = propertyList.Min(x => x.Price);
            ViewData["MaxArea"] = propertyList.Max(x => x.Area);
            ViewData["MinArea"] = propertyList.Min(x => x.Area);

            return View(viewModel);
        }

        public IActionResult PropertyiesPartial(string? keyWord = null, string? city = null, int? status = null,
                                               decimal? maxPrice = null, double? maxArea = null,
                                               int? minBaths = null, int? minBed = null,
                                               bool HasGarage = false, bool Two_Stories = false, bool Laundry_Room = false,
                                               bool HasPool = false, bool HasGarden = false, bool HasElevator = false,
                                               bool HasBalcony = false, bool HasParking = false, bool HasCentralHeating = false, bool IsFurnished = false)
        {
            Filter filter = new Filter
            {
                Status = status == 0 ? null : (status == 1 ? Status.rent : (status == 2 ? Status.buy : null)),
                City = city != "All" ? city : null,
                Keyword = string.IsNullOrEmpty(keyWord) ? null : keyWord,
                PriceRange = maxPrice != 0 ? maxPrice : null,
                AreaSize = maxArea != 0 ? maxArea : null,
                Beds = minBed != 0 ? minBed : null,
                Baths = minBaths != 0 ? minBaths : null,
                HasGarage = HasGarage,
                Two_Stories = Two_Stories,
                Laundry_Room = Laundry_Room,
                HasPool = HasPool,
                HasGarden = HasGarden,
                HasElevator = HasElevator,
                HasBalcony = HasBalcony,
                HasParking = HasParking,
                HasCentralHeating = HasCentralHeating,
                IsFurnished = IsFurnished
            };

            var properties =  _base_API_Call.GetFilteredProperties(filter);
            return PartialView("/Views/Partial_Views/_propertyListPartial.cshtml", properties);
        }



        //public IActionResult PropertyDetails(int id) // ???? ?? ?? ??? ?????? ?? "PropertyDetails"
        //{
        //    var property =  _base_API_Call.GetPropertyById(id);
        //    return View("/Views/Home/PropertyDetails.cshtml", property);
        //}
        public IActionResult PropertyDetails(int id)
        {
            if (id == 0)
            {
                // This is likely the hot reload checking the page
                // Return a minimal response or redirect to properties list
                return RedirectToAction("Properties");
            }

            var property = _base_API_Call.GetPropertyById(id);

            // Add null check for property
            if (property == null)
            {
                return NotFound();
            }

            return View("/Views/Home/PropertyDetails.cshtml", property);
        }


        public IActionResult Profile()
        {
            // ?????? ??? ?????? ?????????? ?? Claims
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            // ?????? ??? ??????? ???????? ?? API
            var user = _base_API_Call.GetUserInfo(email);

            // ????? ???????? ?? ??????
            HttpContext.Session.SetString("User", JsonConvert.SerializeObject(user));
            ViewBag.User = JsonConvert.SerializeObject(user);

            return View(user);
        }

        public IActionResult ProfilePartial()
        {
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Profile");
            }

            var user = JsonConvert.DeserializeObject<User_Basic>(userJson);
            return PartialView("/Views/Partial_Views/_myProfilePartial.cshtml", user);
        }

        public IActionResult MyPropertiesPartial()
        {
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Profile");
            }

            var user = JsonConvert.DeserializeObject<User_Basic>(userJson);
            var properties = _base_API_Call.GetPropertyList(user.PropertiesId);
            return PartialView("/Views/Partial_Views/_myPropertiesPartial.cshtml", properties);
        }

        public IActionResult FavoritedPropertiesPartial()
        {
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Profile");
            }

            var user = JsonConvert.DeserializeObject<User_Basic>(userJson);
            var properties = _base_API_Call.GetPropertyList(user.FavoriteId);
            return PartialView("/Views/Partial_Views/_favoritedPropertiesPartial.cshtml", properties);
        }

        public IActionResult MassagesPartial()
        {
            var userJson = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(userJson))
            {
                return RedirectToAction("Profile");
            }

            var user = JsonConvert.DeserializeObject<User_Basic>(userJson);
            var massages = _base_API_Call.GetMassagesToUser(user.Id);

            return PartialView("/Views/Partial_Views/_massagesPartial.cshtml", massages);
        }


        public IActionResult SubmitPropertyPartial()
        {
            return PartialView("/Views/Partial_Views/_submitPropertyPartial.cshtml");
        }

        public IActionResult ChangePasswordPartial()
        {
            return PartialView("/Views/Partial_Views/_changePasswordPartial.cshtml");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
