using System;
using System.Threading.Tasks;
using AnotherTwitchBot.Clients.Implementation;
using AnotherTwitchBot.Clients.Interfaces;
using AnotherTwitchBot.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AnotherTwitchBot
{
    class Program
    {
       

        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();


            var startUp = new StartUp(configuration);
            var serviceCollection = new ServiceCollection();
            startUp.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            
            var twitchClient = serviceProvider.GetService<ITwitchClient>();
            var twitchConfig = serviceProvider.GetService<IOptions<TwitchConfig>>().Value;

            // Ping to the server to make sure this bot stays connected to the chat
            // Server will respond back to this bot with a PONG (without quotes):
            // Example: ":tmi.twitchClient.tv PONG tmi.twitchClient.tv :twitchClient.twitchClient.tv"
            PingSender ping = new PingSender(twitchClient);
            ping.Start();

            twitchClient.SendPublicChatMessage("Bot is active!");

            // Listen to the chat until program exits
            while (true)
            {   
                // Read any message from the chat room
                string message = await twitchClient.ReadMessageAsync();
                if (!string.IsNullOrEmpty(message))
                {
                    Console.WriteLine(message); // Print raw twitchClient messages

                    if (message.Contains("PRIVMSG"))
                    {
                        // Messages from the users will look something like this (without quotes):
                        // Format: ":[user]![user]@[user].tmi.twitchClient.tv PRIVMSG #[channel] :[message]"

                        // Modify message to only retrieve user and message
                        int intIndexParseSign = message.IndexOf('!');
   

                        string userName =
                            message.Substring(1,
                                intIndexParseSign - 1); // parse username from specific section (without quotes)
                        // Format: ":[user]!"
                        // Get user's message
                        intIndexParseSign = message.IndexOf(" :");
                        message = message.Substring(intIndexParseSign + 2);

                        //Console.WriteLine(message); // Print parsed twitchClient message (debugging only)

                        // Broadcaster commands
                        if (userName.Equals(twitchConfig.UserName))
                        {
                            if (message.Equals("!exitbot"))
                            {
                                twitchClient.SendPublicChatMessage("Bye! Have a beautiful time!");
                                Environment.Exit(0); // Stop the program
                            }
                        }

                        // General commands anyone can use
                        if (message.Equals("!hello"))
                        {
                            twitchClient.SendPublicChatMessage("Hello World!");
                        }
                    }
                }
            }
        }
    }
}