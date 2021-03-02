using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions.AutoMapper;
using Schedule = ImagineCommunications.GamePlan.Domain.Shared.Schedules.Schedule;
using ScheduleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules.Schedule;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;

        private readonly IDictionary<int, ScheduleEntity> _scheduleCache =
            new Dictionary<int, ScheduleEntity>();

        public ScheduleRepository(
            ISqlServerTenantDbContext dbContext,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
        }

        private void UpdateScheduleBreaks(Schedule scheduleModel, ScheduleEntity updatedEntity, bool isNew = false)
        {
            if (scheduleModel.Breaks?.Any() ?? false)
            {
                if (!isNew)
                {
                    //delete breaks
                    updatedEntity.Breaks.Where(b => scheduleModel.Breaks.All(mb => mb.Id != b.Id)).ToList()
                        .ForEach(b => updatedEntity.Breaks.Remove(b));

                    //update breaks
                    foreach (var b in updatedEntity.Breaks)
                    {
                        var model = scheduleModel.Breaks.FirstOrDefault(x => x.Id == b.Id);
                        if (model != null)
                        {
                            _ = _mapper.Map(model, b, opts => opts.UseEntityCache(_salesAreaByNameCache));

                            #region Update Break Efficiency

                            if (model.BreakEfficiencyList?.Any() ?? false)
                            {
                                //delete break efficiency
                                b.BreakEfficiencies.Where(be => model.BreakEfficiencyList.All(mbe =>
                                        !string.Equals(mbe.Demographic, be.Demographic,
                                            StringComparison.InvariantCultureIgnoreCase)))
                                    .ToList()
                                    .ForEach(be => b.BreakEfficiencies.Remove(be));

                                //update break efficiency
                                foreach (var be in b.BreakEfficiencies)
                                {
                                    var mbe = model.BreakEfficiencyList.FirstOrDefault(x =>
                                        string.Equals(x.Demographic, be.Demographic,
                                            StringComparison.InvariantCultureIgnoreCase));
                                    if (mbe != null)
                                    {
                                        _mapper.Map(mbe, be);
                                    }
                                }

                                //add break efficiency
                                _mapper.Map<List<ScheduleBreakEfficiency>>(
                                        model.BreakEfficiencyList.Where(mbe => b.BreakEfficiencies.All(be =>
                                            !string.Equals(be.Demographic, mbe.Demographic,
                                                StringComparison.InvariantCultureIgnoreCase))))
                                    .ForEach(be => b.BreakEfficiencies.Add(be));
                            }
                            else
                            {
                                b.BreakEfficiencies.Clear();
                            }

                            #endregion Update Break Efficiency
                        }
                    }
                }

                //add breaks
                var newBreaks = scheduleModel.Breaks.Where(mb => updatedEntity.Breaks.All(b => b.Id != mb.Id)).ToList();
                if (newBreaks.Any())
                {
                    var idx = 0;
                    foreach (var b in _mapper.Map<List<ScheduleBreak>>(newBreaks, opts => opts.UseEntityCache(_salesAreaByNameCache)))
                    {
                        var efficiencyList = newBreaks[idx].BreakEfficiencyList;
                        if (efficiencyList?.Any() ?? false)
                        {
                            b.BreakEfficiencies = _mapper.Map<List<ScheduleBreakEfficiency>>(efficiencyList);
                        }
                        updatedEntity.Breaks.Add(b);
                    }
                }
            }
            else
            {
                updatedEntity.Breaks.Clear();
            }
        }

        private void UpdateScheduleProgrammes(Schedule scheduleModel, ScheduleEntity updatedEntity, bool isNew = false)
        {
            if (scheduleModel.Programmes?.Any() ?? false)
            {
                if (!isNew)
                {
                    //delete programmes
                    updatedEntity.Programmes.Where(p => scheduleModel.Programmes.All(mp => mp.Id != p.ProgrammeId)).ToList()
                        .ForEach(p => updatedEntity.Programmes.Remove(p));

                    //update programmes
                    foreach (var p in updatedEntity.Programmes)
                    {
                        var model = scheduleModel.Programmes.FirstOrDefault(mp => mp.Id == p.ProgrammeId);
                        if (model != null)
                        {
                            _ = _mapper.Map(model, p, opts => opts.UseEntityCache(_salesAreaByNameCache));
                        }
                    }
                }

                //add programmes
                _mapper.Map<List<ScheduleProgramme>>(
                        scheduleModel.Programmes.Where(mp => updatedEntity.Programmes.All(p => p.ProgrammeId != mp.Id)), opts => opts.UseEntityCache(_salesAreaByNameCache))
                    .ForEach(p => updatedEntity.Programmes.Add(p));
            }
            else
            {
                updatedEntity.Programmes.Clear();
            }
        }

        private IQueryable<ScheduleEntity> GetScheduleQueryWithAllIncludes()
        {
            return _dbContext.Query<ScheduleEntity>()
                .Include(x => x.Breaks).ThenInclude(x => x.BreakEfficiencies)
                .Include(x => x.Programmes).ThenInclude(x => x.Programme).ThenInclude(x => x.ProgrammeCategoryLinks)
                .ThenInclude(x => x.ProgrammeCategory)
                .Include(x => x.Programmes).ThenInclude(x => x.Programme).ThenInclude(x => x.Episode)
                .Include(x => x.Programmes).ThenInclude(x => x.Programme).ThenInclude(x => x.ProgrammeDictionary);
        }

        private ScheduleEntity GetEntityById(int id)
        {
            if (_scheduleCache.TryGetValue(id, out var entity))
            {
                return entity;
            }

            entity = GetScheduleQueryWithAllIncludes().FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                _scheduleCache.Add(entity.Id, entity);
            }

            return entity;
        }

        public Schedule Get(int id)
        {
            return _mapper.Map<Schedule>(GetEntityById(id), opts => opts.UseEntityCache(_salesAreaByIdCache));
        }

        public void Delete(int id)
        {
            if (_scheduleCache.TryGetValue(id, out var entity))
            {
                _ = _scheduleCache.Remove(entity.Id);
                _dbContext.Remove(entity);
            }
            else
            {
                entity = _dbContext.Find<ScheduleEntity>(id);
                if (entity != null)
                {
                    _dbContext.Remove(entity);
                }
            }
        }

        public IEnumerable<Schedule> GetAll()
        {
            _scheduleCache.Clear();
            var entities = GetScheduleQueryWithAllIncludes().AsNoTracking().ToList();
            entities.ForEach(s => _scheduleCache.Add(s.Id, s));

            return _mapper.Map<List<Schedule>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
        }

        public int CountAll => _dbContext.Query<ScheduleEntity>().Count();

        public int Count(Expression<Func<Schedule, bool>> query) =>
            _dbContext.Query<ScheduleEntity>().ProjectTo<Schedule>(_mapper.ConfigurationProvider).Count(query);

        public (int breaksCount, int programmesCount) CountBreaksAndProgrammes(DateTime dateFrom, DateTime dateTo)
        {
            var stats = _dbContext.Query<ScheduleEntity>()
                .Where(s => s.Date >= dateFrom && s.Date <= dateTo)
                .Select(s => new
                {
                    BreakCount = s.Breaks.Count,
                    ProgrammeCount = s.Programmes.Count
                }).ToList();

            return (stats.Sum(x => x.BreakCount), stats.Sum(x => x.ProgrammeCount));
        }

        public void Add(Schedule item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var isNew = true;
            if (item.Id > 0)
            {
                var entity = GetEntityById(item.Id);
                if (entity != null)
                {
                    _ = _mapper.Map(item, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                    UpdateScheduleBreaks(item, entity);
                    UpdateScheduleProgrammes(item, entity);
                    _ = _dbContext.Update(entity,
                        post => post.MapTo(item, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
                    isNew = false;
                }
            }

            if (isNew)
            {
                var entity = _mapper.Map<ScheduleEntity>(item, opts => opts.UseEntityCache(_salesAreaByNameCache));
                UpdateScheduleBreaks(item, entity, true);
                UpdateScheduleProgrammes(item, entity, true);
                _ = _dbContext.Add(entity, post => post.MapTo(item, opts => opts.UseEntityCache(_salesAreaByIdCache)),
                    _mapper);
            }
        }

        public void Update(Schedule item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Id > 0)
            {
                var entity = GetEntityById(item.Id);
                if (entity != null)
                {
                    _ = _mapper.Map(item, entity, opts => opts.UseEntityCache(_salesAreaByNameCache));
                    UpdateScheduleBreaks(item, entity);
                    UpdateScheduleProgrammes(item, entity);
                    _ = _dbContext.Update(entity,
                        post => post.MapTo(item, opts => opts.UseEntityCache(_salesAreaByIdCache)), _mapper);
                }
            }
        }

        public Schedule GetSchedule(string salesareaname, DateTime scheduledate)
        {
            var entity = GetScheduleQueryWithAllIncludes()
                .FirstOrDefault(s => s.SalesArea.Name == salesareaname && s.Date == scheduledate);
            if (entity != null)
            {
                if (!_scheduleCache.ContainsKey(entity.Id))
                {
                    _scheduleCache.Add(entity.Id, entity);
                }
            }

            return _mapper.Map<Schedule>(entity, opts => opts.UseEntityCache(_salesAreaByIdCache));
        }

        public int GetScheduleBreaksCount(string salesAreaName, DateTime scheduleDate)
        {
            return _dbContext.Query<ScheduleEntity>()
                .Where(s => s.SalesArea.Name == salesAreaName && s.Date == scheduleDate)
                .Take(1).SelectMany(x => x.Breaks).Count();
        }

        public List<Schedule> GetSchedule(List<string> salesareanames, DateTime fromdate, DateTime todate)
        {
            var entities = GetScheduleQueryWithAllIncludes().Where(s =>
                    salesareanames.Contains(s.SalesArea.Name) && s.Date >= fromdate.Date && s.Date < todate.Date.AddDays(1))
                .AsNoTracking().ToList();
            if (entities.Any())
            {
                entities.ForEach(s =>
                {
                    if (!_scheduleCache.ContainsKey(s.Id))
                    {
                        _scheduleCache.Add(s.Id, s);
                    }
                });
            }

            return _mapper.Map<List<Schedule>>(entities, opts => opts.UseEntityCache(_salesAreaByIdCache));
        }

        public List<BreakSimple> GetScheduleSimpleBreaks(List<string> salesAreaNames, DateTime fromDate, DateTime toDate)
        {
            return _dbContext.Query<ScheduleEntity>().Include(x => x.SalesArea)
                .Where(x => x.Date >= fromDate.Date && x.Date < toDate.Date.AddDays(1) && salesAreaNames.Contains(x.SalesArea.Name))
                .SelectMany(s => s.Breaks.Select(b =>
                    new BreakSimple
                    {
                        SalesAreaName = b.SalesArea.Name,
                        ScheduleDate = b.ScheduledDate.Date,
                        CustomId = b.CustomId,
                        ExternalBreakRef = b.ExternalBreakRef
                    }))
                .ToList();
        }

        public List<Break> GetBreaks(List<string> salesareanames, DateTime fromdate, DateTime todate) =>
            _mapper.Map<List<Break>>(_dbContext.Query<ScheduleEntity>()
                    .Where(x => x.Date >= fromdate.Date && x.Date < todate.Date.AddDays(1) &&
                                salesareanames.Contains(x.SalesArea.Name))
                    .SelectMany(x => x.Breaks)
                    .Where(x => x.ScheduledDate >= fromdate && x.ScheduledDate <= todate).AsNoTracking(),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public List<Programme> GetProgrammes(List<string> salesareanames, DateTime fromdate, DateTime todate)
        {
            return _mapper.Map<List<Programme>>(
                _dbContext.Query<Entities.Tenant.Schedules.Schedule>()
                    .Where(s => s.Date >= fromdate.Date && s.Date < todate.Date.AddDays(1) &&
                                salesareanames.Contains(s.SalesArea.Name))
                    .SelectMany(x => x.Programmes)
                    .Select(x => x.Programme)
                    .Include(x => x.ProgrammeDictionary)
                    .ThenInclude(x => x.ProgrammeEpisodes).ThenInclude(x => x.ProgrammeDictionary)
                    .Include(x => x.ProgrammeCategoryLinks).ThenInclude(x => x.ProgrammeCategory),
                opts => opts.UseEntityCache(_salesAreaByIdCache));
        }

        public List<Tuple<Break, Programme>> GetBreakWithProgramme(List<string> salesAreaNames, DateTime fromDate, DateTime toDate)
        {
            var schedules = _mapper.Map<List<Schedule>>(GetScheduleQueryWithAllIncludes()
                .Where(s => s.Date >= fromDate.Date && s.Date < toDate.Date.AddDays(1) &&
                            salesAreaNames.Contains(s.SalesArea.Name)), opts => opts.UseEntityCache(_salesAreaByIdCache));

            if (!schedules.Any())
            {
                return null;
            }

            var res = new List<Tuple<Break, Programme>>();
            foreach (Schedule schedule in schedules)
            {
                var programmes = schedule.Programmes?
                    .Where(p => p.StartDateTime >= fromDate && p.StartDateTime <= toDate)
                    .OrderBy(d => d.StartDateTime)
                    .ToList();

                if (!(programmes?.Any() ?? false))
                {
                    continue;
                }

                var programmeFirst = programmes.First();
                var programmeLast = programmes.OrderBy(p => p.StartDateTime.Add(p.Duration.ToTimeSpan())).Last();
                var breaks = schedules.Where(s =>
                        s.SalesArea == schedule.SalesArea && s.Breaks != null && s.Breaks.Any() &&
                        s.Date >= schedule.Date && s.Date <= schedule.Date.AddDays(1))
                    .SelectMany(s => s.Breaks)
                    .Where(b => b.ScheduledDate >= fromDate && b.ScheduledDate <= toDate &&
                        b.ScheduledDate >= programmeFirst.StartDateTime &&
                        b.ScheduledDate.Add(b.Duration.ToTimeSpan()) <= programmeLast.StartDateTime.Add(programmeLast.Duration.ToTimeSpan()))
                    .ToList();
                foreach (var pgm in programmes)
                {
                    res.AddRange(breaks
                        .Where(b =>
                            pgm.StartDateTime <= b.ScheduledDate &&
                            pgm.StartDateTime.AddTicks(pgm.Duration.BclCompatibleTicks) >= b.ScheduledDate)
                        .DefaultIfEmpty()
                        .Select(x => Tuple.Create(x, pgm)));
                }
            }

            return res;
        }

        public List<Schedule> FindByBreakIds(IEnumerable<Guid> breakIds)
        {
            var scheduleIds = breakIds
                .Select((x, i) => new { Item = x, Index = i })
                .GroupBy(x => x.Index / 10000, x => x.Item)
                .SelectMany(x =>
                    _dbContext.Query<ScheduleBreak>()
                        .Where(b => x.Contains(b.Id) && b.ScheduleId != null).Select(b => b.ScheduleId).Distinct().ToList())
                .Distinct().ToList();

            return _mapper.Map<List<Schedule>>(GetScheduleQueryWithAllIncludes()
                .Where(s => scheduleIds.Contains(s.Id)), opts => opts.UseEntityCache(_salesAreaByIdCache));
        }

        public void Truncate() => _dbContext.Truncate<ScheduleEntity>();

        public List<BreakWithProgramme> GetBreakModels(List<string> salesAreaNames, DateTime fromDate, DateTime toDate, string excludeBreakType)
        {
            var schedules = _mapper.Map<List<Schedule>>(GetScheduleQueryWithAllIncludes()
                .Where(s => s.Date >= fromDate.Date && s.Date <= toDate.Date.AddDays(1) &&
                    salesAreaNames.Contains(s.SalesArea.Name)), opts => opts.UseEntityCache(_salesAreaByIdCache))
                .Where(s => s.Breaks?.Any() ?? false)
                .ToList();

            if (!schedules.Any())
            {
                return null;
            }

            var res = new List<BreakWithProgramme>();
            foreach (var schedule in schedules)
            {
                var programmes = schedule.Programmes?.OrderBy(d => d.StartDateTime).ToList();
                if (programmes is null || !programmes.Any())
                {
                    continue;
                }

                var programmeFirst = programmes.First();
                var programmeLast = programmes.OrderBy(p => p.StartDateTime.Add(p.Duration.ToTimeSpan())).Last();
                var breaks = schedules.Where(s =>
                        s.SalesArea == schedule.SalesArea && s.Date >= schedule.Date &&
                        s.Date <= schedule.Date.AddDays(1))
                    .SelectMany(s => s.Breaks)
                    .Where(b => b.BreakType != excludeBreakType && b.ScheduledDate >= fromDate && b.ScheduledDate <= toDate &&
                        b.ScheduledDate >= programmeFirst.StartDateTime &&
                        b.ScheduledDate.Add(b.Duration.ToTimeSpan()) <= programmeLast.StartDateTime.Add(programmeLast.Duration.ToTimeSpan()));
                foreach (var brk in breaks)
                {
                    var pgm = programmes.FirstOrDefault(p =>
                        p.StartDateTime <= brk.ScheduledDate &&
                        p.StartDateTime.Add(p.Duration.ToTimeSpan()) >= brk.ScheduledDate);
                    res.Add(new BreakWithProgramme
                    {
                        Break = brk,
                        //For now default to SPORTS programme category if null
                        ProgrammeCategories = (pgm?.ProgrammeCategories.Any() ?? false) ? pgm.ProgrammeCategories : new List<string>() { "SPORTS" },
                        ProgrammeExternalreference = pgm?.ExternalReference,
                        EpisodeNumber = pgm?.Episode?.Number,
                        PrgtNo = pgm?.PrgtNo ?? 0
                    });
                }
            }

            return res;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
            _scheduleCache.Clear();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            _scheduleCache.Clear();
        }

        public Task TruncateAsync() => Task.Run(Truncate);
    }
}
