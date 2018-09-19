using System;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pubquiz.Repository.Mongo;
using Pubquiz.Repository.NoAction;

namespace Pubquiz.Repository.Extensions
{
    public static class Extensions
    {
        public static IServiceCollection AddInMemoryRepository(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddMemoryCache();
            services.AddLogging();
            services.AddSingleton<IRepositoryOptions>(new InMemoryDatabaseOptions());
            services.AddScoped<IRepositoryFactory, NoActionFactory>();
            return services;
        }

      public static IServiceCollection AddMongoRepository(this IServiceCollection services, string databaseName, string connectionString)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            services.AddMemoryCache();
            services.AddLogging();
            services.AddSingleton<IRepositoryOptions>(new MongoDatabaseOptions(databaseName, connectionString));
            services.AddScoped<IRepositoryFactory, MongoFactory>();
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
