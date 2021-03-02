using System;
using System.Runtime.Serialization;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Resolvers
{
    /// <summary>
    /// Serialization binder for .net framework version of RavenDb library,
    /// instantiated dynamically through lambda expression by <see cref="ModelSerializationBinderSetter"/>.
    /// </summary>
    /// <seealso cref="System.Runtime.Serialization.SerializationBinder" />
    internal sealed class ModelSerializationBinderNetFramework : SerializationBinder
    {
        private readonly SerializationBinder _internalBinder;
        private readonly ModelTypeResolver _resolver;

        public ModelSerializationBinderNetFramework(SerializationBinder binder, ModelTypeResolver resolver)
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
