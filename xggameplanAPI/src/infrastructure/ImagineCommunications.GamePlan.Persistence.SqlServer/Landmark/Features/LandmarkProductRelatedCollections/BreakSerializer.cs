using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Cache;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NodaTime;
using xggameplan.AuditEvents;
using xggameplan.core.Extensions.AutoMapper;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Services.OptimiserInputFilesSerialisers.Breaks;
using SalesAreaEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas.SalesArea;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Landmark.Features.LandmarkProductRelatedCollections
{
    public class BreakSerializer : BreakSerializerBase
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IEntityCache<Guid, SalesAreaEntity> _salesAreaCache;

        public BreakSerializer(IAuditEventRepository auditEventRepository, IFeatureManager featureManager,
            ISqlServerTenantDbContext dbContext,
            IRepositoryFactory repositoryFactory, IConfiguration applicationConfiguration, IMapper mapper, IClock clock)
            : base(auditEventRepository, featureManager, repositoryFactory, applicationConfiguration, mapper, clock)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _salesAreaCache =
                new SqlServerEntityCache<Guid, SalesAreaEntity>(_dbContext, x => x.Id,
                    trackingChanges: false);
        }

        protected override IReadOnlyCollection<BreakWithProgramme> GetBreaksWithProgrammes(Run run,
            IReadOnlyCollection<SalesArea> salesAreas)
        {
            var salesAreaIds = salesAreas.Select(s => s.Id);
            var fromDate = run.StartDate.Add(run.StartTime);
            var toDate = DateHelper.ConvertBroadcastToStandard(run.EndDate, run.EndTime);

            var programmes = _dbContext.Query<ScheduleProgramme>()
                .Include(x => x.Programme).ThenInclude(x => x.ProgrammeCategoryLinks).ThenInclude(x => x.ProgrammeCategory)
                .Include(x => x.Programme).ThenInclude(x => x.Episode)
                .Include(x => x.Programme).ThenInclude(x => x.ProgrammeDictionary)
                .Select(x => x.Programme)
                .Where(p => salesAreaIds.Contains(p.SalesAreaId) && p.StartDateTime >= fromDate.Date &&
                            p.StartDateTime <= toDate.AddDays(1).Date)
                .Select(p => new ProgrammeLink
                {
                    ExternalRef = p.ProgrammeDictionary.ExternalReference,
                    SalesAreaId = p.SalesAreaId,
                    EpisodeNumber = p.Episode.Number,
                    PrgtNo = p.PrgtNo,
                    StartDateTime = p.StartDateTime,
                    ProgrammeCategories = p.ProgrammeCategoryLinks.Select(x => x.ProgrammeCategory.Name).ToList()
                }).AsNoTracking()
                .AsEnumerable()
                .GroupBy(x => x.ExternalRef)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(p => p.StartDateTime).ToArray());

            var defaultCategories = new List<string> { "SPORTS" };

            _salesAreaCache.Load();
            return _dbContext.Query<ScheduleBreak>()
                .Where(x => x.BreakType != ExcludeBreakType && x.ScheduledDate >= fromDate &&
                            x.ScheduledDate <= toDate && salesAreaIds.Contains(x.SalesAreaId))
                .AsEnumerable()
                .Select(b =>
                {
                    ProgrammeLink pgm = null;
                    if (programmes.TryGetValue(b.ExternalProgRef, out var grp))
                    {
                        pgm = grp.FirstOrDefault(x => x.SalesAreaId == b.SalesAreaId && x.StartDateTime <= b.ScheduledDate);
                    }

                    return new BreakWithProgramme
                    {
                        Break = _mapper.Map<Break>(b, opts => opts.UseEntityCache(_salesAreaCache)),
                        //For now default to SPORTS programme category if null
                        ProgrammeCategories =
                            (pgm?.ProgrammeCategories.Count ?? 0) == 0 ? defaultCategories : pgm.ProgrammeCategories,
                        ProgrammeExternalreference = pgm?.ExternalRef,
                        EpisodeNumber = pgm?.EpisodeNumber,
                        PrgtNo = pgm?.PrgtNo ?? 0
                    };
                }).ToArray();
        }

        protected class ProgrammeLink
        {
            public string ExternalRef { get; set; }
            public Guid SalesAreaId { get; set; }
            public int PrgtNo { get; set; }
            public DateTime StartDateTime { get; set; }
            public List<string> ProgrammeCategories { get; set; }
            public int? EpisodeNumber { get; set; }
        }
    }
}
