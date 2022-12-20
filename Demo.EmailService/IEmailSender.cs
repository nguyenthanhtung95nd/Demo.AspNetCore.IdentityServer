using System.Threading.Tasks;

namespace Demo.EmailService
{
    public interface IEmailSender
    {
        void SendEmail(Message message);

        Task SendEmailAsync(Message message);
    }
}