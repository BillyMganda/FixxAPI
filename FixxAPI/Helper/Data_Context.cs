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
    }
}
