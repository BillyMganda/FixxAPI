using FixxAPI.DTOs;
using FixxAPI.Helper;
using FixxAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FixxAPI.Repository
{
    public class property_service : Iproperty_service
    {
        private readonly Data_Context _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public property_service(Data_Context context, IConfiguration configuration, IHttpContextAccessor http)
        {
            _context = context;
            _configuration = configuration;
            _httpContextAccessor = http;
        }

        public string get_logged_in_role()
        {
            return _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role);            
        }

        public async Task<properties> get_property_by_user()
        {            
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;
            var result = await _context.properties.FirstOrDefaultAsync(x => x.user_id == user_id);
            return result!;
        }

        public async Task<properties> add_property_initial(property_create_dto dto)
        {
            string role = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role);
            string email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            var property = new properties
            {
                user_id = user_id,
                name = dto.name,
                description = dto.description,
                pay_per_night = dto.pay_per_night,
                adult_count = dto.adult_count,
                children_count = dto.children_count,
                bathroom_count = dto.bathroom_count,
                bedroom_count = dto.bedroom_count,
                property_type_id = dto.property_type_id,
                property_space = dto.property_space,
                property_category_id = dto.property_category_id,
                status = dto.status,
                created_on = DateTime.Now
            };

            var prop_ = _context.properties.Add(property);
            await _context.SaveChangesAsync();
            return prop_.Entity;
        }
    }
}
