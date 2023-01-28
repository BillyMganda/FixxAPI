using System.ComponentModel.DataAnnotations;

namespace FixxAPI.DTOs
{
    public class login_dto
    {
        [Required, EmailAddress]
        public string email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string password { get; set; } = string.Empty;
    }
}
