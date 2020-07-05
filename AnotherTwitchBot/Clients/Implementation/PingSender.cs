using System.Threading;
using AnotherTwitchBot.Clients.Interfaces;

namespace AnotherTwitchBot.Clients.Implementation
{
    public class PingSender : IPingSender
    {
        private ITwitchIrcClient _twitchIrcClient;
        private Thread pingSender;

        // Empty constructor makes instance of Thread
        public PingSender(ITwitchIrcClient twitchIrcClient)
        {
            _twitchIrcClient = twitchIrcClient;
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
                _twitchIrcClient.SendIrcMessage("PING twitchClient.twitchClient.tv");
                Thread.Sleep(300000); // 5 minutes
            }
        }
    }
}