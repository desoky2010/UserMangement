using System.ComponentModel.DataAnnotations;

namespace UserMangement.Models.DTOs
{
    public class ChangePasswordModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required, Compare(nameof(NewPassword))]
        public string ConfirmNewPassword { get; set; }
    }
}
