namespace UserMangement.Models.DTOs
{
    public class UsersDTO
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public DateTime? BlockedAt { get; set; }


    }
}
