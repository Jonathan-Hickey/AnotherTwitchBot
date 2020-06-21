namespace AnotherTwitchBot.Enums
{
    public static class InternetRelayChatCommands
    {
        public const string PrivateMessage = "PRIVMSG";
        public const string Join = "JOIN";
        public const string Ping = "PING";
        public const string Pong = "PONG";
    }

    public enum IrcCommand
    {
        PrivateMessage = 1,
        Join = 2,
        Ping = 3,
        Pong = 4,
        
    }
}
