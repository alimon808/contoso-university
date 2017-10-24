using System.Threading.Tasks;

namespace ContosoUniversity.Common
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
