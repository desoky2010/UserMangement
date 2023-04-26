using System.ComponentModel.DataAnnotations;

namespace UserMangement.Models.DTOs
{
    public class LoginDTO
    {
       

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? OTP { get; set; }
    }
}
