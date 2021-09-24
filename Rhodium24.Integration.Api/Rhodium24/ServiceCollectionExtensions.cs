using Microsoft.Extensions.Configuration;
using Rhodium24.Integration.Api.Rhodium24;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRhodium24Helper(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<RhodiumSettings>(configuration.GetSection("Rhodium24").Bind);
            serviceCollection.AddSingleton<RhodiumHelper>();
            return serviceCollection;
        }
    }
}
