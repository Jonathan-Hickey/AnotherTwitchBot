using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AnotherTwitchBot.Clients.Interfaces;
using AnotherTwitchBot.Models;
using AnotherTwitchBot.Options;
using AnotherTwitchBot.Services.Interfaces;
using Microsoft.Extensions.Options;
using TwitchApiClient;

namespace AnotherTwitchBot.Services.Implementation
{
    public class ProcessUserDataService : IProcessUserDataService
    {
        private const string HelloCommand = "!hello";
        private const string ExitBotCommandText = "!exitbot";
        private const string StatsCommandText = "!stats";
        private readonly IMessageParser _messageParser;
        private readonly ITwitchIrcClient _twitchIrcClient;
        private readonly TwitchIrcConfig _twitchIrcConfig;

        private readonly ConcurrentDictionary<string, int> Users = new ConcurrentDictionary<string, int>();
        private readonly ITwitchApiUserClient _twitchApiUserClient;
        private readonly ITwitchApiFollowerClient _twitchApiFollowerClient;

        public ProcessUserDataService(IMessageParser messageParser, 
            IOptions<TwitchIrcConfig> twitchConfigOptions,
            ITwitchIrcClient twitchIrcClient,
            ITwitchApiUserClient twitchApiUserClient,
            ITwitchApiFollowerClient twitchApiFollowerClient )
        {
            _twitchApiFollowerClient = twitchApiFollowerClient;
            _twitchApiUserClient = twitchApiUserClient;
            _twitchIrcClient = twitchIrcClient;
            _twitchIrcConfig = twitchConfigOptions.Value;
            _messageParser = messageParser;
        }

        public async Task Process(string message)
        {
            var twitchClientCommand = _messageParser.GetUserMessageModel(message);

            // Broadcaster commands
            await ExitBotCommand(twitchClientCommand);

            await WelcomeCommand(twitchClientCommand);

            var numberOfMessages = Users[twitchClientCommand.UserName];
            Users.TryUpdate(twitchClientCommand.UserName, numberOfMessages + 1, numberOfMessages);

            await StatsCommand(twitchClientCommand);
        }

        private async Task ExitBotCommand(UserMessageModel twitchClientCommand)
        {
            if (twitchClientCommand.UserName.Equals(_twitchIrcConfig.UserName))
                if (twitchClientCommand.UserMessage.Equals(ExitBotCommandText))
                {
                    await _twitchIrcClient.SendPublicChatMessageAsync("Bye! Have a beautiful time!");
                    Environment.Exit(0); // Stop the program
                }
        }

        private async Task WelcomeCommand(UserMessageModel twitchClientCommand)
        {
            if (!Users.ContainsKey(twitchClientCommand.UserName))
            {
                var user = await _twitchApiUserClient.GetUser(twitchClientCommand.UserName);

                var isFollower = await _twitchApiFollowerClient.IsUserAFollower(user.Id);

                if (isFollower)
                {
                    await _twitchIrcClient.SendPublicChatMessageAsync($"Hi {twitchClientCommand.UserName}! Welcome back to the stream!");
                }
                else
                {
                    await _twitchIrcClient.SendPublicChatMessageAsync($"Hello and Welcome {twitchClientCommand.UserName}");
                }

                Users.TryAdd(twitchClientCommand.UserName, 0);
            }
        }

        private async Task StatsCommand(UserMessageModel twitchClientCommand)
        {
            if (twitchClientCommand.UserMessage.Equals(StatsCommandText))
            {
                var numberOfMessages = Users[twitchClientCommand.UserName];
                await _twitchIrcClient.SendPublicChatMessageAsync(
                    $"{twitchClientCommand.UserName} has sent {numberOfMessages} this stream");
            }
        }
    }
}