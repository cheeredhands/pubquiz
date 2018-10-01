using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pubquiz.Persistence.MongoDb;
using Pubquiz.Persistence.NoAction;

namespace Pubquiz.Persistence.Extensions
{
    public static class Extensions
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

      public static IServiceCollection AddMongoDbPersistence(this IServiceCollection services, string databaseName, string connectionString)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddMemoryCache();
            services.AddLogging();
            services.AddSingleton<ICollectionOptions>(new MongoDbDatabaseOptions(databaseName, connectionString));
            services.AddScoped<IUnitOfWork, MongoDbUnitOfWork>();
            return services;
        }
        
        /// <summary>
        ///     Deep clone using JSON serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toClone"></param>
        /// <returns></returns>
        public static T Clone<T>(this T toClone) where T : class =>
            JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(toClone));
    }
}
