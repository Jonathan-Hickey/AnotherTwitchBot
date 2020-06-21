using AnotherTwitchBot.Enums;
using AnotherTwitchBot.Models;

namespace AnotherTwitchBot.Services.Interfaces
{
    public interface IMessageParser
    {
        IrcCommand GetCommandType(string message);
        UserMessageModel GetUserMessageModel(string message);
    }
}