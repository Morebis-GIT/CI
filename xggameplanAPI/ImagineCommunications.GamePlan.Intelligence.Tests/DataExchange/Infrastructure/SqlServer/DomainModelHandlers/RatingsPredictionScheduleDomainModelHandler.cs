using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.DomainModelHandlers
{
    public class RatingsPredictionScheduleDomainModelHandler : SimpleDomainModelMappingHandler<PredictionSchedule, RatingsPredictionSchedule>
    {
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        public RatingsPredictionScheduleDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        protected override PredictionSchedule MapToEntity(RatingsPredictionSchedule model) =>
            Mapper.Map<PredictionSchedule>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<PredictionSchedule> MapToEntity(IEnumerable<RatingsPredictionSchedule> models) =>
            Mapper.Map<IEnumerable<PredictionSchedule>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<RatingsPredictionSchedule> MapToModel(IEnumerable<PredictionSchedule> entities) =>
            Mapper.Map<IEnumerable<RatingsPredictionSchedule>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
    }
}
