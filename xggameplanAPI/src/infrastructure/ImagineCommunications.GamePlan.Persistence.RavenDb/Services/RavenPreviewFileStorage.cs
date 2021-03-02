using System;
using System.IO;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using Raven.Client.FileSystem;
using Raven.Json.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Services
{
    public class RavenPreviewFileStorage
    {

        private readonly IFilesStore _filesStore;

        public RavenPreviewFileStorage(IFilesStore filesStore)
        {
            _filesStore = filesStore;
        }

        public void UploadPreviewFile(PreviewFile previewFile, Stream previewFileStream)
        {
            if (previewFileStream == null)
            {
                return;
            }

            var metadata = new RavenJObject() {
                { "XG-File-Id",  RavenJToken.FromObject(previewFile.Id) },
                { "XG-File-Name", RavenJToken.FromObject(previewFile.FileName) },
                { "XG-File-Extension",RavenJToken.FromObject(previewFile.FileExtension) },
                { "XG-Content-Type", RavenJToken.FromObject(previewFile.ContentType) },
                { "XG-Content-Length", RavenJToken.FromObject(previewFile.ContentLength) }};

            using (var session = _filesStore.OpenAsyncSession())
            {
                session.Commands.UploadAsync(previewFile.Id, previewFileStream, metadata).Wait();
            }
        }

        public Stream GetPreviewFileStream(string entityId)
        {
            try
            {
                using (var session = _filesStore.OpenAsyncSession())
                {
                    return session.DownloadAsync(entityId).Result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void DeletePreviewFile(string entityId)
        {
            using (var session = _filesStore.OpenAsyncSession())
            {
                session.Commands.DeleteAsync(entityId).Wait();
            }
        }

    }
}
