using System.Threading.Tasks;

namespace App.Services.Identity
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string phoneNumber, string message);
    }
}