using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.BusClient.Implementation.BigMessage
{
    /// <summary>
    /// Provides various <see cref="IBigMessage"/> related extensions methods.
    /// </summary>
    public static class BigMessageExtensions
    {
        private static readonly Lazy<List<Type>> EventTypes = new Lazy<List<Type>>(GetEventTypes, true);

        public static bool IsBulkEvent<T>(this T eventData)
            where T : class, IEvent
        {
            var result = typeof(T).GetInterfaces()
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(typeDefinition => typeDefinition == typeof(IBulkEvent<>));

            return result;
        }

        public static Type GetPayloadType(this IBigMessage bigMessage)
        {
            var messageType = bigMessage.Type;
            if (messageType.IsInterface)
            {
                var inheritors = (from assemblyType in EventTypes.Value
                                  where messageType.IsAssignableFrom(assemblyType) && messageType != assemblyType
                                  select assemblyType).ToList();
                if (inheritors.Count != 1)
                {
                    throw new InvalidOperationException($"Incorrect inheritors number {inheritors.Count}");
                }

                messageType = inheritors[0];
            }

            return messageType;
        }

        private static List<Type> GetEventTypes()
        {
            var assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies().Select(Assembly.Load);
            var types = (from domainAssembly in assemblies
                         from assemblyType in domainAssembly.GetExportedTypes()
                         where assemblyType.IsClass && !assemblyType.IsAbstract && typeof(IEvent).IsAssignableFrom(assemblyType)
                         select assemblyType).ToList();

            return types;
        }
    }
}
