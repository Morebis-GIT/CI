using System;
using System.Linq;
using System.Reflection;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Converters;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Resolvers;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Client.NodaTime.JsonConverters;
using Raven.Client.UniqueConstraints;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb
{
    /// <summary>
    /// Document store factory.
    ///
    /// TODO: Move this to separate class library.
    /// </summary>
    public static class DocumentStoreFactory
    {
        public static IDocumentStore CreateStore(string connectionString, params Assembly[] assemblyForIndexingTasks)
        {
            var customizeJsonSerializerFactory = new CustomizeJsonSerializerFactory();
            var documentStore = new DocumentStore();

            documentStore.ParseConnectionString(connectionString);
            ConfigureUniqueConstraints(documentStore);
            ConfigureIdTypeConverters(documentStore);
            ConfigureUnionTypeConverter(documentStore);
            ConfigureListeners(documentStore);
            ConfigureForCustomContractResolver(documentStore);
            documentStore.Initialize();
            CreateIndexes(documentStore, assemblyForIndexingTasks);
            ConfigureMaxRequest(documentStore);
            ConfigureForNodaTime(customizeJsonSerializerFactory);
            ConfigureTypeResolver(documentStore, customizeJsonSerializerFactory);

            documentStore.Conventions.CustomizeJsonSerializer = serializer => customizeJsonSerializerFactory.Execute(serializer);
            return documentStore;
        }

        private static void CreateIndexes(IDocumentStore documentStore, params Assembly[] assemblies)
        {
            if (assemblies is null || assemblies.Length == 0)
            {
                return;
            }

            foreach (var assembly in assemblies.Where(a => !(a is null)))
            {
                var indexes = assembly.GetExportedTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(AbstractIndexCreationTask)))
                    .Select(t => (AbstractIndexCreationTask)Activator.CreateInstance(t)).ToList();
                documentStore.ExecuteIndexes(indexes);

                foreach (var transformer in assembly.GetExportedTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(AbstractTransformerCreationTask)))
                    .Select(t => (AbstractTransformerCreationTask)Activator.CreateInstance(t)))
                {
                    documentStore.ExecuteTransformer(transformer);
                }
            }
        }

        private static void ConfigureForNodaTime(CustomizeJsonSerializerFactory jsonSerializerFactory)
        {
            jsonSerializerFactory.Add(serializer =>
            {
                serializer.Converters.Add(new NodaTimeDurationTicksJsonConverter());
            });
        }

        private static void ConfigureForCustomContractResolver(IDocumentStore documentStore)
        {
            // Not necessary at the moment as we won't be storing F# obhec
        }

        private static void ConfigureUniqueConstraints(DocumentStore documentStore)
        {
            _ = documentStore.RegisterListener(new UniqueConstraintsStoreListener());
        }

        private static void ConfigureMaxRequest(IDocumentStore documentStore)
        {
            documentStore.Conventions.MaxNumberOfRequestsPerSession = 500000;
        }

        private static void ConfigureUnionTypeConverter(IDocumentStore documentStore)
        {
            // Not necessary at the moment as we won't be storing F# obhec
        }

        private static void ConfigureIdTypeConverters(IDocumentStore documentStore)
        {
            documentStore.Conventions.FindIdentityProperty = prop =>
            {
                if (prop.DeclaringType == typeof(Spot) && prop.Name == "CustomId")
                {
                    return true;
                }

                return prop.DeclaringType != typeof(Spot) && prop.Name == "Id";
            };
        }

        private static void ConfigureListeners(IDocumentStore documentStore) { }

        private static void ConfigureTypeResolver(IDocumentStore documentStore, CustomizeJsonSerializerFactory jsonSerializerFactory)
        {
            var modelTypeResolver = new ModelTypeResolver();
            var modelBinderSetter = new ModelSerializationBinderSetter(modelTypeResolver);

            modelTypeResolver.Register<SmoothConfiguration>(
                "xggameplan.SmoothProcessing.SmoothConfiguration, xggameplan.core");
            modelTypeResolver.Register<SmoothPassDefault>(
                "xggameplan.SmoothProcessing.SmoothPassDefault, xggameplan.core");
            modelTypeResolver.Register<SmoothPassUnplaced>(
                "xggameplan.SmoothProcessing.SmoothPassUnplaced, xggameplan.core");
            modelTypeResolver.Register<SmoothPassBooked>(
                "xggameplan.SmoothProcessing.SmoothPassBooked, xggameplan.core");

            jsonSerializerFactory.Add(serializer => modelBinderSetter.Set(serializer));

            documentStore.Conventions.FindClrType = (documentName, obj, metadata) =>
            {
                var clrTypeName = metadata.Value<string>(Raven.Abstractions.Data.Constants.RavenClrType);
                return modelTypeResolver.Resolve(clrTypeName)?.FullName ?? clrTypeName;
            };
        }
    }
}
