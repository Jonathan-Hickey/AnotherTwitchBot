using System.Threading;
using AnotherTwitchBot.Clients.Interfaces;

namespace AnotherTwitchBot.Clients.Implementation
{
    public class PingSender : IPingSender
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