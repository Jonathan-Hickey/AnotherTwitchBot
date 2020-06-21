using AnotherTwitchBot.Clients.Implementation;
using AnotherTwitchBot.Clients.Interfaces;
using AnotherTwitchBot.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnotherTwitchBot
{
    public class StartUp
    {
        private readonly IConfigurationRoot _configurationRoot;

        public StartUp(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }
        public void ConfigureServices(IServiceCollection service)
        {
            service.Configure<TwitchConfig>(_configurationRoot.GetSection("twitch_irc"));
            service.AddSingleton<ITwitchClient, TwitchClient>();
        }
    }
}