using AnotherTwitchBot.Enums;

namespace AnotherTwitchBot.Models
{
    public class TwitchClientCommand
    {
        public string UserName { get; set; }
        public string Channel { get; set; }
        public string UserMessage { get; set; }
        public IrcCommand Command { get; set; }
    }
}
