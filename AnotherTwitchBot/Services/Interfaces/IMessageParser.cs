using AnotherTwitchBot.Models;

namespace AnotherTwitchBot.Services.Interfaces
{
    public interface IMessageParser
    {
        TwitchClientCommand GetClientCommand(string message);
    }
}