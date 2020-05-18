using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Pubquiz.Domain;
using Pubquiz.Logic;
using Pubquiz.Logic.Requests;

namespace Pubquiz.WebApi.Helpers
{
    public static class Extensions
    {
        /// <summary>
        /// Add the Command, Notification and Query requests to the service collection.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="sourceAssembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddRequests(this IServiceCollection serviceCollection, Assembly sourceAssembly)
        {
            var commandTypes = (from assemblyType in sourceAssembly.GetTypes()
                where assemblyType.IsSubclassOfRawGeneric(typeof(Command<>)) && !assemblyType.IsAbstract
                select assemblyType).ToArray();
            foreach (var commandType in commandTypes)
            {
                serviceCollection.AddScoped(commandType);
            }

            var notificationTypes = (from assemblyType in sourceAssembly.GetTypes()
                where assemblyType.IsSubclassOf(typeof(Notification)) && !assemblyType.IsAbstract
                select assemblyType).ToArray();
            foreach (var notificationType in notificationTypes)
            {
                serviceCollection.AddScoped(notificationType);
            }

            var queryTypes = (from assemblyType in sourceAssembly.GetTypes()
                where assemblyType.IsSubclassOfRawGeneric(typeof(Query<>)) && !assemblyType.IsAbstract
                select assemblyType).ToArray();
            foreach (var queryType in queryTypes)
            {
                serviceCollection.AddScoped(queryType);
            }

            return serviceCollection;
        }

        /// <summary>
        /// Alternative version of <see cref="Type.IsSubclassOf"/> that supports raw generic types (generic types without
        /// any type parameters).
        /// </summary>
        /// <param name="generic">The base type class for which the check is made.</param>
        /// <param name="toCheck">To type to determine for whether it derives from <paramref name="generic"/>.</param>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }
    }
}