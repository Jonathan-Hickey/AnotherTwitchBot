using System.Threading.Tasks;

namespace AnotherTwitchBot.Clients.Interfaces
{
    public interface ITwitchClient
    {
        void SendIrcMessage(string message);
        void SendPublicChatMessage(string message);
        Task<string> ReadMessageAsync();
    }
}