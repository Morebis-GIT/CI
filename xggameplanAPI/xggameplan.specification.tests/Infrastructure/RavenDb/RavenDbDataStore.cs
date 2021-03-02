using System;
using System.IO;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.FileSystem;
using Raven.Client.UniqueConstraints;

namespace xggameplan.specification.tests.Infrastructure.RavenDb
{
    internal class RavenDbDataStore
    {
        private readonly EmbeddableDocumentStore _embeddableDocumentStore;
        private IDocumentStore _documentStore;
        private IFilesStore _filesStore;

        public RavenDbDataStore()
        {
            _embeddableDocumentStore = new EmbeddableDocumentStore
            {
                Configuration = {
                    CacheDocumentsInMemory = true,
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    RunInMemory = true,
                    CompiledIndexCacheDirectory = $@"~\CompiledIndexCache\{Guid.NewGuid().ToString()}",
                    TempPath = Path.Combine(Path.GetTempPath(), "RavenDb"),
                    Settings =
                    {
                        { "Raven/FileSystem/DisableRDC", "true" },
                        { "Raven/ActiveBundles", "Unique Constraints" }
                    },
                    Storage =
                    {
                        PreventSchemaUpdate = true,
                        SkipConsistencyCheck = true
                    }
                },
                Conventions = {
                    DisableProfiling = true,
                    MaxNumberOfRequestsPerSession = 1000
                }
            };
            _ = _embeddableDocumentStore.RegisterListener(new UniqueConstraintsStoreListener());
            _embeddableDocumentStore.Configuration.Storage.Voron.AllowOn32Bits = true;
            _embeddableDocumentStore.Configuration.Storage.Voron.TempPath = Path.Combine(Path.GetTempPath(), "RavenDb");
            _ = _embeddableDocumentStore.Initialize();

            _ = _embeddableDocumentStore.AggressivelyCacheFor(new TimeSpan(0, 5, 0));
            _embeddableDocumentStore.AttachIndexesBeforeUse();
            _embeddableDocumentStore.AttachTransformersBeforeUse();
        }

        public IDocumentStore DocumentStore
        {
            get
            {
                if (_documentStore == null)
                {
                    _documentStore = _embeddableDocumentStore;
                }

                return _documentStore;
            }
        }

        public IFilesStore FilesStore => _filesStore ?? (_filesStore = _embeddableDocumentStore.FilesStore);
    }
}
