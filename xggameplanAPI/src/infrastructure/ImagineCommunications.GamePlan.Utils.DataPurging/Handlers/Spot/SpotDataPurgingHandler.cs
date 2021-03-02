using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Utils.DataPurging.Attributes;
using ImagineCommunications.GamePlan.Utils.DataPurging.Options;
using Microsoft.Extensions.Options;
using xggameplan.core.Interfaces;
using Microsoft.Extensions.Logging;
using NodaTime;
using ImagineCommunications.GamePlan.Domain.SpotPlacements;
using Microsoft.EntityFrameworkCore;
using System;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Handlers.Spot
{
    [PurgingOptions("spots", Name = "spots")]
    public class SpotDataPurgingHandler : DataPurgingHandlerBase<PurgingOptions>
    {
        private readonly ISqlServerLongRunningTenantDbContext _dbContext;
        private readonly ISpotCleaner _spotCleaner;
        private readonly ILogger<SpotDataPurgingHandler> _logger;
        private readonly ISpotPlacementRepository _spotPlacement;
        private readonly IClock _clock;

        public SpotDataPurgingHandler(
            ISqlServerLongRunningTenantDbContext dbContext,
            ISpotCleaner spotCleaner,
            ILogger<SpotDataPurgingHandler> logger,
            ISpotPlacementRepository spotPlacement,
            IClock clock,
            IOptionsSnapshot<PurgingOptions> options) : base(options)
        {
            _dbContext = dbContext;
            _spotCleaner = spotCleaner;
            _logger = logger;
            _spotPlacement = spotPlacement;
            _clock = clock;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (Options.DaysAfter == 0)
            {
                _logger.LogInformation($"Configuration parameters hasn't been specified, execution skipped.");

                return;
            }

            var thresholdDate = _clock.GetCurrentInstant().ToDateTimeUtc().AddDays(-Options.DaysAfter);

            var externalRefs = await (
                from spot in _dbContext.Query<Persistence.SqlServer.Entities.Tenant.Spot>()
                join campaignJoin in _dbContext.Query<Campaign>() on spot.ExternalCampaignNumber equals campaignJoin
                    .ExternalId into cJoin
                from camp in cJoin.DefaultIfEmpty()
                where spot.StartDateTime < thresholdDate && (camp == null || camp.EndDateTime < thresholdDate)
                select spot.ExternalSpotRef
               ).ToArrayAsync(cancellationToken).ConfigureAwait(false);

            LogInitialState(thresholdDate, externalRefs.LongLength);

            await _spotCleaner.ExecuteAsync(
                externalRefs,
                (string message) => _logger.LogDebug(message),
                thresholdDate,
                cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Finish removing spots");

            _logger.LogDebug("Start removing spot placements");
            _spotPlacement.DeleteBefore(thresholdDate);
            _logger.LogDebug("Finish removing spot placements");
        }

        private void LogInitialState(DateTime thresholdDate, long externalRefsLength)
        {
            _logger.LogDebug($"'{nameof(Options.DaysAfter)}' parameter is set to {Options.DaysAfter}.");
            _logger.LogDebug($"Threshold DateTime is {thresholdDate}");
            _logger.LogDebug($"{externalRefsLength} initial spots returned.");
        }
    }
}
