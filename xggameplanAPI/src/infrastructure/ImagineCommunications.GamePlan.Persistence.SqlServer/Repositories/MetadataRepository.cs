using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Metadatas;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class MetadataRepository : IMetadataRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public MetadataRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public MetadataModel GetAll() => _mapper.Map<MetadataModel>(_dbContext
            .Query<MetadataCategory>().Include(e => e.MetadataValues).ToList());

        public void Add(MetadataModel metadata)
        {
            _dbContext.RemoveRange(_dbContext.Query<MetadataCategory>().ToArray());
            if (metadata.Any())
            {
                _dbContext.AddRange(_mapper.Map<IEnumerable<MetadataCategory>>(metadata).ToArray());
            }
        }

        public List<Data> GetByKey(MetaDataKeys key)
        {
            var values = _dbContext
                .Query<MetadataCategory>().Include(e => e.MetadataValues)
                .FirstOrDefault(e => e.Name == key.ToString())
                ?.MetadataValues;

            return values?.Any() ?? false
                ? _mapper.Map<List<Data>>(values)
                : new List<Data>();
        }

        public MetadataModel GetByKeys(List<MetaDataKeys> keys)
        {
            var categories = _dbContext
                .Query<MetadataCategory>()
                .Include(e => e.MetadataValues)
                .Where(e => keys.Contains((MetaDataKeys)Enum.Parse(typeof(MetaDataKeys), e.Name)))
                .ToList();

            return categories.Any()
                ? _mapper.Map<MetadataModel>(categories)
                : null;
        }

        public void DeleteByKey(MetaDataKeys key)
        {
            var metadataCategory = _dbContext.Query<MetadataCategory>()
                .FirstOrDefault(x => x.Name == key.ToString());

            if (metadataCategory != null)
            {
                _dbContext.Remove(metadataCategory);
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
