using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Caches;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.common.Extensions;
using ProgrammeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes.Programme;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Programme
{
    public class BulkProgrammeUpdatedEventHandler : IEventHandler<IBulkProgrammeUpdated>
    {
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;
        private readonly ILoggerService _logger;

        public BulkProgrammeUpdatedEventHandler(
            ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory,
            ILoggerService logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public void Handle(IBulkProgrammeUpdated command)
        {
            Task.Run(async () =>
            {
                var actionBlock = new ActionBlock<(int Id, IProgrammeUpdated UpdateInfo)>(
                    x => UpdateProgrammeAsync(x.Id, x.UpdateInfo),
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });

                using (var dbContext = _dbContextFactory.Create())
                {
                    var programmeDictionaryCache = new ProgrammeDictionaryCache(dbContext);

                    try
                    {
                        foreach (var cmd in command.Data.GroupBy(x => x.ExternalReference).Select(x => x.Last()))
                        {
                            var programmDictionary = programmeDictionaryCache.Get(cmd.ExternalReference);

                            if (programmDictionary is null)
                            {
                                _logger?.Warn($" Handler: {GetType().Name}. Invalid programme external reference, updating skipped",
                                    cmd.ExternalReference);
                                continue;
                            }

                            programmDictionary.Name = cmd.ProgrammeName;
                            programmDictionary.Description = cmd.Description;
                            programmDictionary.Classification = cmd.Classification;

                            _ = await actionBlock.SendAsync((programmDictionary.Id, cmd)).ConfigureAwait(false);

                            await dbContext.SaveChangesAsync().ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        actionBlock.Complete();
                        var exceptions = await actionBlock.Completion.WaitWithTaskExceptionGatheringAsync().ConfigureAwait(false);

                        if (exceptions.Count == 0)
                        {
                            throw;
                        }

                        throw new AggregateException(
                            "Updating of programmes finished with some errors. See inner exception for more details.",
                            new AggregateException(new[] { ex }.Union(exceptions)));
                    }
                }

                actionBlock.Complete();
                await actionBlock.Completion.AggregateExceptions().ConfigureAwait(false);

                async Task UpdateProgrammeAsync(int id, IProgrammeUpdated updateInfo)
                {
                    using (var dbContext = _dbContextFactory.Create())
                    {
                        var programmes = await dbContext.Query<ProgrammeEntity>()
                            .Where(p => p.ProgrammeDictionaryId == id && p.LiveBroadcast != updateInfo.LiveBroadcast)
                            .Select(p => new ProgrammeEntity { Id = p.Id }).AsNoTracking().ToArrayAsync()
                            .ConfigureAwait(false);

                        foreach (var programme in programmes)
                        {
                            var entry = dbContext.Specific.Attach(programme);
                            entry.Property(x => x.LiveBroadcast).CurrentValue = updateInfo.LiveBroadcast;
                        }

                        await dbContext.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
            }).GetAwaiter().GetResult();
        }
    }
}
