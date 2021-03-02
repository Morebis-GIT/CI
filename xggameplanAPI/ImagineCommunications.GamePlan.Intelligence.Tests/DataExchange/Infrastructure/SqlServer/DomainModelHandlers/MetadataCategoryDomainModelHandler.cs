using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Metadatas;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.DomainModelHandlers
{
    public class MetadataCategoryDomainModelHandler : SimpleDomainModelMappingHandler<MetadataCategory, Metadata>
    {
        private readonly ITenantDbContext _dbContext;

        public MetadataCategoryDomainModelHandler(ISqlServerTestDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public override Metadata Add(Metadata model)
        {
            _dbContext.AddRange(Mapper
                .Map<IEnumerable<MetadataCategory>>((model ?? throw new ArgumentNullException(nameof(model)))
                    .ToMetadataModel()).ToArray());
            return model;
        }

        public override void AddRange(params Metadata[] models)
        {
            _dbContext.AddRange(models.SelectMany(x => Mapper.Map<IEnumerable<MetadataCategory>>(x.ToMetadataModel()))
                .ToArray());
        }

        public override IEnumerable<Metadata> GetAll()
        {
            var res = _dbContext.Query<MetadataCategory>().ToList();
            if (res.Any())
            {
                var metadata = new Metadata
                {
                    Dictionary = new Dictionary<MetaDataKeys, string>()
                };

                Mapper.Map<MetadataModel>(res).ApplyToMetadata(metadata);
                return new[] { metadata };
            }

            return Enumerable.Empty<Metadata>();
        }
    }
}
