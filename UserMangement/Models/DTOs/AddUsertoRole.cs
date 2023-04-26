using System.ComponentModel.DataAnnotations;

namespace UserMangement.Models.DTOs
{
    public class AddUsertoRole
    {
        //  public string userId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string roleName { get; set; }
    }
}
