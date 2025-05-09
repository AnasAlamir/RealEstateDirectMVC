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
using Microsoft.AspNetCore.Authorization;
using MVC_Project.Attributes;
using MVC_Project.Services.AuthServices;
using _Services.Contracts;
using System.Security.Claims;
using _Services.Models.PropertyImage;


namespace MVC_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBase_API_Call _base_API_Call;
        private readonly IAuthService _authService;
        private readonly IInquiryService _inquiryService;
        private readonly IUserService _userService;
        private readonly IPropertyService _propertyService;

        public HomeController(ILogger<HomeController> logger, IBase_API_Call base_API_Call, IAuthService authService, IInquiryService inquiryService, IUserService userService, IPropertyService propertyService)
        {
            _logger = logger;
            _base_API_Call = base_API_Call;
            _authService = authService;
            _inquiryService = inquiryService;
            _userService = userService;
            _propertyService = propertyService;
        }
        [ServiceFilter(typeof(JwtAuthorizeAttribute))]
        public IActionResult Index()
        {
            var cityList = _base_API_Call.GetAllCity();
            var username = User.Identity?.Name;
            return View(cityList);
        }
        [ServiceFilter(typeof(JwtAuthorizeAttribute))]
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
        [ServiceFilter(typeof(JwtAuthorizeAttribute))]
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
        [ServiceFilter(typeof(JwtAuthorizeAttribute))]
        public IActionResult SendInquiry(_Services.Models.Inquiry.Inquiry_Create inquiryCreate)
        {
            _inquiryService.CreateInquiry(inquiryCreate);
            return RedirectToAction("PropertyDetails", new { id = inquiryCreate.PropertyId });
        }

        [ServiceFilter(typeof(JwtAuthorizeAttribute))]
        public IActionResult Profile()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _base_API_Call.GetUserInfo(email);

            ViewBag.User = user;
            return View(user);
        }
        [ServiceFilter(typeof(JwtAuthorizeAttribute))]
        public IActionResult ProfilePartial()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Profile");
            }

            var user = _base_API_Call.GetUserInfo(email);

            if (user == null)
            {
                return RedirectToAction("Profile");
            }

            return PartialView("/Views/Partial_Views/_myProfilePartial.cshtml", user);
        }
        [ServiceFilter(typeof(JwtAuthorizeAttribute))]
        public IActionResult UpdateuserInfo(_Services.Models.User.User_Update user_Update)
        {
            var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            _userService.UpdateUser(int.Parse(id), user_Update);
            return RedirectToAction("Profile");
        }
        [ServiceFilter(typeof(JwtAuthorizeAttribute))]
        public IActionResult SubmitNewProperty(Property_CreateViewModel model)
        {
            var ownerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var imageUrls = new List<PropertyImage_Create>();

            if (model.ImageFiles != null && model.ImageFiles.Any())
            {
                foreach (var file in model.ImageFiles)
                {
                    // Save the file and get its URL � replace with your logic
                    var fileName = Path.GetFileName(file.FileName);
                    var savePath = Path.Combine("wwwroot/uploads", fileName);
                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var relativeUrl = "/uploads/" + fileName;
                    imageUrls.Add(new PropertyImage_Create { ImageUrl = relativeUrl });
                }
            }
            _Services.Models.Property.Property_Create property_Create = new _Services.Models.Property.Property_Create
            {
                OwnerId = int.Parse(ownerId),
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Location = model.Location,
                City = model.City,
                Area = model.Area,
                PropertyType = model.PropertyType,
                Bedrooms = model.Bedrooms,
                Bathrooms = model.Bathrooms,
                YearBuilt = model.YearBuilt,
                Status = model.Status,
                Amenities = model.Amenities,
                Images = imageUrls
            };

            _propertyService.CreateProperty(property_Create);
            return RedirectToAction("Profile");
        }
        [ServiceFilter(typeof(JwtAuthorizeAttribute))]
        public IActionResult MyPropertiesPartial()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Profile");
            }

            var user = _base_API_Call.GetUserInfo(email);

            if (user == null)
            {
                return RedirectToAction("Profile");
            }

            var properties = _base_API_Call.GetPropertyList(user.PropertiesId);
            return PartialView("/Views/Partial_Views/_myPropertiesPartial.cshtml", properties);
        }
        //[ServiceFilter(typeof(JwtAuthorizeAttribute))]
        //public IActionResult FavoritedPropertiesPartial()
        //{
        //    var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return RedirectToAction("Profile");
        //    }

        //    var user = _base_API_Call.GetUserInfo(email);

        //    if (user == null)
        //    {
        //        return RedirectToAction("Profile");
        //    }
        //    var properties = _base_API_Call.GetPropertyList(user.FavoriteId);
        //    return PartialView("/Views/Partial_Views/_favoritedPropertiesPartial.cshtml", properties);
        //}
        [ServiceFilter(typeof(JwtAuthorizeAttribute))]
        public IActionResult MassagesPartial()
        {
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Profile");
            }

            var user = _base_API_Call.GetUserInfo(email);

            if (user == null)
            {
                return RedirectToAction("Profile");
            }
            var massages = _base_API_Call.GetMassagesToUser(user.Id);

            return PartialView("/Views/Partial_Views/_massagesPartial.cshtml", massages);
        }


        public IActionResult SubmitPropertyPartial()
        {
            return PartialView("/Views/Partial_Views/_submitPropertyPartial.cshtml");
        }

        //public IActionResult ChangePasswordPartial()
        //{
        //    return PartialView("/Views/Partial_Views/_changePasswordPartial.cshtml");
        //}

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
