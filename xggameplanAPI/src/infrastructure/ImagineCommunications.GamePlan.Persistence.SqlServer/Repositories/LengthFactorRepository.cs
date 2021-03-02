using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.LengthFactors;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class LengthFactorRepository : ILengthFactorRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public LengthFactorRepository(
            ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public void AddRange(IEnumerable<LengthFactor> items) =>
            _dbContext.AddRange(_mapper.Map<Entities.Tenant.LengthFactor[]>(items, opts => opts.UseEntityCache(_salesAreaByNameCache)),
                post => post.MapToCollection(items, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);

        public void Delete(int id)
        {
            var entity = _dbContext.Find<Entities.Tenant.LengthFactor>(id);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public LengthFactor Get(int id) =>
            _mapper.Map<LengthFactor>(_dbContext.Find<Entities.Tenant.LengthFactor>(id),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<LengthFactor> GetAll() =>
            _mapper.Map<List<LengthFactor>>(_dbContext.Query<Entities.Tenant.LengthFactor>().AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public void Truncate() => _dbContext.Truncate<Entities.Tenant.LengthFactor>();

        public void Update(LengthFactor item)
        {
            var entity = _dbContext.Find<Entities.Tenant.LengthFactor>(item.Id);
            if (entity != null)
            {
                _ = _mapper.Map(item, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(entity);
            }
        }
    }
}
