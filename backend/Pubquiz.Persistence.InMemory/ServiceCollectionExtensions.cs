using System;
using Microsoft.Extensions.DependencyInjection;

namespace Pubquiz.Persistence.InMemory
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryPersistence(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddMemoryCache();
            services.AddLogging();
            services.AddSingleton<ICollectionOptions>(new InMemoryDatabaseOptions());
            services.AddScoped<IUnitOfWork, NoActionUnitOfWork>();
            return services;
        }
    }
}