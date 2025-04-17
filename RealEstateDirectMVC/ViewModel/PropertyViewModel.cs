using MVC_Project.Models;

namespace MVC_Project.ViewModel
{
    public class PropertyViewModel
    {
        public IEnumerable<_Services.Models.Property.Properties_List> Properties { get; set; }
        public IEnumerable<_Services.Models.City.City_Get> Cites { get; set; }
    }
}
