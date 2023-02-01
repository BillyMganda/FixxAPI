using FixxAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FixxAPI.Helper
{
    public class Data_Context : DbContext
    {
        public Data_Context(DbContextOptions<Data_Context> options) : base(options)
        {

        }
        public DbSet<users> users { get; set; }
        public DbSet<properties> properties { get; set; }
        public DbSet<amenities> amenities { get; set; }
        public DbSet<property_category> property_category { get; set; }
        public DbSet<property_type> property_type { get; set; }
        public DbSet<prop_categ_type_amen> prop_categ_type_amen { get; set; }
    }
}
