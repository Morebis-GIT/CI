using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.common.Extensions;
using BreakEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks.Break;
using SpotEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Spot;
using ScheduleBreakEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules.ScheduleBreak;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Breaks
{
    public class BulkBreaksDeletedEventHandler : BusClient.Abstraction.Classes.EventHandler<IBulkBreaksDeleted>, IBatchingEnumerateHandler<IBulkBreaksDeleted, IBreakDeleted>
    {
        private const int DeletionBatch = 5000;
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;

        public BulkBreaksDeletedEventHandler(ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory) => _dbContextFactory = dbContextFactory;

        public override void Handle(IBulkBreaksDeleted command) => Handle(command.Data.OrderBy(c => c.DateRangeStart).GetEnumerator());

        public void Handle(IEnumerator<IBreakDeleted> commandEnumerator)
        {
            // Remove Task.Run when Handle method becomes async
            Task.Run(async () =>
            {
                var actionBlock = new ActionBlock<IBreakDeleted>(DeleteBreaksAsync,
                    new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 1});

                while (commandEnumerator.MoveNext())
                {
                    _ = await actionBlock.SendAsync(commandEnumerator.Current).ConfigureAwait(false);
                }

                actionBlock.Complete();
                await actionBlock.Completion.AggregateExceptions().ConfigureAwait(false);
            }).GetAwaiter().GetResult();
        }

        private async Task DeleteBreaksAsync(IBreakDeleted deletionInfo)
        {
            // zero out time
            var broadcastDate = deletionInfo.DateRangeStart.Date;

            int count;

            do
            {
                using (var dbContext = _dbContextFactory.Create())
                {
                    var breaks = await dbContext.Query<BreakEntity>()
                        .AsNoTracking()
                        .Where(b => b.BroadcastDate == broadcastDate && deletionInfo.SalesAreaNames.Contains(b.SalesArea.Name))
                        .Select(x => new BreakEntity {Id = x.Id, ExternalBreakRef = x.ExternalBreakRef})
                        .Take(DeletionBatch)
                        .ToDictionaryAsync(x => x.Id, x => x.ExternalBreakRef).ConfigureAwait(false);

                    count = breaks.Count;

                    if (count == 0)
                    {
                        break;
                    }

                    var spotIds = await dbContext
                        .Query<SpotEntity>()
                        .AsNoTracking()
                        .Where(s => breaks.Values.Contains(s.ExternalBreakNo))
                        .Select(x => x.Id)
                        .ToArrayAsync().ConfigureAwait(false);

                    using (var transaction = await dbContext.Specific.Database.BeginTransactionAsync().ConfigureAwait(false))
                    {
                        var breakIds = breaks.Keys.ToArray();
                        _ = dbContext.Specific.RemoveByUniqueIdentifierIds<BreakEntity>(breakIds);
                        _ = dbContext.Specific.RemoveByUniqueIdentifierIds<ScheduleBreakEntity>(breakIds);

                        for (int j = 0; j <= spotIds.Length / DeletionBatch; j++)
                        {
                            var spotIdsBatch = spotIds.Skip(j * DeletionBatch).Take(DeletionBatch).ToArray();
                            _ = dbContext.Specific.RemoveByIdentityIds<SpotEntity>(spotIdsBatch);
                        }

                        transaction.Commit();
                    }
                }
            } while (count == DeletionBatch);
        }
    }
}
