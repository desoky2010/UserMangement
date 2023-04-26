namespace UserMangement.Models
{
    public class AuthModel
    {
        public string Message { get; set; }

        public bool IsAuthnticated { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }

        public string Token { get; set; }

        public DateTime ExpireDate { get; set; }


    }
}
