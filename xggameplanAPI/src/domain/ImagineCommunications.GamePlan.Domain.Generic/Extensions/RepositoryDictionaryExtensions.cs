using System;
using ImagineCommunications.GamePlan.Domain.Generic.Types;

namespace ImagineCommunications.GamePlan.Domain.Generic.Extensions
{
    public static class RepositoryDictionaryExtensions
    {
        public static TRepository Get<TRepository>(this RepositoryDictionary repositories) where TRepository : class
        {
            if (repositories == null)
            {
                return null;
            }

            if (!repositories.TryGetValue(typeof(TRepository), out var repository))
            {
                throw new InvalidOperationException($"'{typeof(TRepository).Name}' type has not been created.");
            }

            return (TRepository)repository;
        }
    }
}
