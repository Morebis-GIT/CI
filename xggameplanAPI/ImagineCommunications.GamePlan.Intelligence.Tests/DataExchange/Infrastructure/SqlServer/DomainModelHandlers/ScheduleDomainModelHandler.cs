using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using xggameplan.core.Extensions.AutoMapper;
using ScheduleBreakEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules.ScheduleBreak;
using ScheduleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules.Schedule;
using ScheduleProgrammeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules.ScheduleProgramme;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.SqlServer.DomainModelHandlers
{
    public class ScheduleDomainModelHandler : SimpleDomainModelMappingHandler<ScheduleEntity, Schedule>
    {
        private readonly ISqlServerTestDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;

        protected override ScheduleEntity MapToEntity(Schedule model) =>
            Mapper.Map<ScheduleEntity>(model, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<ScheduleEntity> MapToEntity(IEnumerable<Schedule> models) =>
            Mapper.Map<IEnumerable<ScheduleEntity>>(models, opts => opts.UseEntityCache(_salesAreaByNameCache));

        protected override IEnumerable<Schedule> MapToModel(IEnumerable<ScheduleEntity> entities) =>
            Mapper.Map<IEnumerable<Schedule>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));

        public ScheduleDomainModelHandler(
            ISqlServerTestDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
            : base(dbContext, mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
        }

        public override Schedule Add(Schedule model)
        {
            var entity = Mapper.Map<ScheduleEntity>(model ?? throw new ArgumentNullException(nameof(model)),
                opts => opts.UseEntityCache(_salesAreaByNameCache));
            entity.Breaks =
                Mapper.Map<List<ScheduleBreakEntity>>(model.Breaks, opts => opts.UseEntityCache(_salesAreaByNameCache));
            entity.Programmes = Mapper.Map<List<ScheduleProgrammeEntity>>(model.Programmes,
                opts => opts.UseEntityCache(_salesAreaByNameCache));

            _ = _dbContext.Add(entity, post => post.MapTo(model, opts => opts.UseEntityCache(_salesAreaByIdCache)),
                Mapper);

            return model;
        }

        public override void AddRange(params Schedule[] models)
        {
            foreach (var model in models ?? throw new ArgumentNullException(nameof(models)))
            {
                _ = Add(model);
            }
        }
    }
}
