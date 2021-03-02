using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using Raven.Imports.Newtonsoft.Json;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Resolvers
{
    /// <summary>
    /// RavenDb libraries expose different types for <see cref="JsonSerializer.Binder"/> property:
    /// System.Runtime.Serialization.SerializationBinder for .NET Framework and
    /// Raven.Imports.Newtonsoft.Json.SerializationBinder for .NET Standard.
    /// On early binding .NET Standard RavenDb library is used but having been a part of .NET Framework
    /// application, the current project references .NET Framework RavenDb library and <see cref="TypeLoadException"/>
    /// is thrown in runtime. <see cref="ModelSerializationBinderSetter"/> is intended to resolve
    /// SerializationBinder type correctly on late binding according to the current execution framework.
    /// </summary>
    internal sealed class ModelSerializationBinderSetter
    {
        private const string DotNetFramework = ".NET Framework";
        private const string DotNetCore = ".NET Core";

        private readonly Action<JsonSerializer> _setBinderAction;

        private Action<JsonSerializer> CompileAction(ModelTypeResolver modelTypeResolver)
        {
            Type modelBinderType;
            var framework = RuntimeInformation.FrameworkDescription;

            if (framework.StartsWith(DotNetFramework))
            {
                modelBinderType = Assembly.GetExecutingAssembly()
                    .GetType($"{GetType().Namespace}.{nameof(ModelSerializationBinderNetFramework)}");
            }
            else if (framework.StartsWith(DotNetCore))
            {
                modelBinderType = Assembly.GetExecutingAssembly()
                    .GetType($"{GetType().Namespace}.{nameof(ModelSerializationBinderNetCore)}");
            }
            else
            {
                throw new NotSupportedException(
                    $"'Substitution of json serializer binder for the RavenDb isn't supported under '{framework}' framework.");
            }

            // Creates lambda expression delegate
            // (serializer) => serializer.Binder = new AppropriateSerializerBinder(serializer.Binder, modelTypeResolver)
            var p = Expression.Parameter(typeof(JsonSerializer));
            var binderProperty = Expression.Property(p, nameof(JsonSerializer.Binder));

            var binderSetter = Expression.Assign(binderProperty,
                Expression.New(modelBinderType.GetConstructors().Single(),
                    binderProperty,
                    Expression.Constant(modelTypeResolver)));

            return Expression.Lambda<Action<JsonSerializer>>(binderSetter, p).Compile();
        }

        public ModelSerializationBinderSetter(ModelTypeResolver modelTypeResolver)
        {
            if (modelTypeResolver is null)
            {
                throw new ArgumentNullException(nameof(modelTypeResolver));
            }

            _setBinderAction = CompileAction(modelTypeResolver);
        }

        public void Set(JsonSerializer serializer)
        {
            _setBinderAction?.Invoke(serializer);
        }
    }
}
