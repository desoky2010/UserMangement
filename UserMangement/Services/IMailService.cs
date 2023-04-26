namespace UserMangement.Services
{
    public interface IMailService
    {
        Task SendEmail(string mailto , string subject, string body);
    }
}
