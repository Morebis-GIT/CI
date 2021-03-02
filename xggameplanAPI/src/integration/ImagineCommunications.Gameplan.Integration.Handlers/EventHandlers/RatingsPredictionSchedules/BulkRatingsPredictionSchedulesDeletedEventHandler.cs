using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.RatingsPredictionSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.common.Extensions;
using PredictionScheduleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules.PredictionSchedule;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.RatingsPredictionSchedules
{
    public class BulkRatingsPredictionSchedulesDeletedEventHandler : IEventHandler<IBulkRatingsPredictionSchedulesDeleted>,
        IBatchingEnumerateHandler<IBulkRatingsPredictionSchedulesDeleted, IRatingsPredictionSchedulesDeleted>
    {

        private const int DeletionBatch = 5_000;
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;

        public BulkRatingsPredictionSchedulesDeletedEventHandler(ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory) =>
            _dbContextFactory = dbContextFactory;

        public void Handle(IBulkRatingsPredictionSchedulesDeleted command)
        {
            foreach (var grpoup in command.Data.GroupBy(x => x.SalesArea, StringComparer.InvariantCultureIgnoreCase))
            {
                using (var enumerator = grpoup.OrderBy(x => x.DateTimeFrom).GetEnumerator())
                {
                    Handle(enumerator);
                }
            }
        }

        public void Handle(IEnumerator<IRatingsPredictionSchedulesDeleted> eventEnumerator)
        {
            // Remove Task.Run when Handle method becomes async
            Task.Run(async () =>
            {
                var actionBlock = new ActionBlock<RatingsPredictionScheduleDeleteBatch>(DeleteRatingsPredictionSchedulesAsync,
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });
                RatingsPredictionScheduleDeleteBatch deleteInfo = null;

                while (eventEnumerator.MoveNext())
                {
                    var criteria = eventEnumerator.Current;
                    if (criteria is null)
                    {
                        continue;
                    }

                    if (deleteInfo is null)
                    {
                        deleteInfo = RatingsPredictionScheduleDeleteBatch.Create(criteria);
                    }
                    else
                    {
                        if (deleteInfo.SalesArea.Equals(criteria.SalesArea,
                                StringComparison.InvariantCultureIgnoreCase) &&
                            deleteInfo.ToDate.AddSeconds(1) >= criteria.DateTimeFrom &&
                            criteria.DateTimeTo.AddSeconds(1) >= deleteInfo.FromDate)
                        {
                            deleteInfo.FromDate = deleteInfo.FromDate < criteria.DateTimeFrom
                                ? deleteInfo.FromDate
                                : criteria.DateTimeFrom;

                            deleteInfo.ToDate = deleteInfo.ToDate < criteria.DateTimeTo
                                ? criteria.DateTimeTo
                                : deleteInfo.ToDate;
                        }
                        else
                        {
                            _ = await actionBlock.SendAsync(deleteInfo).ConfigureAwait(false);
                            deleteInfo = RatingsPredictionScheduleDeleteBatch.Create(criteria);
                        }
                    }
                }

                if (!(deleteInfo is null))
                {
                    _ = await actionBlock.SendAsync(deleteInfo).ConfigureAwait(false);
                }

                actionBlock.Complete();
                await actionBlock.Completion.AggregateExceptions().ConfigureAwait(false);

                async Task DeleteRatingsPredictionSchedulesAsync(RatingsPredictionScheduleDeleteBatch ratingsPredictionScheduleDeleteBatch)
                {
                    int count;

                    do
                    {
                        using (var dbContext = _dbContextFactory.Create())
                        {
                            var ratingsPredictionSchedules = await dbContext.Query<PredictionScheduleEntity>().Where(p =>
                                    p.ScheduleDay >= ratingsPredictionScheduleDeleteBatch.FromDate &&
                                    p.ScheduleDay <= ratingsPredictionScheduleDeleteBatch.ToDate &&
                                    p.SalesArea == ratingsPredictionScheduleDeleteBatch.SalesArea)
                                .Select(x => new PredictionScheduleEntity { Id = x.Id })
                                .Take(DeletionBatch)
                                .AsNoTracking()
                                .ToArrayAsync().ConfigureAwait(false);

                            count = ratingsPredictionSchedules.Length;

                            foreach (var item in ratingsPredictionSchedules)
                            {
                                dbContext.Specific.Attach(item).State = EntityState.Deleted;
                            }

                            await dbContext.SaveChangesWithConcurrencyConflictsResolvingAsync()
                                .ConfigureAwait(false);
                        }
                    } while (count == DeletionBatch);
                }
            }).GetAwaiter().GetResult();
        }

        protected class RatingsPredictionScheduleDeleteBatch
        {
            public static RatingsPredictionScheduleDeleteBatch Create(IRatingsPredictionSchedulesDeleted deletedEvent)
            {
                return new RatingsPredictionScheduleDeleteBatch
                {
                    SalesArea = deletedEvent.SalesArea,
                    FromDate = deletedEvent.DateTimeFrom,
                    ToDate = deletedEvent.DateTimeTo
                };
            }

            public string SalesArea { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }
    }
}
