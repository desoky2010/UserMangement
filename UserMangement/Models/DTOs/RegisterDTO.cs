using System.ComponentModel.DataAnnotations;

namespace UserMangement.Models.DTOs
{
    public class RegisterDTO
    {
        [Required ,  StringLength(50)]
        public string UserName { get; set; }

        [Required , EmailAddress]
        public string Email { get; set; }

        [Required ]
        public string Password { get; set; }
        [Required , Compare("Password")]
        public string ConfirmPassword { get; set; }

        
    }
}
