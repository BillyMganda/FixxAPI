using FixxAPI.DTOs;
using FixxAPI.Models;

namespace FixxAPI.Repository
{
    public interface Iadmin_service
    {
        public string get_logged_in_role();
        //property categories
        public Task<property_category> add_property_category(string name);
        public Task<IEnumerable<property_category>> get_property_categories();
        public Task<bool> delete_property_category(int id);
        //property types
        public Task<property_type> add_property_type(string name);
        public Task<IEnumerable<property_type>> get_property_types();
        public Task<bool> delete_property_type(int id);
        //prop_categ_type_amen
        public Task<IEnumerable<prop_categ_type_amen>> get_all_prop_categ_type_amen();
        public Task<prop_categ_type_amen> get_one_prop_categ_type_amen(Guid userid);
    }
}
