using System;
using Microsoft.Extensions.DependencyInjection;

namespace Pubquiz.Persistence.MongoDb
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDbPersistence(this IServiceCollection services, string databaseName,
            string connectionString)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddMemoryCache();
            services.AddLogging();
            services.AddSingleton<ICollectionOptions>(new MongoDbDatabaseOptions(databaseName, connectionString));
            services.AddScoped<IUnitOfWork, MongoDbUnitOfWork>();
            return services;
        }
    }
}