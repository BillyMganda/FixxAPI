using System.ComponentModel.DataAnnotations;

namespace FixxAPI.Models
{
    public class users
    {
        [Key]
        public string uuid { get; set; } = Guid.NewGuid().ToString();
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string phone_number { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string zip_code { get; set; } = string.Empty;
        public byte[] password_hash { get; set; } = new byte[32];
        public byte[] password_salt { get; set; } = new byte[32];
        public DateTime created_on { get; set; } = DateTime.Now;
        public string forgot_password_token { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public bool terms_agreed { get; set; } = true;
    }
}
