using System;
using Raven.Imports.Newtonsoft.Json;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Resolvers
{
    /// <summary>
    /// Serialization binder for .net standard version of RavenDb library,
    /// instantiated dynamically through lambda expression by <see cref="ModelSerializationBinderSetter"/>.
    /// </summary>
    /// <seealso cref="Raven.Imports.Newtonsoft.Json.SerializationBinder" />
    internal sealed class ModelSerializationBinderNetCore : SerializationBinder
    {
        private readonly SerializationBinder _internalBinder;
        private readonly ModelTypeResolver _resolver;

        public ModelSerializationBinderNetCore(SerializationBinder binder, ModelTypeResolver resolver)
        {
            _internalBinder = binder;
            _resolver = resolver;
        }

        public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = serializedType.Assembly.FullName;
            typeName = serializedType.FullName;
        }

        public override Type BindToType(string assemblyName, string typeName)
        {
            return _resolver.Resolve($"{typeName}, {assemblyName}") ??
                   _internalBinder.BindToType(assemblyName, typeName);
        }
    }
}
