using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Queries;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Caches;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Builders;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Interfaces;
using Microsoft.EntityFrameworkCore;
using xggameplan.core.Extensions;
using xggameplan.core.Extensions.AutoMapper;
using Programme = ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects.Programme;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ProgrammeRepository : IProgrammeRepository
    {
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly ISqlServerDbContextFactory<ISqlServerTenantDbContext> _dbContextFactory;
        private readonly IFullTextSearchConditionBuilder _searchConditionBuilder;
        private readonly ISqlServerSalesAreaByIdCacheAccessor _salesAreaByIdCache;
        private readonly ISqlServerSalesAreaByNameCacheAccessor _salesAreaByNameCache;
        private readonly IMapper _mapper;
        private readonly ProgrammeDictionaryCache _programmeDictionaryCache;
        private readonly ProgrammeCategoryCache _programmeCategoryCache;

        public ProgrammeRepository(
            ISqlServerTenantDbContext dbContext,
            ISqlServerDbContextFactory<ISqlServerTenantDbContext> dbContextFactory,
            IFullTextSearchConditionBuilder searchConditionBuilder,
            ISqlServerSalesAreaByIdCacheAccessor salesAreaByIdCache,
            ISqlServerSalesAreaByNameCacheAccessor salesAreaByNameCache,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _dbContextFactory = dbContextFactory;
            _searchConditionBuilder = searchConditionBuilder;
            _salesAreaByIdCache = salesAreaByIdCache;
            _salesAreaByNameCache = salesAreaByNameCache;
            _mapper = mapper;
            _programmeDictionaryCache =
                new ProgrammeDictionaryCache(_dbContext, preloadData: false);
            _programmeCategoryCache = new ProgrammeCategoryCache(_dbContext);
        }

        public Programme Find(Guid id) =>
            Get(id);

        public Programme Get(Guid id) =>
            _mapper.Map<Programme>(ProgrammeQuery.FirstOrDefault(x => x.Id == id),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Programme> GetAll() =>
            _mapper.Map<List<Programme>>(ProgrammeQuery, opts => opts.UseEntityCache(_salesAreaByIdCache));

        public int CountAll =>
            _dbContext.Query<Entities.Tenant.Programmes.Programme>().Count();

        public int Count(Expression<Func<Programme, bool>> query) =>
            _dbContext.Query<Entities.Tenant.Programmes.Programme>()
                .ProjectTo<Programme>(_mapper.ConfigurationProvider)
                .Count(query);

        public IEnumerable<Programme> Search(DateTime dateFrom, DateTime dateTo, string salesArea) =>
            _mapper.Map<List<Programme>>(ProgrammeQuery.Where(x => x.SalesArea.Name == salesArea
                                                                   && x.StartDateTime >= dateFrom
                                                                   && x.StartDateTime <= dateTo),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public PagedQueryResult<ProgrammeNameModel> Search(ProgrammeSearchQueryModel searchQuery)
        {
            if (searchQuery == null)
            {
                throw new ArgumentNullException(nameof(searchQuery));
            }

            var query = _dbContext.Query<Entities.Tenant.Programmes.Programme>();




            if (searchQuery.SalesArea != null && searchQuery.SalesArea.Any())
            {
                query = query.Where(e => searchQuery.SalesArea.Contains(e.SalesArea.Name));
            }

            if (searchQuery.FromDateInclusive != default)
            {
                query = query.Where(p => p.StartDateTime >= searchQuery.FromDateInclusive);
            }

            if (searchQuery.ToDateInclusive != default)
            {
                query = query.Where(p => p.StartDateTime <= searchQuery.ToDateInclusive);
            }

            if (!String.IsNullOrWhiteSpace(searchQuery.NameOrRef))
            {
                //this piece of code will be replaced with the Match functionality
               query = query
                    .Where(p =>
                            p.ProgrammeDictionary.Name.Contains(searchQuery.NameOrRef)
                        || p.ProgrammeDictionary.ExternalReference.Contains(searchQuery.NameOrRef));
            }

            if (searchQuery.OrderBy != null)
            {
                var sortBy = searchQuery.OrderBy == ProgrammeOrder.LocalDate
                    ? nameof(Programme.StartDateTime)
                    : searchQuery.OrderBy.ToString();

                query = query.OrderBySingleItem(sortBy,
                    searchQuery.OrderByDirection ?? OrderDirection.Asc);
            }

            var pQuery = query.Select(x => new ProgrammeNameModel
            {
                ExternalReference = x.ProgrammeDictionary.ExternalReference,
                ProgrammeName = x.ProgrammeDictionary.Name
            }).Distinct();

            return new PagedQueryResult<ProgrammeNameModel>(pQuery.Count(),
                pQuery.ApplyPaging(searchQuery.Skip, searchQuery.Top).ToList());
        }

        public IEnumerable<Programme> FindByExternal(List<string> externalRefs) =>
            _mapper.Map<List<Programme>>(ProgrammeQuery
                    .Where(e => externalRefs.Contains(e.ProgrammeDictionary.ExternalReference)),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public IEnumerable<Programme> FindByExternal(string externalRef) =>
            _mapper.Map<List<Programme>>(ProgrammeQuery
                    .Where(x => x.ProgrammeDictionary.ExternalReference == externalRef),
                opts => opts.UseEntityCache(_salesAreaByIdCache));

        public bool Exists(Expression<Func<Programme, bool>> condition) =>
            _dbContext.Query<Entities.Tenant.Programmes.Programme>()
                .ProjectTo<Programme>(_mapper.ConfigurationProvider)
                .Any(condition);

        public void Add(Programme item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var programmeDictionary = _programmeDictionaryCache.Get(item.ExternalReference);

            if (programmeDictionary is null)
            {
                programmeDictionary = _mapper.Map<Entities.Tenant.ProgrammeDictionary>(item);
                _programmeDictionaryCache.Add(programmeDictionary);
            }
            else
            {
                programmeDictionary.Name = item.ProgrammeName;
                programmeDictionary.Description = item.Description;
                programmeDictionary.Classification = item.Classification;
            }

            var programme =
                _mapper.Map<Entities.Tenant.Programmes.Programme>(item,
                    opts => opts.UseEntityCache(_salesAreaByNameCache));

            if (programme.Id == Guid.Empty)
            {
                programme.Id = Guid.NewGuid();
            }

            _ = _dbContext.Add(programme, post => post.MapTo(item, opts => opts.UseEntityCache(_salesAreaByIdCache)),
                _mapper);

            programme.ProgrammeDictionary = programmeDictionary;

            foreach (var cat in item.ProgrammeCategories ?? Enumerable.Empty<string>())
            {
                var category = _programmeCategoryCache.GetOrAdd(cat, key => new ProgrammeCategory { Name = key });

                if (programme.ProgrammeCategoryLinks.All(x => !String.Equals(x.ProgrammeCategory.Name, category.Name)))
                {
                    programme.ProgrammeCategoryLinks.Add(new ProgrammeCategoryLink
                    {
                        ProgrammeCategory = category,
                        Programme = programme
                    });
                }
            }

            if (!(item.Episode is null))
            {
                var episode = programmeDictionary.ProgrammeEpisodes.FirstOrDefault(x => x.Number == item.Episode.Number);

                if (episode is null)
                {
                    episode = new ProgrammeEpisode
                    {
                        Name = item.Episode.Name,
                        Number = item.Episode.Number,
                    };
                    programmeDictionary.ProgrammeEpisodes.Add(episode);
                }
                else
                {
                    episode.Name = item.Episode.Name;
                }

                programme.Episode = episode;
            }
        }

        public void Add(IEnumerable<Programme> item)
        {
            using (var operationDbContext = _dbContextFactory.Create())
            {
                var prgDictCache = new ProgrammeDictionaryCache(operationDbContext, trackingChanges: false);
                var prgCatCache = new ProgrammeCategoryCache(operationDbContext, trackingChanges: false);

                var prgDictToProcess = new HashSet<Entities.Tenant.ProgrammeDictionary>();
                var prgCatToProcess = new List<ProgrammeCategory>();
                var prgToProcess = new List<Entities.Tenant.Programmes.Programme>();
                var prgCatLinkToProcess = new List<ProgrammeCategoryLink>();
                var prgDictEpisodeToProcess = new HashSet<ProgrammeEpisode>();

                foreach (var model in item)
                {
                    var programmeDictionary = prgDictCache.Get(model.ExternalReference);

                    if (programmeDictionary is null)
                    {
                        programmeDictionary = _mapper.Map<Entities.Tenant.ProgrammeDictionary>(model);
                        prgDictCache.Add(programmeDictionary);
                    }
                    else
                    {
                        programmeDictionary.Name = model.ProgrammeName;
                        programmeDictionary.Description = model.Description;
                        programmeDictionary.Classification = model.Classification;
                    }

                    _ = prgDictToProcess.Add(programmeDictionary);

                    var programme = _mapper.Map<Entities.Tenant.Programmes.Programme>(model,
                        opts => opts.UseEntityCache(_salesAreaByNameCache));

                    if (programme.Id == Guid.Empty)
                    {
                        programme.Id = Guid.NewGuid();
                    }

                    programme.ProgrammeDictionary = programmeDictionary;
                    prgToProcess.Add(programme);

                    foreach (var catName in model.ProgrammeCategories ?? Enumerable.Empty<string>())
                    {
                        var category = prgCatCache.GetOrAdd(catName, key =>
                        {
                            var newCat = new ProgrammeCategory { Name = key };
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

                    if (!(model.Episode is null))
                    {
                        var episode = programmeDictionary.ProgrammeEpisodes.FirstOrDefault(x => x.Number == model.Episode.Number);

                        if (episode is null)
                        {
                            episode = new ProgrammeEpisode
                            {
                                Name = model.Episode.Name,
                                Number = model.Episode.Number,
                                ProgrammeDictionary = programmeDictionary
                            };

                            programmeDictionary.ProgrammeEpisodes.Add(episode);
                        }
                        else
                        {
                            episode.Name = model.Episode.Name;
                        }

                        programme.Episode = episode;
                        _ = prgDictEpisodeToProcess.Add(episode);
                    }
                }

                using (var transaction = operationDbContext.Specific.Database.BeginTransaction())
                {
                    operationDbContext.BulkInsertEngine.BulkInsertOrUpdate(prgDictToProcess.ToList(),
                        new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

                    foreach (var episode in prgDictEpisodeToProcess)
                    {
                        episode.ProgrammeDictionaryId = episode.ProgrammeDictionary.Id;
                    }

                    operationDbContext.BulkInsertEngine.BulkInsertOrUpdate(prgDictEpisodeToProcess.ToList(),
                        new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

                    operationDbContext.BulkInsertEngine.BulkInsert(prgCatToProcess,
                        new BulkInsertOptions { PreserveInsertOrder = true, SetOutputIdentity = true });

                    foreach (var prg in prgToProcess)
                    {
                        prg.ProgrammeDictionaryId = prg.ProgrammeDictionary.Id;
                        prg.EpisodeId = prg.Episode?.Id;
                    }

                    operationDbContext.BulkInsertEngine.BulkInsert(prgToProcess,
                        new BulkInsertOptions { PreserveInsertOrder = true });

                    foreach (var catLink in prgCatLinkToProcess)
                    {
                        catLink.ProgrammeCategoryId = catLink.ProgrammeCategory.Id;
                    }

                    operationDbContext.BulkInsertEngine.BulkInsert(prgCatLinkToProcess);

                    transaction.Commit();
                }

                var actionBuilder = new BulkInsertActionBuilder<Entities.Tenant.Programmes.Programme>(prgToProcess, _mapper);
                actionBuilder.TryToUpdate(item, opts => opts.UseEntityCache(_salesAreaByIdCache));
                actionBuilder.Build()?.Execute();
            }
        }

        public void Remove(Guid uid) => Delete(uid);

        public void DeleteRange(IEnumerable<Guid> ids)
        {
            var entities = _dbContext.Query<Entities.Tenant.Programmes.Programme>()
                .Where(e => ids.Contains(e.Id))
                .ToArray();

            if (entities.Any())
            {
                _dbContext.RemoveRange(entities);
            }
        }

        public void Delete(Guid uid)
        {
            var entity = _dbContext.Find<Entities.Tenant.Programmes.Programme>(uid);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void Truncate() =>
            _dbContext.Truncate<Entities.Tenant.Programmes.Programme>();

        public Task TruncateAsync() =>
            Task.Run(Truncate);

        public void SaveChanges() => _dbContext.SaveChanges();

        protected IQueryable<Entities.Tenant.Programmes.Programme> ProgrammeQuery =>
            _dbContext.Query<Entities.Tenant.Programmes.Programme>()
                .Include(x => x.ProgrammeDictionary)
                .Include(x => x.ProgrammeCategoryLinks).ThenInclude(x => x.ProgrammeCategory)
                .Include(x => x.Episode).ThenInclude(x => x.ProgrammeDictionary);
    }
}
