using Microsoft.AspNetCore.Identity;

namespace UserMangement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? VerficationEmailToken { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpire { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? LoginToken { get; set; }
        public DateTime? LoginTokenExpire { get; set; }
        public bool? UserBlock { get; set; } = false;
        public DateTime BlockedAt { get; set; }
    }
}
