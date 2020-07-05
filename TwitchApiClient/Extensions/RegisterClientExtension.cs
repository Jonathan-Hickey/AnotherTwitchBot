using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitchApiClient.Options;

namespace TwitchApiClient.Extensions
{
    public static class RegisterClientExtension
    {
        public static IServiceCollection AddTwitchClients(this IServiceCollection serviceCollection, IConfigurationSection twitchApiConfigConfigurationSection)
        {
            serviceCollection.Configure<TwitchApiConfig>(twitchApiConfigConfigurationSection);
            serviceCollection.AddHttpClient<ITwitchApiUserClient, TwitchApiUserClient>();
            serviceCollection.AddHttpClient<ITwitchApiFollowerClient, TwitchApiFollowerClient>();
            serviceCollection.AddHttpClient<IAuthenticationClient, AuthenticationClient>();
            serviceCollection.AddMemoryCache();
            return serviceCollection;
        }
    }
}
