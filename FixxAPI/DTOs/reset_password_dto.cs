using System.ComponentModel.DataAnnotations;

namespace FixxAPI.DTOs
{
    public class reset_password_dto
    {
        [Required]
        public string token { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string new_password { get; set; } = string.Empty;
        [Required, Compare("new_password")]
        public string confirm_new_password { get; set; } = string.Empty;
    }
}
