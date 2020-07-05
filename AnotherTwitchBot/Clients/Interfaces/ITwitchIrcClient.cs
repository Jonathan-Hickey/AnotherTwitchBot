using System.Threading.Tasks;

namespace AnotherTwitchBot.Clients.Interfaces
{
    public interface ITwitchIrcClient
    {
        Task SendIrcMessageAsync(string message);
        Task SendPublicChatMessageAsync(string message);
        Task<string> ReadMessageAsync();
        void SendIrcMessage(string message);
    }
}