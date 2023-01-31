namespace FixxAPI.Models
{
    public class properties
    {
        public int id { get; set; }
        public Guid user_id { get; set; }
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public decimal pay_per_night { get; set; }
        public int adult_count { get; set; }
        public int children_count { get; set; }
        public int bathroom_count { get; set; }
        public int bedroom_count { get; set; }
        public int property_type_id { get; set; }
        public string property_space { get; set; } = string.Empty;
        public int property_category_id { get; set; }
        public string status { get; set; } = string.Empty;
        public DateTime created_on { get; set; }
    }
}
