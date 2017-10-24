using System.Threading.Tasks;

namespace ContosoUniversity.Common
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string email, string message);
    }
}
