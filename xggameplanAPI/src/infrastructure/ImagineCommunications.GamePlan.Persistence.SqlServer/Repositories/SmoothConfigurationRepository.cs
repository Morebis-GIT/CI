using System;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using SmoothConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothConfiguration;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SmoothConfigurationRepository
        : ISmoothConfigurationRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        public SmoothConfigurationRepository(ISqlServerTenantDbContext dbContext, ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
          ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache, IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        public SmoothConfiguration GetById(int id)
        {
         
            var entity = PrepareQuery().FirstOrDefault(x => x.Id == id);

            return entity != null
                ? _mapper.Map<SmoothConfiguration>(entity, opts => opts.UseEntityCache(_salesAreaByIdCache))
                : null;
        }

        public void Add(SmoothConfiguration smoothConfiguration) =>
            _dbContext.Add(_mapper.Map<SmoothConfigurationEntity>(smoothConfiguration, opts => opts.UseEntityCache(_salesAreaByNameCache)),
                post => post.MapTo(smoothConfiguration, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);

        public void Update(SmoothConfiguration smoothConfiguration)
        {
            var entity = _dbContext.Query<SmoothConfigurationEntity>()
                .FirstOrDefault(x => x.Id == smoothConfiguration.Id);

            if (entity != null)
            {
                _ = _mapper.Map(smoothConfiguration, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                _ = _dbContext.Update(entity, post => post.MapTo(smoothConfiguration, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
            }
        }

        public void SaveChanges() =>
            _dbContext.SaveChanges();

        protected IQueryable<SmoothConfigurationEntity> PrepareQuery()
        {
            return _dbContext.Query<SmoothConfigurationEntity>()
                .Include(x => x.DiagnosticConfiguration)
                .ThenInclude(x=> x.SpotSalesAreas)
                .Include(x => x.Passes)
                .Include(x => x.IterationRecords)
                    .ThenInclude(x => x.PassSequences)
                .Include(x => x.IterationRecords)
                    .ThenInclude(x => x.PassDefaultIteration)
                .Include(x => x.IterationRecords)
                    .ThenInclude(x => x.PassUnplacedIteration)
                .Include(x => x.BestBreakFactorGroupRecords)
                    .ThenInclude(x => x.PassSequences)
                .Include(x => x.BestBreakFactorGroupRecords)
                    .ThenInclude(x => x.BestBreakFactorGroup)
                        .ThenInclude(x => x.Items)
                .Include(x => x.BestBreakFactorGroupRecords)
                    .ThenInclude(x => x.BestBreakFactorGroup)
                        .ThenInclude(x => x.SameBreakGroupScoreFactor)
                .Include(x => x.BestBreakFactorGroupRecords)
                    .ThenInclude(x => x.BestBreakFactorGroup)
                        .ThenInclude(x => x.Items).ThenInclude(x => x.DefaultFactors)
                .Include(x => x.BestBreakFactorGroupRecords)
                    .ThenInclude(x => x.BestBreakFactorGroup)
                        .ThenInclude(x => x.Items).ThenInclude(x => x.FilterFactors);
        }

        public void Truncate() => throw new NotImplementedException();
    }
}
