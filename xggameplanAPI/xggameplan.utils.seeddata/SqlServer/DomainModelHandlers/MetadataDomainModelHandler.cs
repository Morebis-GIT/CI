using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using MetadataCategoryEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Metadatas.MetadataCategory;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class MetadataDomainModelHandler : IDomainModelHandler<Metadata>
    {
        private readonly IMetadataRepository _metadataRepository;
        private readonly ISqlServerTenantDbContext _dbContext;

        public MetadataDomainModelHandler(IMetadataRepository metadataRepository, ISqlServerTenantDbContext dbContext)
        {
            _metadataRepository = metadataRepository ?? throw new ArgumentNullException(nameof(metadataRepository));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Metadata Add(Metadata model)
        {
            _metadataRepository.Add(model.ToMetadataModel());
            return model;
        }

        public void AddRange(params Metadata[] models)
        {
            foreach (var model in models)
            {
                _ = Add(model);
            }
        }

        public int Count() => _dbContext.Query<MetadataCategoryEntity>().Any() ? 1 : 0;

        public void DeleteAll() => _dbContext.Truncate<MetadataCategoryEntity>();

        public IEnumerable<Metadata> GetAll()
        {
            var metadataModel = _metadataRepository.GetAll();
            if (metadataModel != null)
            {
                var metadata = new Metadata
                {
                    Dictionary = new Dictionary<MetaDataKeys, string>()
                };

                metadataModel.ApplyToMetadata(metadata);
                return new[] { metadata };
            }

            return Enumerable.Empty<Metadata>();
        }
    }
}
