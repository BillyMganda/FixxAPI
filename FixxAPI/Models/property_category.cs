using System.ComponentModel.DataAnnotations;

namespace FixxAPI.Models
{
    public class property_category
    {
        [Key]
        public int id { get; set; }
        public string category_name { get; set; } = string.Empty;
        public Guid created_by { get; set; }
        public DateTime created_on { get; set; }
    }
}
