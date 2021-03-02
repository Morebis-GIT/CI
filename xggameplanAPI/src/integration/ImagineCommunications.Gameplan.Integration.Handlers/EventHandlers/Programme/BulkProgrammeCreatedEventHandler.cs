using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Programme;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Counters;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Caches;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Cache;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using ProgrammeCategoryEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes.ProgrammeCategory;
using ProgrammeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes.Programme;
using ProgrammeEpisodeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes.ProgrammeEpisode;
using ScheduleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules.Schedule;
using ScheduleProgrammeEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules.ScheduleProgramme;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Programme
{
    public class BulkProgrammeCreatedEventHandler : IEventHandler<IBulkProgrammeCreated>, IBatchingEnumerateHandler<IBulkProgrammeCreated, IProgrammeCreated>
    {
        private readonly IMapper _mapper;
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;
        private readonly ProgrammePrgtNoSequenceCounter _prgtNoCounter = new ProgrammePrgtNoSequenceCounter();

        private ProgrammeDictionaryCache _programmeDictionaryCache;
        private ProgrammeCategoryCache _programmeCategoryCache;
        private IEntityCache<string, SalesArea> _salesAreaCache;
        private HashSet<string> _categoryHierarchyNames;
        private IEntityCache<int, ScheduleEntity> _scheduleCache;
        private bool _cachesInitialized;

        public BulkProgrammeCreatedEventHandler(
            ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory,
            IMapper mapper)
        {
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
        }

        public void Handle(IBulkProgrammeCreated command)
        {
            using (var enumerator = command.Data.OrderBy(x => x.StartDateTime).GetEnumerator())
            {
                Handle(enumerator);
            }
        }

        public void Handle(IEnumerator<IProgrammeCreated> eventEnumerator)
        {
            InitializeCaches();

            var prgDictToProcess = new HashSet<ProgrammeDictionary>();
            var prgCatToProcess = new List<ProgrammeCategoryEntity>();
            var prgToProcess = new List<ProgrammeEntity>();
            var prgCatLinkToProcess = new List<ProgrammeCategoryLink>();
            var prgDictEpisodeToProcess = new HashSet<ProgrammeEpisode>();
            var scheduleToProcess = new List<ScheduleEntity>();
            var schPrgToProcess = new List<ScheduleProgrammeEntity>();

            while (eventEnumerator.MoveNext())
            {
                var eventModel = eventEnumerator.Current;

                if (eventModel is null)
                {
                    continue;
                }

                var salesArea = _salesAreaCache.Get(eventModel.SalesArea);
                if (salesArea is null)
                {
                    throw new DataSyncException(DataSyncErrorCode.SalesAreaNotFound,
                        $"Invalid Sales Area: {eventModel.SalesArea}");
                }

                if (!(eventModel.ProgrammeCategories is null) && eventModel.ProgrammeCategories.Any())
                {
                    var invalidCategories = eventModel.ProgrammeCategories
                        .Except(_categoryHierarchyNames, StringComparer.InvariantCultureIgnoreCase).ToArray();
                    if (invalidCategories.Length != 0)
                    {
                        throw new DataSyncException(DataSyncErrorCode.ProgrammeCategoryNotFound,
                            "Invalid programme categories: " + String.Join(",", invalidCategories));
                    }
                }

                var programmeDictionary = _programmeDictionaryCache.Get(eventModel.ExternalReference);

                if (programmeDictionary is null)
                {
                    programmeDictionary = _mapper.Map<ProgrammeDictionary>(eventModel);
                    _programmeDictionaryCache.Add(programmeDictionary);
                }
                else
                {
                    programmeDictionary.Name = eventModel.ProgrammeName;
                    programmeDictionary.Description = eventModel.Description;
                    programmeDictionary.Classification = eventModel.Classification;
                }

                _ = prgDictToProcess.Add(programmeDictionary);

                var programme = _mapper.Map<ProgrammeEntity>(eventModel);
                programme.Id = Guid.NewGuid();
                programme.ProgrammeDictionary = programmeDictionary;
                programme.SalesAreaId = salesArea.Id;
                _prgtNoCounter.Process(programme);
                prgToProcess.Add(programme);

                foreach (var catName in eventModel.ProgrammeCategories ?? Enumerable.Empty<string>())
                {
                    var category = _programmeCategoryCache.GetOrAdd(catName, key =>
                    {
                        var newCat = new ProgrammeCategoryEntity { Name = key };
                        prgCatToProcess.Add(newCat);

                        return newCat;
                    });

                    if (programme.ProgrammeCategoryLinks.All(x => !String.Equals(x.ProgrammeCategory.Name, category.Name)))
                    {
                        var link = new ProgrammeCategoryLink
                        {
                            ProgrammeCategory = category,
                            Programme = programme,
                            ProgrammeId = programme.Id
                        };

                        programme.ProgrammeCategoryLinks.Add(link);
                        prgCatLinkToProcess.Add(link);
                    }
                }

                if (!(eventModel.Episode is null))
                {
                    var episode = programmeDictionary.ProgrammeEpisodes.FirstOrDefault(x => x.Number == eventModel.Episode.Number);

                    if (episode is null)
                    {
                        episode = _mapper.Map<ProgrammeEpisodeEntity>(eventModel.Episode);
                        episode.ProgrammeDictionary = programmeDictionary;

                        programmeDictionary.ProgrammeEpisodes.Add(episode);
                    }
                    else
                    {
                        episode.Name = eventModel.Episode.Name;
                    }

                    programme.Episode = episode;
                    _ = prgDictEpisodeToProcess.Add(episode);
                }

                var schedule = _scheduleCache.GetOrAdd(programme.ScheduleUniqueKey,
                    key =>
                    {
                        var sch = new ScheduleEntity
                        {
                            SalesAreaId = programme.SalesAreaId,
                            Date = programme.StartDateTime.Date
                        };
                        scheduleToProcess.Add(sch);

                        return sch;
                    });

                schPrgToProcess.Add(new ScheduleProgrammeEntity { Programme = programme, Schedule = schedule });
            }

            using (var dbContext = _dbContextFactory.Create())
            {
                using (var transaction = dbContext.Specific.Database.BeginTransaction())
                {
                    dbContext.BulkInsertEngine.BulkInsertOrUpdate(prgDictToProcess.ToList(),
                        new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

                    foreach (var episode in prgDictEpisodeToProcess)
                    {
                        episode.ProgrammeDictionaryId = episode.ProgrammeDictionary.Id;
                    }

                    dbContext.BulkInsertEngine.BulkInsertOrUpdate(prgDictEpisodeToProcess.ToList(),
                        new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

                    dbContext.BulkInsertEngine.BulkInsert(prgCatToProcess,
                        new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

                    foreach (var prg in prgToProcess)
                    {
                        prg.ProgrammeDictionaryId = prg.ProgrammeDictionary.Id;
                        prg.EpisodeId = prg.Episode?.Id;
                    }

                    dbContext.BulkInsertEngine.BulkInsert(prgToProcess,
                        new BulkInsertOptions { PreserveInsertOrder = true });

                    foreach (var catLink in prgCatLinkToProcess)
                    {
                        catLink.ProgrammeCategoryId = catLink.ProgrammeCategory.Id;
                    }

                    dbContext.BulkInsertEngine.BulkInsert(prgCatLinkToProcess);

                    dbContext.BulkInsertEngine.BulkInsert(scheduleToProcess,
                        new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

                    foreach (var scheduleProgramme in schPrgToProcess)
                    {
                        scheduleProgramme.ProgrammeId = scheduleProgramme.Programme.Id;
                        scheduleProgramme.ScheduleId = scheduleProgramme.Schedule.Id;
                    }

                    dbContext.BulkInsertEngine.BulkInsert(schPrgToProcess);

                    transaction.Commit();
                }
            }
        }

        protected void InitializeCaches()
        {
            if (!_cachesInitialized)
            {
                using (var dbContext = _dbContextFactory.Create())
                {
                    _programmeDictionaryCache = new ProgrammeDictionaryCache(dbContext, trackingChanges: false);
                    _programmeDictionaryCache.Load();

                    _programmeCategoryCache = new ProgrammeCategoryCache(dbContext, trackingChanges: false);
                    _programmeCategoryCache.Load();

                    _scheduleCache =
                        new SqlServerEntityCache<int, ScheduleEntity>(dbContext, x => x.ScheduleUniqueKey, trackingChanges: false);
                    _scheduleCache.Load();

                    _salesAreaCache = new SqlServerEntityCache<string, SalesArea>(dbContext, x => x.Name, trackingChanges: false);
                    _salesAreaCache.Load();

                    _categoryHierarchyNames = new HashSet<string>(dbContext.Query<ProgrammeCategoryHierarchy>().Select(x => x.Name)
                        .AsEnumerable());
                }

                _cachesInitialized = true;
            }
        }
    }
}
