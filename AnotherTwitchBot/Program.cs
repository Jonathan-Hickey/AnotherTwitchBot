using System;
using System.Threading.Tasks;
using AnotherTwitchBot.Clients.Implementation;
using AnotherTwitchBot.Clients.Interfaces;
using AnotherTwitchBot.Enums;
using AnotherTwitchBot.Options;
using AnotherTwitchBot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AnotherTwitchBot
{
    internal class Program
    {
        private const string HelloCommand = "!hello";
        private const string ExitBotCommand = "!exitbot";
        private const char ExclamationMark = '!';

        private static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var startUp = new StartUp(configuration);
            var serviceCollection = new ServiceCollection();
            startUp.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var twitchClient = serviceProvider.GetService<ITwitchClient>();
            var twitchConfig = serviceProvider.GetService<IOptions<TwitchConfig>>().Value;
            var messageParser = serviceProvider.GetService<IMessageParser>();
            var ping = new PingSender(twitchClient);
            ping.Start();

            twitchClient.SendPublicChatMessage("Bot is active!");

            // Listen to the chat until program exits
            while (true)
            {
                // Read any message from the chat room
                var message = await twitchClient.ReadMessageAsync();
                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine(message);
                    var twitchClientCommand = messageParser.GetClientCommand(message);

                    if (twitchClientCommand != null && twitchClientCommand.Command ==  IrcCommand.PrivateMessage)
                    {
                        // Broadcaster commands
                        if (twitchClientCommand.UserName.Equals(twitchConfig.UserName))
                            if (message.Equals(ExitBotCommand))
                            {
                                twitchClient.SendPublicChatMessage("Bye! Have a beautiful time!");
                                Environment.Exit(0); // Stop the program
                            }

                        if (message.Equals(HelloCommand))
                        {
                            twitchClient.SendPublicChatMessage("Hello World!");
                        }
                    }
                }
            }
        }
    }
}