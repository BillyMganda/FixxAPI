using FixxAPI.DTOs;
using FixxAPI.Helper;
using FixxAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FixxAPI.Repository
{
    public class admin_service : Iadmin_service
    {
        private readonly Data_Context _context;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public admin_service(Data_Context data, IConfiguration configuration, IHttpContextAccessor accessor)
        {
            _context = data;
            _configuration = configuration;
            _httpContextAccessor = accessor;
        }

        public string get_logged_in_role()
        {
            return _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Role);
        }

        public async Task<property_category> add_property_category(string name)
        {
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            var propertyCategory = new property_category
            {
                category_name = name,
                created_by = user_id,
                created_on = DateTime.Now
            };

            var cat = _context.property_category.Add(propertyCategory);
            await _context.SaveChangesAsync();
            return cat.Entity;
        }

        public async Task<IEnumerable<property_category>> get_property_categories()
        {
           return await _context.property_category.ToListAsync();
        }

        public async Task<bool> delete_property_category(int id)
        {
            var catProp = await _context.property_category.FirstOrDefaultAsync(x => x.id == id);
            if(catProp != null)
            {
                _context.property_category.Remove(catProp);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<property_type> add_property_type(string name)
        {
            string email = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.users.FirstOrDefaultAsync(x => x.email == email);
            Guid user_id = user!.uuid;

            var propertyType = new property_type
            {
                type_name = name,
                created_by = user_id,
                created_on = DateTime.Now
            };

            var cat = _context.property_type.Add(propertyType);
            await _context.SaveChangesAsync();
            return cat.Entity;
        }

        public async Task<IEnumerable<property_type>> get_property_types()
        {
            return await _context.property_type.ToListAsync();
        }

        public async Task<bool> delete_property_type(int id)
        {
            var catProp = await _context.property_type.FirstOrDefaultAsync(x => x.id == id);
            if (catProp != null)
            {
                _context.property_type.Remove(catProp);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        //prop_categ_type_amen
        public async Task<IEnumerable<prop_categ_type_amen>> get_all_prop_categ_type_amen()
        {
            return await _context.prop_categ_type_amen.ToListAsync();
        }

        public async Task<prop_categ_type_amen> get_one_prop_categ_type_amen(Guid userid)
        {
            var results = await _context.prop_categ_type_amen.FirstOrDefaultAsync(x => x.user_id == userid);
            return results!;
        }
    }
}
