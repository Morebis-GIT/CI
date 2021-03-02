using System;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using SmoothConfigurationEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothConfiguration;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class SmoothConfigurationRepository
        : ISmoothConfigurationRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;

        public SmoothConfigurationRepository(ISqlServerTenantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public SmoothConfiguration GetById(int id)
        {
            var entity = PrepareQuery().FirstOrDefault(x => x.Id == id);

            return entity != null ? _mapper.Map<SmoothConfiguration>(entity) : null;
        }

        public void Add(SmoothConfiguration smoothConfiguration) =>
            _dbContext.Add(_mapper.Map<SmoothConfigurationEntity>(smoothConfiguration),
                post => post.MapTo(smoothConfiguration), _mapper);

        public void Update(SmoothConfiguration smoothConfiguration)
        {
            var entity = _dbContext.Query<SmoothConfigurationEntity>()
                .FirstOrDefault(x => x.Id == smoothConfiguration.Id);

            if (entity != null)
            {
                _mapper.Map(smoothConfiguration, entity);
                _dbContext.Update(entity, post => post.MapTo(smoothConfiguration), _mapper);
            }
        }

        public void SaveChanges() =>
            _dbContext.SaveChanges();

        protected IQueryable<SmoothConfigurationEntity> PrepareQuery()
        {
            return _dbContext.Query<SmoothConfigurationEntity>()
                .Include(x => x.DiagnosticConfiguration)
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
