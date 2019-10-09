using System;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
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

        /// <summary>
        ///     Deep clone using JSON serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toClone"></param>
        /// <returns></returns>
        public static T Clone<T>(this T toClone) where T : class =>
            JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(toClone));

        public static string ToShortGuidString(this Guid guid)
        {
            string enc = Convert.ToBase64String(guid.ToByteArray());
            enc = enc.Replace("/", "_");
            enc = enc.Replace("+", "-");
            return enc.Substring(0, 22);
        }

        public static bool TryDecodeToGuid(this string input, out Guid decoded)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    decoded = Guid.Empty;
                    return true;
                }
                input = input.Replace("_", "/");
                input = input.Replace("-", "+");
                byte[] buffer = Convert.FromBase64String(input + "==");
                decoded = new Guid(buffer);
                return true;
            }
            catch (Exception)
            {
                decoded = Guid.Empty;
                return false;
            }
        }
    }
}