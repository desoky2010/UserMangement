using System.ComponentModel.DataAnnotations;

namespace UserMangement.Models.DTOs
{
    public class ResetPasswordModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string Password { get; set; }
        [Required , Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
