using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AnotherTwitchBot
{
    class Program
    {
        // Bot settings
        private static IConfigurationRoot _configuration;

        static async Task Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();


            var startUp = new StartUp();
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

        public class StartUp
        {
            public void ConfigureServices(IServiceCollection service)
            {
                service.Configure<TwitchConfig>(_configuration.GetSection("twitch_irc"));
                service.AddSingleton<ITwitchClient, TwitchClient>();
            }
        }


    }
    public class TwitchConfig
    {
        public string UserName { get; set; }
        public string Channel { get; set; }
        public string OAuthToken { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }

    }


    public interface ITwitchClient
    {
        void SendIrcMessage(string message);
        void SendPublicChatMessage(string message);
        Task<string> ReadMessageAsync();
    }


    public class TwitchClient : ITwitchClient
    {
        private TcpClient _tcpClient;
        private StreamReader _inputStream;
        private StreamWriter _outputStream;
        private NetworkStream _networkStream;
        private readonly TwitchConfig _twitchConfig;


        public TwitchClient(IOptions<TwitchConfig> twitchConfigOptions)
        {
            try
            {
                _twitchConfig = twitchConfigOptions.Value;

                _tcpClient = new TcpClient(_twitchConfig.Server, _twitchConfig.Port);
                _networkStream = _tcpClient.GetStream();
                _inputStream = new StreamReader(_networkStream);
                _outputStream = new StreamWriter(_networkStream) { NewLine = "\r\n", AutoFlush = true }; ;


                // Try to join the room
                _outputStream.WriteLine("PASS " + _twitchConfig.OAuthToken);
                _outputStream.WriteLine("NICK " + _twitchConfig.UserName);
                _outputStream.WriteLine("USER " + _twitchConfig.UserName + " 8 * :" + _twitchConfig.UserName);
                _outputStream.WriteLine("JOIN #" + _twitchConfig.Channel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
     
        public void SendIrcMessage(string message)
        {
            try
            {
                _outputStream.WriteLine(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendPublicChatMessage(string message)
        {
            try
            {
                SendIrcMessage(":" + _twitchConfig.UserName + "!" + _twitchConfig.UserName + "@" + _twitchConfig.UserName +
                               ".twitchClient.chat.twitchClient.tv PRIVMSG #" + _twitchConfig.Channel + " :" + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<string> ReadMessageAsync()
        {
            try
            {
                if (_networkStream.DataAvailable)
                {
                    return await _inputStream.ReadLineAsync();
                }

                await Task.Delay(100);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return "Error receiving message: " + ex.Message;
            }
        }
    }


    /*
    * Class that sends PING to twitchClient server every 5 minutes
    */
    public class PingSender
    {
        private ITwitchClient _twitchClient;
        private Thread pingSender;

        // Empty constructor makes instance of Thread
        public PingSender(ITwitchClient twitchClient)
        {
            _twitchClient = twitchClient;
            pingSender = new Thread(new ThreadStart(this.Run));
        }

        // Starts the thread
        public void Start()
        {
            pingSender.IsBackground = true;
            pingSender.Start();
        }

        // Send PING to twitchClient server every 5 minutes
        public void Run()
        {
            while (true)
            {
                _twitchClient.SendIrcMessage("PING twitchClient.twitchClient.tv");
                Thread.Sleep(300000); // 5 minutes
            }
        }
    }



    
}