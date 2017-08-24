using System.Threading.Tasks;

namespace ContosoUniversity.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string email, string message);
    }
}
