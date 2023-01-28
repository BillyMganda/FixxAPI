using System.ComponentModel.DataAnnotations;

namespace FixxAPI.DTOs
{
    public class user_add_dto
    {
        [Required]
        public string first_name { get; set; } = string.Empty;
        [Required]
        public string last_name { get; set; } = string.Empty;
        [Required]
        public string phone_number { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string email { get; set; } = string.Empty;
        [Required]
        public string address { get; set; } = string.Empty;
        [Required]
        public string city { get; set; } = string.Empty;
        [Required]
        public string state { get; set; } = string.Empty;
        [Required]
        public string zip_code { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string password { get; set; } = string.Empty;
        [Required, MinLength(6), Compare("password")]
        public string confirm_password { get; set; } = string.Empty;
        [Required]
        public bool terms_agreed { get; set; } = true;
    }
}
