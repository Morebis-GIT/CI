using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Queries;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto.Internal;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions;
using RunScenarioEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs.RunScenario;
using RunStatus = ImagineCommunications.GamePlan.Domain.Runs.RunStatus;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class RunRepository : IRunRepository
    {
        private readonly List<ScenarioStatus> _scheduledOrRunningStatuses = new List<ScenarioStatus>
        {
            ScenarioStatus.Scheduled,
            ScenarioStatus.Starting,
            ScenarioStatus.Smoothing,
            ScenarioStatus.InProgress,
            ScenarioStatus.GettingResults
        };

        private readonly ExternalScenarioStatus[] _triggeredInLandmarkStatuses =
        {
            ExternalScenarioStatus.Accepted,
            ExternalScenarioStatus.Scheduled,
            ExternalScenarioStatus.Running
        };

        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IFullTextSearchConditionBuilder _searchConditionBuilder;
        private readonly IMapper _mapper;

        public RunRepository(ISqlServerTenantDbContext dbContext, IFullTextSearchConditionBuilder searchConditionBuilder, IMapper mapper)
        {
            _dbContext = dbContext;
            _searchConditionBuilder = searchConditionBuilder;
            _mapper = mapper;
        }

        private IQueryable<Run> GetRunByExpression(Expression<Func<Entities.Tenant.Runs.Run, bool>> expression) =>
            _dbContext.Query<Entities.Tenant.Runs.Run>()
                .Where(expression)
                .ProjectTo<Run>(_mapper.ConfigurationProvider);

        public Run Find(Guid id) =>
            _dbContext.Query<Entities.Tenant.Runs.Run>().ProjectTo<Run>(_mapper.ConfigurationProvider).FirstOrDefault(x => x.Id == id);

        public Guid GetRunIdForScenario(Guid scenarioId) =>
            _dbContext.Query<Entities.Tenant.Runs.Run>()
                .Where(x => x.Scenarios.Any(s => s.ScenarioId == scenarioId))
                .Select(x => x.Id)
                .FirstOrDefault();

        public IEnumerable<Run> FindByIds(IEnumerable<Guid> ids) =>
            _dbContext.Query<Entities.Tenant.Runs.Run>().Where(r => ids.Contains(r.Id))
                .ProjectTo<Run>(_mapper.ConfigurationProvider).ToList();

        public Run FindByScenarioId(Guid scenarioId) =>
            _dbContext.Query<Entities.Tenant.Runs.Run>()
                .Where(x => x.Scenarios.Any(s => s.ScenarioId == scenarioId)).ProjectTo<Run>(_mapper.ConfigurationProvider).FirstOrDefault();

        public IEnumerable<Run> GetByScenarioId(Guid scenarioId) =>
            _dbContext.Query<Entities.Tenant.Runs.Run>()
                .Where(x => x.Scenarios.Any(s => s.ScenarioId == scenarioId)).ProjectTo<Run>(_mapper.ConfigurationProvider).ToList();

        public IEnumerable<Run> GetAll() =>
            _dbContext.Query<Entities.Tenant.Runs.Run>().ProjectTo<Run>(_mapper.ConfigurationProvider).ToList();

        public IEnumerable<Run> GetAllActive()
        {
            var recent = DateTime.UtcNow.AddHours(-48);
            return _dbContext.Query<Entities.Tenant.Runs.Run>()
                .Where(r => r.ExecuteStartedDateTime >= recent &&
                    r.Scenarios.Any(s => _scheduledOrRunningStatuses.Contains(s.Status)))
                .ProjectTo<Run>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public IEnumerable<RunsWithScenarioIdTransformerResult> GetRunsWithScenarioId() =>
            _dbContext.Query<Entities.Tenant.Runs.RunScenario>().Select(x =>
                new RunsWithScenarioIdTransformerResult
                {
                    Id = $"runs/{x.RunId}",
                    ScenarioId = x.ScenarioId
                }).ToList();

        public Run FindByExternalRunId(Guid externalRunId) =>
            GetRunByExpression(x => x.Scenarios.Any(s => s.ExternalRunInfo != null && s.ExternalRunInfo.ExternalRunId == externalRunId))
                .SingleOrDefault();

        public IEnumerable<Run> FindTriggeredInLandmark() =>
            GetRunByExpression(x => x.Scenarios.Any(s =>
                s.ExternalRunInfo != null &&
                _triggeredInLandmarkStatuses.Contains(s.ExternalRunInfo.ExternalStatus)));

        public IEnumerable<Run> FindLandmarkRuns() =>
            GetRunByExpression(x => x.Scenarios.Any(s => s.ExternalRunInfo != null));

        public void Add(Run run)
        {
            var entity = _mapper.Map<Entities.Tenant.Runs.Run>(run);
            ApplyScenariosOrder(entity.Scenarios);

            _dbContext.Add(entity, post => post.MapTo(run), _mapper);
        }

        public void Update(Run run)
        {
            var entity = GetQueryWithAllIncludes()
                .FirstOrDefault(x => x.Id == run.Id);

            if (entity != null)
            {
                _mapper.Map(run, entity);
                ApplyScenariosOrder(entity.Scenarios);

                _dbContext.Update(entity, post => post.MapTo(run), _mapper);
            }
        }

        public void UpdateRange(IEnumerable<Run> runs)
        {
            var entities = GetQueryWithAllIncludes()
                .Where(x => runs.Any(y => y.Id == x.Id))
                .ToArray();

            if (!entities.Any())
            {
                return;
            }

            foreach (var entity in entities)
            {
                var run = runs.SingleOrDefault(x => x.Id == entity.Id);

                if (run != null)
                {
                    _mapper.Map(run, entity);
                }
            }

            _dbContext.UpdateRange(entities, post => post.MapToCollection(runs), _mapper);
        }

        public bool Exists(Guid id) =>
            _dbContext.Query<Entities.Tenant.Runs.Run>().ProjectTo<Run>(_mapper.ConfigurationProvider).Any(r => r.Id == id);

        public void Remove(Guid id)
        {
            var entity = _dbContext.Query<Entities.Tenant.Runs.Run>()
                .Include(e => e.Scenarios)
                .FirstOrDefault(e => e.Id == id);

            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void SaveChanges() => _dbContext.SaveChanges();

        public PagedQueryResult<RunExtendedSearchModel> Search(RunSearchQueryModel queryModel, StringMatchRules freeTextMatchRules)
        {
            if (queryModel == null)
            {
                throw new ArgumentNullException(nameof(queryModel));
            }

            if (queryModel.Orderby?.Any() ?? false)
            {
                queryModel.Orderby.ForEach(i =>
                {
                    if (string.Equals(i.OrderBy, "ExecuteStartedDateTime", StringComparison.InvariantCultureIgnoreCase))
                    {
                        i.OrderBy = nameof(RunDto.CreatedOrExecuteDateTime);
                    }
                });
            }

            var query =
                (from run in _dbContext.Query<Entities.Tenant.Runs.Run>()
                    join rsj in _dbContext.Query<Entities.Tenant.Runs.RunScenario>() on run.Id equals rsj.RunId
                    join sj in _dbContext.Query<Entities.Tenant.Scenarios.Scenario>() on rsj.ScenarioId equals sj.Id
                        into scenarios
                    from s in scenarios.DefaultIfEmpty()
                    join prj in _dbContext.Query<Entities.Tenant.Scenarios.ScenarioPassReference>() on s.Id equals prj.ScenarioId
                        into passReferences
                     from pr in passReferences.DefaultIfEmpty()
                     join pj in _dbContext.Query<Pass>() on pr.PassId equals pj.Id
                        into passes
                    from p in passes.DefaultIfEmpty()
                    select new { run, s, p}).AsQueryable();

            if (queryModel.RunPeriodStartDate.HasValue && queryModel.RunPeriodEndDate.HasValue)
            {
                query = query.Where(q => q.run.StartDate <= queryModel.RunPeriodEndDate &&
                    q.run.EndDate >= queryModel.RunPeriodStartDate);
            }

            if (queryModel.ExecutedStartDate.HasValue && queryModel.ExecutedEndDate.HasValue)
            {
                query = query.Where(q => (q.run.ExecuteStartedDateTime ?? q.run.CreatedDateTime) >= queryModel.ExecutedStartDate &&
                    (q.run.ExecuteStartedDateTime ?? q.run.CreatedDateTime) <= queryModel.ExecutedEndDate);
            }

            if (queryModel.Users?.Any() ?? false)
            {
                query = query.Where(x => queryModel.Users.Contains(x.run.Author.AuthorId));
            }

            if (queryModel.Status?.Any() ?? false)
            {
                var entityStatuses = queryModel.Status.Cast<Entities.RunStatus>().ToArray();
                query = query.Where(x => entityStatuses.Contains(x.run.RunStatus));
            }

            /* redundant code that should be reviewed. all operations with searching by description are done in the ApplyFreeTextMatchRules method that is called at the end of these method */
            


            if (!string.IsNullOrEmpty(queryModel.Description))
            {
                //This piece of code will be replaced via Match functionality 
                query = query.Where(q =>
                    q.run.Id.ToString().Contains(queryModel.Description)
                    || q.run.Description.Contains(queryModel.Description)
                    || q.run.Author.Name.Contains(queryModel.Description)
                    || q.s.Id.ToString().Contains(queryModel.Description)
                    || q.s.Name.Contains(queryModel.Description)
                    || q.p.Id.ToString().Contains(queryModel.Description)
                    || q.p.Name.Contains(queryModel.Description));
                if (freeTextMatchRules.HowManyWordsToMatch == StringMatchHowManyWordsToMatch.AllWords)
                {
                    query = query.MakeContainsAll();
                }
                if (freeTextMatchRules.HowManyWordsToMatch == StringMatchHowManyWordsToMatch.AnyWord)
                {
                    query = query.MakeContainsAny();
                }
                if (!freeTextMatchRules.CaseSensitive)
                {
                    query = query.MakeCaseInsensitive();
                }

                //
            }



            var filteredQuery = query.Select(x => new
            {
                x.run.Id,
                AuthorId = x.run.Author.Id,
                AuthorName = x.run.Author.Name,
                x.run.RunStatus,
                x.run.StartDate,
                x.run.StartTime,
                x.run.EndDate,
                x.run.EndTime,
                x.run.CreatedOrExecuteDateTime,
                x.run.Description
            }).Distinct();

            var extendedSearchQuery = queryModel.Orderby != null
                ? filteredQuery.OrderByMultipleItems(queryModel.Orderby)
                : filteredQuery.OrderBySingleItem(nameof(Entities.Tenant.Runs.Run.Id), OrderDirection.Asc);

            // If there is a valid description then do not apply paging now,
            // since we need to filter further by applying free text matching rules
            var shouldApplyFreeTextMatchRules = !string.IsNullOrWhiteSpace(queryModel.Description);
            extendedSearchQuery = shouldApplyFreeTextMatchRules ?
                extendedSearchQuery :
                extendedSearchQuery.ApplyDefaultPaging(queryModel.Skip, queryModel.Top);

            var totalCount = filteredQuery.Count();
            var runIds = extendedSearchQuery.Select(x => x.Id).ToList();

            var runs = _mapper.Map<List<RunExtendedSearchModel>>(_dbContext.Query<Entities.Tenant.Runs.Run>()
                .Include(x => x.Author)
                .Include(x => x.Scenarios)
                .ThenInclude(x => x.Scenario)
                .ThenInclude(x => x.PassReferences)
                .ThenInclude(x => x.Pass)
                .Include(x => x.Scenarios)
                .ThenInclude(x => x.ExternalRunInfo)
                .Where(x => runIds.Contains(x.Id))
                .AsEnumerable()
                .Select(x => new RunDto
                {
                    Id = x.Id,
                    CustomId = x.CustomId,
                    Description = x.Description,
                    CreatedDateTime = x.CreatedDateTime,
                    StartDate = x.StartDate,
                    StartTime = x.StartTime,
                    EndDate = x.EndDate,
                    EndTime = x.EndTime,
                    LastModifiedDateTime = x.LastModifiedDateTime,
                    ExecuteStartedDateTime = x.ExecuteStartedDateTime,
                    IsLocked = x.IsLocked,
                    InventoryLock = x.InventoryLock,
                    Real = x.Real,
                    Smooth = x.Smooth,
                    AuthorId = x.Author.Id,
                    AuthorName = x.Author.Name,
                    SmoothDateStart = x.SmoothDateStart,
                    SmoothDateEnd = x.SmoothDateEnd,
                    ISR = x.ISR,
                    ISRDateStart = x.ISRDateStart,
                    ISRDateEnd = x.ISRDateEnd,
                    Optimisation = x.Optimisation,
                    OptimisationDateStart = x.OptimisationDateStart,
                    OptimisationDateEnd = x.OptimisationDateEnd,
                    RightSizer = x.RightSizer,
                    RightSizerDateStart = x.RightSizerDateStart,
                    RightSizerDateEnd = x.RightSizerDateEnd,
                    SpreadProgramming = x.SpreadProgramming,
                    SkipLockedBreaks = x.SkipLockedBreaks,
                    IgnorePremiumCategoryBreaks = x.IgnorePremiumCategoryBreaks,
                    ExcludeBankHolidays = x.ExcludeBankHolidays,
                    ExcludeSchoolHolidays = x.ExcludeSchoolHolidays,
                    Objectives = x.Objectives,
                    CreatedOrExecuteDateTime = x.CreatedOrExecuteDateTime,
                    RunStatus = x.RunStatus,
                    RunScenarios = x.Scenarios.ToList(),
                    Scenarios = x.Scenarios.Select(rs => rs.Scenario).ToList(),
                    Passes = x.Scenarios.Where(rs => rs.Scenario != null).SelectMany(rs => rs.Scenario.PassReferences.Select(p => p.Pass)).ToList()
                }).OrderBy(x => runIds.IndexOf(x.Id)));

            if (shouldApplyFreeTextMatchRules && totalCount > 0)
            {
                // Filter runs, necessary because Sql server cannot filter sufficiently. This is nasty
                // but necessary because Sql server cannot filter the data as required
                runs = ApplyFreeTextMatchRules(runs, freeTextMatchRules, queryModel.Description);
                totalCount = runs.Count;
                runs = runs.ApplyDefaultPaging(queryModel.Skip, queryModel.Top).ToList();
            }

            return new PagedQueryResult<RunExtendedSearchModel>(totalCount, runs);
        }

        public IEnumerable<Run> GetRunsByCampaignExternalIdsAndStatus(IEnumerable<string> externalIds, RunStatus runStatus)
        {
            var entityRunStatus = (Entities.RunStatus) runStatus;
            return _dbContext.Query<Entities.Tenant.Runs.Run>()
                .Where(x => x.RunStatus == entityRunStatus &&
                    (!x.Campaigns.Any() || x.Campaigns.Any(c => externalIds.Contains(c.ExternalId))))
                .ProjectTo<Run>(_mapper.ConfigurationProvider)
                .ToList();
        }

        public IEnumerable<Run> GetRunsByDeliveryCappingGroupId(int id) =>
            _dbContext.Query<Entities.Tenant.Runs.Run>()
                .Where(x => x.CampaignsProcessesSettings.Any(y => y.DeliveryCappingGroupId == id))
                .ProjectTo<Run>(_mapper.ConfigurationProvider)
                .ToList();

        private List<RunExtendedSearchModel> ApplyFreeTextMatchRules(List<RunExtendedSearchModel> runs, StringMatchRules freeTextMatchRules, string description)
        {
            if (string.IsNullOrWhiteSpace(description)) { return runs; }

            return runs.Where(r => freeTextMatchRules.IsMatches(r.Description, description) ||
                (!string.IsNullOrWhiteSpace(r.Author?.Name) &&
                    freeTextMatchRules.IsMatches(r.Author.Name, description)) ||
                freeTextMatchRules.IsMatches(r.Id.ToString(), description) ||
                r.ScenarioIds.Any(id => freeTextMatchRules.IsMatches(id, description)) ||
                r.ScenarioNames.Any(name => freeTextMatchRules.IsMatches(name, description)) ||
                r.PassNames.Any(name => freeTextMatchRules.IsMatches(name, description)) ||
                r.PassIds.Any(id => freeTextMatchRules.IsMatches(id, description))).ToList();
        }

        private static void ApplyScenariosOrder(IEnumerable<RunScenarioEntity> passReferences)
        {
            var order = 1;
            foreach (var pr in passReferences)
            {
                pr.Order = order++;
            }
        }

        private IQueryable<Entities.Tenant.Runs.Run> GetQueryWithAllIncludes() =>
            _dbContext.Query<Entities.Tenant.Runs.Run>()
                .Include(x => x.Author)
                .Include(x => x.Campaigns)
                .Include(x => x.Scenarios)
                .Include(x => x.CampaignsProcessesSettings)
                .Include(x => x.InventoryLock)
                .Include(x => x.ExcludedInventoryStatuses)
                .Include(x => x.SalesAreaPriorities)
                .Include(x => x.AnalysisGroupTargets)
                .Include(x => x.ScheduleSettings);
    }
}
