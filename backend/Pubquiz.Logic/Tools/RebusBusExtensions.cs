using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rebus.Bus;
using Rebus.Handlers;

namespace Pubquiz.Logic.Tools
{
    public static class RebusBusExtensions
    {
        /// <summary>
        ///     Scans the assemblies supplied in <paramref name="assemblies" /> for handlers that implement
        ///     <see cref="IHandleMessages{TMessage}" /> and adds a subscription for the handled message types.
        /// </summary>
        public static void SubscribeByScanningForHandlers(this IBus bus, params Assembly[] assemblies)
        {
            foreach (var messageType in GetTypesOfMessagesHandledByRebus(assemblies))
            {
                bus.Subscribe(messageType).Wait();
            }
        }
        static IEnumerable<Type> GetTypesOfMessagesHandledByRebus(Assembly[] assemblies)
        {
            return assemblies.SelectMany(x => x.GetTypes())
                .SelectMany(x => x.GetInterfaces())
                .Where(IsGenericRebusHandler)
                .SelectMany(x => x.GenericTypeArguments)
                .Distinct();
        }
        static bool IsGenericRebusHandler(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof (IHandleMessages<>);
        }
    }
}