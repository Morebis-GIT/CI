using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns;
using ImagineCommunications.GamePlan.Utils.DataPurging.Attributes;
using ImagineCommunications.GamePlan.Utils.DataPurging.Exceptions;
using ImagineCommunications.GamePlan.Utils.DataPurging.Infrastructure;
using ImagineCommunications.GamePlan.Utils.DataPurging.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NodaTime;
using xggameplan.common.Extensions;
using xggameplan.core.Interfaces;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Handlers.Campaigns
{
    [PurgingOptions("campaigns")]
    public class CampaignDataPurgingHandler : DataPurgingHandlerBase<CampaignPurgingOptions>
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ICampaignCleaner _campaignCleaner;
        private readonly IClock _clock;
        private readonly ILogger<CampaignDataPurgingHandler> _logger;

        public CampaignDataPurgingHandler(
            IOptionsSnapshot<CampaignPurgingOptions> options,
            ISqlServerTenantDbContext dbContext,
            ICampaignCleaner campaignCleaner,
            IClock clock,
            ILogger<CampaignDataPurgingHandler> logger) : base(options)
        {
            _dbContext = dbContext;
            _campaignCleaner = campaignCleaner;
            _clock = clock;
            _logger = logger;
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (Options.DaysAfter == 0)
            {
                _logger.LogDebug(
                    $"'{nameof(Options.DaysAfter)}' configuration parameters hasn't been specified, execution skipped.");
                return;
            }

            var thresholdDate = _clock.GetCurrentInstant().ToDateTimeUtc().AddDays(-Options.DaysAfter);

            var campaignIds = await _dbContext.Query<Campaign>().AsNoTracking()
                .Where(x => x.EndDateTime < thresholdDate)
                .Select(x => x.Id).ToArrayAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogDebug(
                $"'{nameof(Options.DaysAfter)}' parameter is set to {Options.DaysAfter} days. {campaignIds.Length} campaign(s) returned which end date less than {thresholdDate}.");

            if (campaignIds.Length > 0)
            {
                var targetBlock = new BatchExecuteTargetBlock<Guid>(
                    ids => _campaignCleaner.ExecuteAsync(ids, cancellationToken), Options.Concurrency,
                    cancellationToken);

                foreach (var campaignId in campaignIds)
                {
                    _ = await targetBlock.SendAsync(campaignId, cancellationToken).ConfigureAwait(false);
                }

                targetBlock.Complete();

                await targetBlock.Completion.AggregateExceptions<DataPurgingException>(
                    "Purging of Campaign data has been failed. See inner exception for more details.");

                _logger.LogDebug(
                    $"Purging of {campaignIds.Length} campaign(s) has been completed successfully according to the '{nameof(Options.DaysAfter)}' parameter.");
            }
        }
    }
}
