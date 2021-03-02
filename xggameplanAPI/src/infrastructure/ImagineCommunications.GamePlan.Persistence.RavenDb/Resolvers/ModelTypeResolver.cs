using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Resolvers
{
    /// <summary>
    /// During decoupling stage (June 2019) some model classes which represent RavenDb data
    /// were moved into different library. Due to the fact that RavenDb can save full type name
    /// of a model into collection item metadata, sometimes it's necessary to resolve model type
    /// by it's previous full type name.
    /// </summary>
    internal sealed class ModelTypeResolver
    {
        private readonly IDictionary<string, Type> _typeDictionary = new Dictionary<string, Type>();

        public void Register<T>(string qualifiedTypeName)
        {
            _typeDictionary.Add(qualifiedTypeName, typeof(T));
        }

        public Type Resolve(string qualifiedTypeName)
        {
            return qualifiedTypeName == null ? null :
                _typeDictionary.TryGetValue(qualifiedTypeName, out var type) ? type : null;
        }
    }
}
