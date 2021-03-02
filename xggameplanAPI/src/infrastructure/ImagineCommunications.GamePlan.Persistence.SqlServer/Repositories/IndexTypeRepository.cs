using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.IndexTypes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class IndexTypeRepository : IIndexTypeRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByNullableIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public IndexTypeRepository(
            ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByNullableIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public int CountAll => _dbContext.Query<Entities.Tenant.IndexType>().Count();

        public void Add(IEnumerable<IndexType> items) => _dbContext.BulkInsertEngine.BulkInsertOrUpdate(
            _mapper.Map<List<Entities.Tenant.IndexType>>(items, opts => opts.UseEntityCache(_salesAreaByNameCache)),
            new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true },
            post => post.TryToUpdate(items, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);

        public IndexType Find(int id) => _mapper.Map<IndexType>(_dbContext.Find<Entities.Tenant.IndexType>(id), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<IndexType> GetAll() =>
            _mapper.Map<List<IndexType>>(_dbContext.Query<Entities.Tenant.IndexType>(), opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void Update(IndexType indexType)
        {
            var entity = _dbContext.Find<Entities.Tenant.IndexType>(indexType.Id);
            if (entity != null)
            {
                _ = _mapper.Map(indexType, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(entity, post => post.MapTo(indexType, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
        }

        public void Remove(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.IndexType>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.IndexType>();

        public void SaveChanges() => _dbContext.SaveChanges();
    }
}
