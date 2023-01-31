using System.ComponentModel.DataAnnotations;

namespace FixxAPI.DTOs
{
    public class reset_password_dto
    {
        [Required]
        public string token { get; set; } = string.Empty;
        [Required]
        public string new_password { get; set; } = string.Empty;
        [Required]
        public string confirm_new_password { get; set; } = string.Empty;
    }
}
