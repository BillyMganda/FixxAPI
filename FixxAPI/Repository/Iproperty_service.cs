using FixxAPI.DTOs;
using FixxAPI.Models;

namespace FixxAPI.Repository
{
    public interface Iproperty_service
    {
        public string get_logged_in_role();
        public Task<properties> get_property_by_user();
        public Task<properties> add_property_initial(property_create_dto dto);
    }
}
