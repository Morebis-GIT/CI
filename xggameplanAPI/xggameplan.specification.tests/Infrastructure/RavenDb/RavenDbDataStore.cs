using System;
using System.IO;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.FileSystem;
using Raven.Client.UniqueConstraints;

namespace xggameplan.specification.tests.Infrastructure.RavenDb
{
    internal class RavenDbDataStore
        : IDisposable
    {
        private static readonly string _tempRavenDbPath = Path.Combine(Path.GetTempPath(), "RavenDb");

        private readonly EmbeddableDocumentStore _embeddableDocumentStore;
        private readonly string _ravenDbCachePath;

        private IDocumentStore _documentStore;
        private IFilesStore _filesStore;

        public RavenDbDataStore()
        {
            _ravenDbCachePath = Path.Combine(
                _tempRavenDbPath,
                Path.Combine(
                    "CompiledIndexCache",
                    Guid.NewGuid().ToString())
                );

            _embeddableDocumentStore = new EmbeddableDocumentStore
            {
                Configuration = {
                    CacheDocumentsInMemory = true,
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    RunInMemory = true,
                    CompiledIndexCacheDirectory = _ravenDbCachePath,
                    TempPath = _tempRavenDbPath,
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
            _embeddableDocumentStore.Configuration.Storage.Voron.TempPath = _tempRavenDbPath;

            _ = _embeddableDocumentStore.Initialize();

            _embeddableDocumentStore.AttachIndexesBeforeUse();
            _embeddableDocumentStore.AttachTransformersBeforeUse();
        }

        public IDocumentStore DocumentStore
        {
            get
            {
                if (_documentStore is null)
                {
                    _documentStore = _embeddableDocumentStore;
                }

                return _documentStore;
            }
        }

        public IFilesStore FilesStore => _filesStore ?? (_filesStore = _embeddableDocumentStore.FilesStore);

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            if (Directory.Exists(_ravenDbCachePath))
            {
                try
                {
                    Directory.Delete(_ravenDbCachePath, true);
                }
                catch
                {
                }
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
