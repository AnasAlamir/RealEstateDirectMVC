using MVC_Project.Models;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using System.Security.Policy;
using Newtonsoft.Json;
using System.Text;
using _Services.Contracts;

namespace MVC_Project.API_Services
{
    public interface IBase_API_Call
    {
        // Any common functionality for API calls can be placed here
        IEnumerable<_Services.Models.City.City_Get> GetAllCity();
        _Services.Models.Property.Property_AllInfo GetPropertyById(int Id);
        IEnumerable<_Services.Models.Property.Properties_List> GetPropertyList();
        IEnumerable<_Services.Models.Property.Properties_List> GetPropertyList(IEnumerable<int> Id);
        IEnumerable<_Services.Models.Property.Properties_List> GetFilteredProperties(Filter filter);
        Task CreateUserAsync(_Services.Models.User.User_Create user);
        void UpdateUserInfo(_Services.Models.User.User_Basic userInfo);
        _Services.Models.User.User_Basic GetUserInfo(string email);
        IEnumerable<_Services.Models.Inquiry.Inquity_List> GetMassagesToUser(int Id);
        IEnumerable<_Services.Models.Inquiry.Inquity_List> GetMassagesToProperty(int Id);



    }



    internal class Base_API_Call : IBase_API_Call
    {
        protected readonly ICityService _cityService;
        protected readonly IPropertyService _propertyService;
        protected readonly IUserService _userService;
        protected readonly IInquiryService _inquiryService;

        public Base_API_Call(IPropertyService propertyService, ICityService cityService, IUserService userService, IInquiryService inquiryService)
        {
            _propertyService = propertyService;
            _cityService = cityService;
            _userService = userService;
            _inquiryService = inquiryService;
        }

        public IEnumerable<_Services.Models.City.City_Get> GetAllCity()
        {
            try
            {
                return _cityService.GetCities();
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new ApplicationException("An error occurred while fetching city data.", ex);
            }
        }

        public _Services.Models.Property.Property_AllInfo GetPropertyById(int Id)
        {
            try
            {
                return _propertyService.GetPropertyById(Id);

            }
            catch (Exception ex)
            {
                // Log exception here
                throw new ApplicationException("An error occurred while fetching city data.", ex);
            }
        }
        public IEnumerable<_Services.Models.Property.Properties_List> GetPropertyList()
        {
            try
            {
                return _propertyService.GetPropertyList().OrderByDescending(U => U.DateAdded)
                                       .ThenBy(U => U.Price);

            }
            catch (Exception ex)
            {
                // Log exception here
                throw new ApplicationException("An error occurred while fetching city data.", ex);
            }
        }

        public IEnumerable<_Services.Models.Property.Properties_List> GetPropertyList(IEnumerable<int> Id)
        {
            try
            {
                return _propertyService.GetPropertyList().Where(x => Id.Contains(x.Id)).OrderByDescending(U => U.DateAdded)
                                       .ThenBy(U => U.Price);
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new ApplicationException("An error occurred while fetching city data.", ex);
            }
        }

        public IEnumerable<_Services.Models.Property.Properties_List> GetFilteredProperties(Filter filter)
        {
            var url = $"Property/GetPropertiesWithFilter?keyword={filter.Keyword}&city={filter.City}&status={filter.Status}&maxPrice={filter.PriceRange}&maxArea={filter.AreaSize}&minBaths={filter.Baths}&minBed={filter.Beds}&HasGarage={filter.HasGarage}&Two_Stories={filter.Two_Stories}&Laundry_Room={filter.Laundry_Room}&HasPool={filter.HasPool}&HasGarden={filter.HasGarden}&HasElevator={filter.HasElevator}&HasBalcony={filter.HasBalcony}&HasParking={filter.HasParking}&HasCentralHeating={filter.HasCentralHeating}&IsFurnished={filter.IsFurnished}";

            try
            {
                var result = _propertyService.GetPropertiesWithFilter(filter.Keyword, filter.City, (application.DataAccess.Models.Status?)filter.Status,
                    filter.PriceRange, filter.AreaSize, filter.Beds, filter.Baths,
                    filter.HasGarage, filter.Two_Stories, filter.Laundry_Room,
                    filter.HasPool, filter.HasGarden, filter.HasElevator,
                    filter.HasBalcony, filter.HasParking, filter.HasCentralHeating,
                    filter.IsFurnished);
                return result.OrderByDescending(U => U.DateAdded)
                                       .ThenBy(U => U.Price);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error URL: " + url);  // Log the exact URL
                Console.WriteLine("Error Message: " + ex.Message);  // Log the error message
                throw new ApplicationException("An error occurred while fetching filtered properties.", ex);
            }
        }



        public async Task CreateUserAsync(_Services.Models.User.User_Create user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object cannot be null");
            }

            try
            {
                _userService.CreateUser(user);
                // تسجيل معلومات في حالة نجاح الطلب
                Console.WriteLine($"User {user.Email} created successfully.");
            }
            catch (HttpRequestException httpEx)
            {
                // في حالة وجود خطأ HTTP
                Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
                throw new ApplicationException("An error occurred while creating the user.", httpEx);
            }
            catch (Exception ex)
            {
                // معالجة الأخطاء الأخرى
                Console.WriteLine($"General Error: {ex.Message}");
                throw new ApplicationException("An error occurred while processing the request.", ex);
            }
        }
        public void UpdateUserInfo(_Services.Models.User.User_Basic userInfo)
        {
            // تأكد من ملء خصائص المستخدم
            _Services.Models.User.User_Update user = new _Services.Models.User.User_Update
            {
                F_Name = userInfo.F_Name,
                L_Name = userInfo.L_Name,
                PhoneNumber = userInfo.PhoneNumber,
                Email = userInfo.Email,
                Address = userInfo.Address,
                ProfilePicture = "" // تأكد من إضافة هذه الخاصية إذا كانت موجودة في User_Update
            };

            _userService.UpdateUser(userInfo.Id, user);  // تأكد من أن هذه الطريقة موجودة في IUserService
        }



        public _Services.Models.User.User_Basic GetUserInfo(string email)
        {
            try
            {
                return _userService.GetUserByEmail(email);
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new ApplicationException("An error occurred while fetching city data.", ex);
            }
        }

        public IEnumerable<_Services.Models.Inquiry.Inquity_List> GetMassagesToUser(int Id)
        {
            try
            {
                return _inquiryService.GetInquitiesToUser(Id).OrderByDescending(U => U.DateSent);
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new ApplicationException("An error occurred while fetching city data.", ex);
            }
        }

        public IEnumerable<_Services.Models.Inquiry.Inquity_List> GetMassagesToProperty(int Id)
        {
            try
            {
                return _inquiryService.GetInquitiesToPropety(Id).OrderByDescending(U => U.DateSent);
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new ApplicationException("An error occurred while fetching city data.", ex);
            }
        }

    }
}

    