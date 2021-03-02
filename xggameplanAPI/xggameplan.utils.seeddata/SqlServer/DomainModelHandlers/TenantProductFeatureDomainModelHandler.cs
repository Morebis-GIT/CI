using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.System.Features;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using TenantProductFeatureEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master.TenantProductFeatures.TenantProductFeature;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class TenantProductFeatureDomainModelHandler : IDomainModelHandler<TenantProductFeature>
    {
        private readonly ISqlServerDbContext _dbContext;
        private readonly IMapper _mapper;

        public TenantProductFeatureDomainModelHandler(
            ISqlServerDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public TenantProductFeature Add(TenantProductFeature model)
        {
            _ = _dbContext.Add(_mapper.Map<TenantProductFeatureEntity>(model), post => post.MapTo(model), _mapper);
            return model;
        }

        public void AddRange(params TenantProductFeature[] models) =>
            _dbContext.BulkInsertEngine.BulkInsert(_mapper.Map<List<TenantProductFeatureEntity>>(models),
                post => post.TryToUpdate(models), _mapper);

        public int Count() => _dbContext.Query<TenantProductFeatureEntity>().Count();

        public void DeleteAll() => _dbContext.Truncate<TenantProductFeatureEntity>();

        public IEnumerable<TenantProductFeature> GetAll() =>
            _mapper.Map<List<TenantProductFeature>>(_dbContext.Query<TenantProductFeatureEntity>().ToList());
    }
}
