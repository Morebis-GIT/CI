using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.common.Extensions;
using ProgrammeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes.Programme;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Programme
{
    public class BulkProgrammeDeletedEventHandler : IEventHandler<IBulkProgrammeDeleted>, IBatchingEnumerateHandler<IBulkProgrammeDeleted, IProgrammesDeleted>
    {
        private const int DeletionBatch = 5000;
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;

        public BulkProgrammeDeletedEventHandler(ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Handle(IBulkProgrammeDeleted command)
        {
            foreach (var grp in command.Data.GroupBy(x => x.SalesArea, StringComparer.InvariantCultureIgnoreCase))
            {
                using (var enumerator = grp.OrderBy(x => x.FromDate).GetEnumerator())
                {
                    Handle(enumerator);
                }
            }
        }

        public void Handle(IEnumerator<IProgrammesDeleted> eventEnumerator)
        {
            // Remove Task.Run when Handle method becomes async
            Task.Run(async () =>
            {
                var actionBlock = new ActionBlock<ProgrammDeleteBatch>(DeleteProgrammesAsync,
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });
                ProgrammDeleteBatch deleteInfo = null;

                while (eventEnumerator.MoveNext())
                {
                    var criteria = eventEnumerator.Current;
                    if (criteria is null)
                    {
                        continue;
                    }

                    if (deleteInfo is null)
                    {
                        deleteInfo = ProgrammDeleteBatch.Create(criteria);
                    }
                    else
                    {
                        if (deleteInfo.SalesArea.Equals(criteria.SalesArea,
                                StringComparison.InvariantCultureIgnoreCase) &&
                            deleteInfo.ToDate.AddSeconds(1) >= criteria.FromDate &&
                            criteria.ToDate.AddSeconds(1) >= deleteInfo.FromDate)
                        {
                            deleteInfo.FromDate = deleteInfo.FromDate < criteria.FromDate
                                ? deleteInfo.FromDate
                                : criteria.FromDate;

                            deleteInfo.ToDate = deleteInfo.ToDate < criteria.ToDate
                                ? criteria.ToDate
                                : deleteInfo.ToDate;
                        }
                        else
                        {
                            _ = await actionBlock.SendAsync(deleteInfo).ConfigureAwait(false);
                            deleteInfo = ProgrammDeleteBatch.Create(criteria);
                        }
                    }
                }

                if (!(deleteInfo is null))
                {
                    _ = await actionBlock.SendAsync(deleteInfo).ConfigureAwait(false);
                }

                actionBlock.Complete();
                await actionBlock.Completion.AggregateExceptions().ConfigureAwait(false);

                async Task DeleteProgrammesAsync(ProgrammDeleteBatch programmDeleteBatch)
                {
                    int count;

                    do
                    {
                        using (var dbContext = _dbContextFactory.Create())
                        {
                            var programmes = await dbContext.Query<ProgrammeEntity>().Where(p =>
                                    p.StartDateTime >= programmDeleteBatch.FromDate &&
                                    p.StartDateTime <= programmDeleteBatch.ToDate &&
                                    p.SalesArea.Name == programmDeleteBatch.SalesArea)
                                .Select(x => new ProgrammeEntity { Id = x.Id })
                                .Take(DeletionBatch)
                                .AsNoTracking()
                                .ToArrayAsync().ConfigureAwait(false);

                            count = programmes.Length;

                            foreach (var programme in programmes)
                            {
                                dbContext.Specific.Attach(programme).State = EntityState.Deleted;
                            }

                            await dbContext.SaveChangesWithConcurrencyConflictsResolvingAsync()
                                .ConfigureAwait(false);
                        }
                    } while (count == DeletionBatch);
                }
            }).GetAwaiter().GetResult();
        }

        protected class ProgrammDeleteBatch
        {
            public static ProgrammDeleteBatch Create(IProgrammesDeleted deletedEvent)
            {
                return new ProgrammDeleteBatch
                {
                    SalesArea = deletedEvent.SalesArea,
                    FromDate = deletedEvent.FromDate,
                    ToDate = deletedEvent.ToDate
                };
            }

            public string SalesArea { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }
    }
}
