using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Services;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenTenantsRepository : ITenantsRepository, IPreviewFileStorage
    {
        private readonly IRavenMasterDbContext _dbContext;
        private readonly RavenPreviewFileStorage _previewFileStorage;

        public RavenTenantsRepository(IRavenMasterDbContext dbContext, RavenPreviewFileStorage previewFileStorage)
        {
            _dbContext = dbContext;
            _previewFileStorage = previewFileStorage;
        }

        public List<Tenant> GetAll() => _dbContext.Query<Tenant>().ToList();

        public Tenant GetById(int id) => _dbContext.Find<Tenant>(id);

        public void Add(Tenant tenant) =>
            _dbContext.Add(tenant);

        public void Update(Tenant tenant) => _dbContext.Update(tenant);

        public void SaveChanges() => _dbContext.SaveChanges();

        public void SetPreviewFile(int tenantId, PreviewFile previewFile, Stream previewFileStream)
        {
            var entity = _dbContext.Find<Tenant>(tenantId);

            if (previewFileStream == null)
            {
                return;
            }

            var existingPreviewId = entity.Preview?.Id;
            if (existingPreviewId != null)
            {
                _previewFileStorage.DeletePreviewFile(existingPreviewId);
            }
            entity.Preview = previewFile;
            _previewFileStorage.UploadPreviewFile(previewFile, previewFileStream);
        }

        public PreviewFile GetPreviewFile(int entityId, out Stream previewFileStream)
        {
            var previewFile = _dbContext.Find<Tenant>(entityId)?.Preview;
            previewFileStream = null;
            if (previewFile?.Id == null)
            {
                return null;
            }

            previewFileStream = _previewFileStorage.GetPreviewFileStream(previewFile.Id);
            return previewFile;
        }

        public void DeletePreviewFile(int entityId)
        {
            var entity = _dbContext.Find<Tenant>(entityId);
            var previewFileId = entity?.Preview?.Id;
            
            if (previewFileId != null)
            {
                entity.Preview = null;
                _dbContext.Update(entity);
                _previewFileStorage.DeletePreviewFile(previewFileId);
            }
        }

        public void Flush() => SaveChanges();
    }
}
