using System;
using System.Threading.Tasks;
using AnotherTwitchBot.Clients.Interfaces;
using AnotherTwitchBot.Options;
using AnotherTwitchBot.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AnotherTwitchBot.Services.Implementation
{
    public class ProcessUserDataService : IProcessUserDataService
    {
        private readonly IMessageParser _messageParser;
        private readonly TwitchConfig _twitchConfig;
        private readonly ITwitchClient _twitchClient;
        private const string HelloCommand = "!hello";
        private const string ExitBotCommand = "!exitbot";

        public ProcessUserDataService(IMessageParser messageParser, IOptions<TwitchConfig> twitchConfigOptions, ITwitchClient twitchClient)
        {
            _twitchClient = twitchClient;
            _twitchConfig = twitchConfigOptions.Value;
            _messageParser = messageParser;
        }

        public async Task Process(string message)
        {
            var twitchClientCommand = _messageParser.GetUserMessageModel(message);

            // Broadcaster commands
            if (twitchClientCommand.UserName.Equals(_twitchConfig.UserName))
                if (twitchClientCommand.UserMessage.Equals(ExitBotCommand))
                {
                    await _twitchClient.SendPublicChatMessageAsync("Bye! Have a beautiful time!");
                    Environment.Exit(0); // Stop the program
                }

            if (twitchClientCommand.UserMessage.Equals(HelloCommand))
            {
                await _twitchClient.SendPublicChatMessageAsync("Hello World!");
            }
        }
    }
}