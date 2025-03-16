
namespace WebApp_Identity.Services
{
    public interface IEmailService
    {
        Task SendAsync(string emailSender, string sendEmailTo, string emailSubject, string emailBody);
    }
}