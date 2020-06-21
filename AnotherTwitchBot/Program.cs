using System;
using System.Threading.Tasks;
using AnotherTwitchBot.Clients.Implementation;
using AnotherTwitchBot.Clients.Interfaces;
using AnotherTwitchBot.Enums;
using AnotherTwitchBot.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnotherTwitchBot
{
    internal class Program
    {

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
            var messageParser = serviceProvider.GetService<IMessageParser>();
            var processUserDataService = serviceProvider.GetService<IProcessUserDataService>();
            var ping = new PingSender(twitchClient);
            ping.Start();
            
            await twitchClient.SendPublicChatMessageAsync("Bot is active!");

            // Listen to the chat until program exits
            while (true)
            {
                // Read any message from the chat room
                var message = await twitchClient.ReadMessageAsync();

                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine((string)message);

                    var command = messageParser.GetCommandType(message);

                    switch (command)
                    {
                        //    case IrcCommand.Ping:
                        //    case IrcCommand.Pong:
                        //        await twitchClient.SendIrcMessageAsync("PING twitchClient.twitchClient.tv");
                        //break;
                        case IrcCommand.PrivateMessage:
                            await processUserDataService.Process(message);
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}