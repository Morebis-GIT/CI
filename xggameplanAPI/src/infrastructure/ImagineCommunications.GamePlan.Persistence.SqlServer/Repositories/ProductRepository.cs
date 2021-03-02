using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Queries;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.PostProcessing.Builders;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Dto;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using xggameplan.core.Extensions;
using xggameplan.Extensions;
using AgencyGroup = ImagineCommunications.GamePlan.Domain.Shared.Products.Objects.AgencyGroup;
using Product = ImagineCommunications.GamePlan.Domain.Shared.Products.Objects.Product;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private const int MaxClauseCount = 2000;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IFullTextSearchConditionBuilder _searchConditionBuilder;
        private readonly IMapper _mapper;
        private readonly IClock _clock;

        private IDictionary<string, Advertiser> _dbAdvertisers;
        private IDictionary<string, Agency> _dbAgencies;
        private IDictionary<int, Person> _dbPersons;
        private IDictionary<int, Entities.Tenant.AgencyGroup> _dbAgencyGroups;

        protected void PrepareDictionaryCache()
        {
            if (_dbAdvertisers is null)
            {
                _dbAdvertisers = _dbContext.Query<Advertiser>().ToDictionary(k => k.ExternalIdentifier, v => v);
            }
            if (_dbAgencies is null)
            {
                _dbAgencies = _dbContext.Query<Agency>().ToDictionary(k => k.ExternalIdentifier, v => v);
            }
            if (_dbAgencyGroups is null)
            {
                _dbAgencyGroups = _dbContext.Query<Entities.Tenant.AgencyGroup>()
                    .ToDictionary(k => HashCode.Combine(k.ShortName, k.Code), v => v);
            }
            if (_dbPersons is null)
            {
                _dbPersons = _dbContext.Query<Person>().ToDictionary(k => k.ExternalIdentifier, v => v);
            }
        }

        protected Entities.Tenant.Products.Product CreateProductEntity(Product productModel)
        {
            var product = new Entities.Tenant.Products.Product();
            UpdateProductEntity(product, productModel);

            return product;
        }

        protected void UpdateProductEntity(Entities.Tenant.Products.Product productEntity, Product productModel)
        {
            PrepareDictionaryCache();
            _mapper.Map(productModel, productEntity);

            var productAdvertiser = productEntity.ProductAdvertisers.FirstOrDefault();
            if (productEntity.ProductAdvertisers.Count == 1 &&
                productAdvertiser.Advertiser.ExternalIdentifier == productModel.AdvertiserIdentifier &&
                productAdvertiser.Advertiser.Name == productModel.AdvertiserName)
            {
                productAdvertiser.StartDate = productModel.AdvertiserLinkStartDate;
                productAdvertiser.EndDate = productModel.AdvertiserLinkEndDate;
            }
            else
            {
                _dbContext.RemoveRange(productEntity.ProductAdvertisers.ToArray());
                productEntity.ProductAdvertisers.Clear();

                if (!string.IsNullOrWhiteSpace(productModel.AdvertiserIdentifier))
                {
                    if (_dbAdvertisers.TryGetValue(productModel.AdvertiserIdentifier, out var dbAdv))
                    {
                        if (!string.Equals(dbAdv.Name, productModel.AdvertiserName))
                        {
                            dbAdv.Name = productModel.AdvertiserName;
                        }
                    }
                    else
                    {
                        dbAdv = new Advertiser
                        {
                            ExternalIdentifier = productModel.AdvertiserIdentifier,
                            Name = productModel.AdvertiserName,
                            ShortName = productModel.AdvertiserName
                        };
                        _dbContext.Add(dbAdv);
                        _dbAdvertisers.Add(dbAdv.ExternalIdentifier, dbAdv);
                    }

                    productAdvertiser = new ProductAdvertiser
                    {
                        Product = productEntity,
                        Advertiser = dbAdv,
                        StartDate = productModel.AdvertiserLinkStartDate,
                        EndDate = productModel.AdvertiserLinkEndDate
                    };
                    _dbContext.Add(productAdvertiser);
                    productEntity.ProductAdvertisers.Add(productAdvertiser);
                }
            }

            var productAgency = productEntity.ProductAgencies.FirstOrDefault();
            if (productEntity.ProductAgencies.Count == 1 &&
                  productAgency.Agency.ExternalIdentifier == productModel.AgencyIdentifier &&
                  productAgency.Agency.Name == productModel.AgencyName)
            {
                productAgency.StartDate = productModel.AgencyStartDate;
                productAgency.EndDate = productModel.AgencyLinkEndDate;
            }
            else
            {
                _dbContext.RemoveRange(productEntity.ProductAgencies.ToArray());
                productEntity.ProductAgencies.Clear();

                if (!string.IsNullOrWhiteSpace(productModel.AgencyIdentifier))
                {
                    Entities.Tenant.AgencyGroup dbAgencyGroup = null;
                    if (productModel.AgencyGroup != null)
                    {
                        var key = HashCode.Combine(productModel.AgencyGroup.ShortName, productModel.AgencyGroup.Code);
                        if (!_dbAgencyGroups.TryGetValue(key, out dbAgencyGroup))
                        {
                            dbAgencyGroup = _mapper.Map<Entities.Tenant.AgencyGroup>(productModel.AgencyGroup);
                            _dbContext.Add(dbAgencyGroup);
                            _dbAgencyGroups.Add(key, dbAgencyGroup);
                        }
                    }

                    if (_dbAgencies.TryGetValue(productModel.AgencyIdentifier, out var dbAgency))
                    {
                        if (!string.Equals(dbAgency.Name, productModel.AgencyName))
                        {
                            dbAgency.Name = productModel.AgencyName;
                        }
                    }
                    else
                    {
                        dbAgency = new Agency
                        {
                            ExternalIdentifier = productModel.AgencyIdentifier,
                            Name = productModel.AgencyName,
                            ShortName = productModel.AgencyName
                        };
                        _dbContext.Add(dbAgency);
                        _dbAgencies.Add(dbAgency.ExternalIdentifier, dbAgency);
                    }

                    productAgency = new ProductAgency
                    {
                        Product = productEntity,
                        Agency = dbAgency,
                        AgencyGroup = dbAgencyGroup,
                        StartDate = productModel.AgencyStartDate,
                        EndDate = productModel.AgencyLinkEndDate
                    };
                    _dbContext.Add(productAgency);
                    productEntity.ProductAgencies.Add(productAgency);
                }
            }

            var productPerson = productEntity.ProductPersons.FirstOrDefault();
            if (productEntity.ProductPersons.Count == 1 &&
                productPerson.Person.ExternalIdentifier == productModel.SalesExecutive?.Identifier &&
                productPerson.Person.Name == productModel.SalesExecutive?.Name)
            {
                productPerson.StartDate = productModel.SalesExecutive.StartDate;
                productPerson.EndDate = productModel.SalesExecutive.EndDate;
            }
            else
            {
                _dbContext.RemoveRange(productEntity.ProductPersons.ToArray());
                productEntity.ProductPersons.Clear();

                if (!(productModel.SalesExecutive is null))
                {
                    if (_dbPersons.TryGetValue(productModel.SalesExecutive.Identifier, out var dbPerson))
                    {
                        if (!string.Equals(dbPerson.Name, productModel.SalesExecutive.Name))
                        {
                            dbPerson.Name = productModel.SalesExecutive.Name;
                        }
                    }
                    else
                    {
                        dbPerson = _mapper.Map<Person>(productModel.SalesExecutive);
                        _dbContext.Add(dbPerson);
                        _dbPersons.Add(dbPerson.ExternalIdentifier, dbPerson);
                    }

                    productPerson = new ProductPerson
                    {
                        Product = productEntity,
                        Person = dbPerson,
                        StartDate = productModel.SalesExecutive.StartDate,
                        EndDate = productModel.SalesExecutive.EndDate
                    };
                    _dbContext.Add(productPerson);
                    productEntity.ProductPersons.Add(productPerson);
                }
            }
        }

        protected IQueryable<Entities.Tenant.Products.Product> ProductQuery =>
            _dbContext.Query<Entities.Tenant.Products.Product>()
            .Include(x => x.ProductAdvertisers).ThenInclude(x => x.Advertiser)
            .Include(x => x.ProductAgencies).ThenInclude(x => x.Agency)
            .Include(x => x.ProductAgencies).ThenInclude(x => x.AgencyGroup)
            .Include(x => x.ProductPersons).ThenInclude(x => x.Person);

        protected virtual IQueryable<ProductDto> ActualProductQuery(DateTime onDate, Expression<Func<Entities.Tenant.Products.Product, bool>> productExpressionPredicate = null)
        {
            var query =
                from product in _dbContext.Query<Entities.Tenant.Products.Product>()
                join productAdvertiserJoin in _dbContext.Query<ProductAdvertiser>() on product.Uid equals productAdvertiserJoin.ProductId
                    into paJoin
                from productAdvertiser in paJoin.DefaultIfEmpty()
                join advertiserJoin in _dbContext.Query<Advertiser>() on productAdvertiser.AdvertiserId equals advertiserJoin.Id into aJoin
                from advertiser in aJoin.DefaultIfEmpty()
                join productAgencyJoin in _dbContext.Query<ProductAgency>() on product.Uid equals productAgencyJoin.ProductId
                    into pagJoin
                from productAgency in pagJoin.DefaultIfEmpty()
                join agencyJoin in _dbContext.Query<Agency>() on productAgency.AgencyId equals agencyJoin.Id into agJoin
                from agency in agJoin.DefaultIfEmpty()
                join agencyGroupJoin in _dbContext.Query<Entities.Tenant.AgencyGroup>() on productAgency.AgencyGroupId equals agencyGroupJoin.Id
                    into aggJoin
                from agencyGroup in aggJoin.DefaultIfEmpty()
                join productPersonJoin in _dbContext.Query<ProductPerson>() on product.Uid equals productPersonJoin.ProductId
                    into pseJoin
                from productPerson in pseJoin.DefaultIfEmpty()
                join personJoin in _dbContext.Query<Person>() on productPerson.PersonId equals personJoin.Id into seJoin
                from person in seJoin.DefaultIfEmpty()
                select new
                {
                    product,
                    productAdvertiser,
                    productAgency,
                    productPerson,
                    advertiser,
                    agency,
                    agencyGroup,
                    person      //Person (SalesExecutive)
                };
            if (!(productExpressionPredicate is null))
            {
                query = ExpressProductPredicate(query, productExpressionPredicate);
            }
            return query.Select(x => new ProductDto
            {
                Uid = x.product.Uid,
                Name = x.product.Name,
                Externalidentifier = x.product.Externalidentifier,
                ParentExternalidentifier = x.product.ParentExternalidentifier,
                ClashCode = x.product.ClashCode,
                EffectiveStartDate = x.product.EffectiveStartDate,
                EffectiveEndDate = x.product.EffectiveEndDate,
                ReportingCategory = x.product.ReportingCategory,
                AdvertiserId = x.advertiser.Id,
                AdvertiserIdentifier = x.advertiser.ExternalIdentifier,
                AdvertiserName = x.advertiser.Name,
                AdvertiserShortName = x.advertiser.ShortName,
                AdvertiserStartDate = x.productAdvertiser.StartDate,
                AdvertiserEndDate = x.productAdvertiser.EndDate,
                AgencyId = x.agency.Id,
                AgencyIdentifier = x.agency.ExternalIdentifier,
                AgencyName = x.agency.Name,
                AgencyShortName = x.agency.ShortName,
                AgencyStartDate = x.productAgency.StartDate,
                AgencyEndDate = x.productAgency.EndDate,
                AgencyGroupId = x.agencyGroup.Id,
                AgencyGroupShortName = x.agencyGroup.ShortName,
                AgencyGroupCode = x.agencyGroup.Code,
                PersonId = x.person.Id,
                PersonIdentifier = x.person.ExternalIdentifier,
                PersonName = x.person.Name,
                PersonStartDate = x.productPerson.StartDate,
                PersonEndDate = x.productPerson.EndDate
            });
        }

        protected IQueryable<T> ExpressProductPredicate<T>(IQueryable<T> q, Expression<Func<Entities.Tenant.Products.Product, bool>> expression)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var productAccess = Expression.MakeMemberAccess(param, param.Type.GetMember("product").First());
            return q.Where(Expression.Lambda<Func<T, bool>>(new ActualProductQueryParameterVisitor(productAccess).Visit(expression.Body), param));
        }

        public ProductRepository(
            ISqlServerTenantDbContext dbContext,
            IFullTextSearchConditionBuilder searchConditionBuilder,
            IMapper mapper,
            IClock clock)
        {
            _dbContext = dbContext;
            _searchConditionBuilder = searchConditionBuilder;
            _mapper = mapper;
            _clock = clock;
        }

        public int CountAll => _dbContext.Query<Entities.Tenant.Products.Product>().Count();

        public void Add(Product item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var entity = ProductQuery.FirstOrDefault(x => x.Uid == item.Uid);

            if (entity == null)
            {
                _dbContext.Add(CreateProductEntity(item), post => post.MapTo(item), _mapper);
            }
            else
            {
                UpdateProductEntity(entity, item);
                _dbContext.Update(entity, post => post.MapTo(item), _mapper);
            }
        }

        /// <summary>
        /// BulkInsert method, only for insert. It doesn't need SaveChanges for
        /// applying changes.
        /// </summary>
        /// <param name="items"></param>
        public void Add(IEnumerable<Product> items)
        {
            if (!(items?.Any() ?? false))
            {
                return;
            }

            PrepareDictionaryCache();
            var products = new List<Entities.Tenant.Products.Product>();
            var advertisers = new List<Advertiser>();
            var agencies = new List<Agency>();
            var agencyGroups = new List<Entities.Tenant.AgencyGroup>();
            var persons = new List<Person>();
            var productAdvertisers = new List<ProductAdvertiser>();
            var productAgencies = new List<ProductAgency>();
            var productPersons = new List<ProductPerson>();

            foreach (var item in items)
            {
                var product = _mapper.Map<Entities.Tenant.Products.Product>(item);
                products.Add(product);
                if (!string.IsNullOrWhiteSpace(item.AdvertiserIdentifier))
                {
                    if (_dbAdvertisers.TryGetValue(item.AdvertiserIdentifier, out var dbAdv))
                    {
                        if (!string.Equals(dbAdv.Name, item.AdvertiserName))
                        {
                            dbAdv.Name = item.AdvertiserName;
                            advertisers.Add(dbAdv);
                        }
                    }
                    else
                    {
                        dbAdv = new Advertiser
                        {
                            ExternalIdentifier = item.AdvertiserIdentifier,
                            Name = item.AdvertiserName,
                            ShortName = item.AdvertiserName
                        };
                        advertisers.Add(dbAdv);
                        _dbAdvertisers.Add(dbAdv.ExternalIdentifier, dbAdv);
                    }

                    var productAdvertiser = new ProductAdvertiser
                    {
                        Product = product,
                        Advertiser = dbAdv,
                        StartDate = item.AdvertiserLinkStartDate,
                        EndDate = item.AdvertiserLinkEndDate
                    };
                    productAdvertisers.Add(productAdvertiser);
                    product.ProductAdvertisers.Add(productAdvertiser);
                }

                if (!string.IsNullOrWhiteSpace(item.AgencyIdentifier))
                {
                    Entities.Tenant.AgencyGroup dbAgencyGroup = null;
                    if (item.AgencyGroup != null)
                    {
                        var key = HashCode.Combine(item.AgencyGroup.ShortName, item.AgencyGroup.Code);
                        if (!_dbAgencyGroups.TryGetValue(key, out dbAgencyGroup))
                        {
                            dbAgencyGroup = _mapper.Map<Entities.Tenant.AgencyGroup>(item.AgencyGroup);
                            agencyGroups.Add(dbAgencyGroup);
                            _dbAgencyGroups.Add(key, dbAgencyGroup);
                        }
                    }

                    if (_dbAgencies.TryGetValue(item.AgencyIdentifier, out var dbAgency))
                    {
                        if (!string.Equals(dbAgency.Name, item.AgencyName))
                        {
                            dbAgency.Name = item.AgencyName;
                            agencies.Add(dbAgency);
                        }
                    }
                    else
                    {
                        dbAgency = new Agency
                        {
                            ExternalIdentifier = item.AgencyIdentifier,
                            Name = item.AgencyName,
                            ShortName = item.AgencyName
                        };
                        agencies.Add(dbAgency);
                        _dbAgencies.Add(dbAgency.ExternalIdentifier, dbAgency);
                    }

                    var productAgency = new ProductAgency
                    {
                        Product = product,
                        Agency = dbAgency,
                        AgencyGroup = dbAgencyGroup,
                        StartDate = item.AgencyStartDate,
                        EndDate = item.AgencyLinkEndDate
                    };
                    productAgencies.Add(productAgency);
                    product.ProductAgencies.Add(productAgency);
                }

                if (!(item.SalesExecutive is null))
                {
                    if (_dbPersons.TryGetValue(item.SalesExecutive.Identifier, out var dbPerson))
                    {
                        if (!string.Equals(dbPerson.Name, item.SalesExecutive.Name))
                        {
                            dbPerson.Name = item.SalesExecutive.Name;
                            persons.Add(dbPerson);
                        }
                    }
                    else
                    {
                        dbPerson = _mapper.Map<Person>(item.SalesExecutive);
                        persons.Add(dbPerson);
                        _dbPersons.Add(dbPerson.ExternalIdentifier, dbPerson);
                    }

                    var productPerson = new ProductPerson
                    {
                        Product = product,
                        Person = dbPerson,
                        StartDate = item.SalesExecutive.StartDate,
                        EndDate = item.SalesExecutive.EndDate
                    };
                    productPersons.Add(productPerson);
                    product.ProductPersons.Add(productPerson);
                }
            }

            using var transaction = _dbContext.Specific.Database.BeginTransaction();

            if (advertisers.Count > 0)
            {
                _dbContext.BulkInsertEngine.BulkInsertOrUpdate(advertisers,
                    new BulkInsertOptions { SetOutputIdentity = true, PreserveInsertOrder = true });
            }

            if (agencies.Count > 0)
            {
                _dbContext.BulkInsertEngine.BulkInsertOrUpdate(agencies,
                    new BulkInsertOptions { SetOutputIdentity = true, PreserveInsertOrder = true });
            }

            if (agencyGroups.Count > 0)
            {
                _dbContext.BulkInsertEngine.BulkInsertOrUpdate(agencyGroups,
                    new BulkInsertOptions { SetOutputIdentity = true, PreserveInsertOrder = true });
            }

            if (persons.Count > 0)
            {
                _dbContext.BulkInsertEngine.BulkInsertOrUpdate(persons,
                    new BulkInsertOptions { SetOutputIdentity = true, PreserveInsertOrder = true });
            }

            if (products.Count > 0)
            {
                _dbContext.BulkInsertEngine.BulkInsert(products);
            }

            if (productAdvertisers.Count > 0)
            {
                foreach (var item in productAdvertisers)
                {
                    item.ProductId = item.Product.Uid;
                    item.AdvertiserId = item.Advertiser.Id;
                }

                _dbContext.BulkInsertEngine.BulkInsert(productAdvertisers);
            }

            if (productAgencies.Count > 0)
            {
                foreach (var item in productAgencies)
                {
                    item.ProductId = item.Product.Uid;
                    item.AgencyId = item.Agency.Id;
                    item.AgencyGroupId = item.AgencyGroup?.Id;
                }

                _dbContext.BulkInsertEngine.BulkInsert(productAgencies);
            }

            if (productPersons.Count > 0)
            {
                foreach (var item in productPersons)
                {
                    item.ProductId = item.Product.Uid;
                    item.PersonId = item.Person.Id;
                }

                _dbContext.BulkInsertEngine.BulkInsert(productPersons);
            }

            transaction.Commit();

            var actionBuilder = new BulkInsertActionBuilder<Entities.Tenant.Products.Product>(products, _mapper);
            actionBuilder.TryToUpdate(items);
            actionBuilder.Build()?.Execute();
        }

        public int Count(Expression<Func<Product, bool>> query) =>
            _dbContext.Query<Entities.Tenant.Products.Product>().ProjectTo<Product>(_mapper.ConfigurationProvider)
                .Count(query);

        public void Delete(Guid uid)
        {
            var entity = _dbContext.Find<Entities.Tenant.Products.Product>(uid);
            if (entity != null)
            {
                _dbContext.Remove(entity);
            }
        }

        public void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs)
        {
            var dataCount = externalRefs.Count();
            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                for (int i = 0; i <= dataCount / 1000; i++)
                {
                    var ids = externalRefs.Skip(i * 1000).Take(1000);
                    var prodcutIds = _dbContext.Query<Entities.Tenant.Products.Product>()
                        .Where(x => ids.Contains(x.Externalidentifier)).Select(r => r.Uid)
                        .ToArray();

                    _dbContext.Specific.RemoveByUniqueIdentifierIds<Entities.Tenant.Products.Product>(prodcutIds);
                }

                transaction.Commit();
            }
        }

        public bool Exists(Expression<Func<Product, bool>> condition) =>
            _dbContext.Query<Entities.Tenant.Products.Product>().ProjectTo<Product>(_mapper.ConfigurationProvider).Any(condition);

        public IEnumerable<Product> FindByExternal(string externalRef, DateTime onDate) =>
            ActualProductQuery(onDate).Where(x => x.Externalidentifier == externalRef)
                .ProjectTo<Product>(_mapper.ConfigurationProvider).ToList();

        public IEnumerable<Product> FindByExternal(List<string> externalRefs, DateTime onDate)
        {
            var products = new List<Product>();
            externalRefs = externalRefs.Distinct().ToList();

            for (int i = 0, page = 0; i < externalRefs.Count; i += MaxClauseCount, page++)
            {
                var refs = externalRefs.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                products.AddRange(ActualProductQuery(onDate)
                    .Where(x => refs.Contains(x.Externalidentifier)).ProjectTo<Product>(_mapper.ConfigurationProvider)
                    .ToArray());
            }

            return products;
        }

        public Product Find(Guid uid) => Get(uid);

        public IEnumerable<Product> FindByAdvertiserId(List<string> advertiserIds, DateTime onDate) =>
            ActualProductQuery(onDate)
                .Where(x => advertiserIds.Contains(x.AdvertiserIdentifier))
                .ProjectTo<Product>(_mapper.ConfigurationProvider).ToList();

        public IEnumerable<Product> FindByAdvertiserId(List<string> advertiserIds) =>
            FindByAdvertiserId(advertiserIds, _clock.GetCurrentInstant().ToDateTimeUtc());

        public IEnumerable<Product> FindByExternal(string externalRef) =>
            FindByExternal(externalRef, _clock.GetCurrentInstant().ToDateTimeUtc());

        public IEnumerable<Product> FindByExternal(List<string> externalRefs) =>
            FindByExternal(externalRefs, _clock.GetCurrentInstant().ToDateTimeUtc());

        public Product Get(Guid uid) =>
            Get(uid, _clock.GetCurrentInstant().ToDateTimeUtc());

        public Product Get(Guid uid, DateTime onDate) =>
            ActualProductQuery(onDate).ProjectTo<Product>(_mapper.ConfigurationProvider).FirstOrDefault(x => x.Uid == uid);

        public IEnumerable<Product> GetAll(DateTime onDate) =>
            ActualProductQuery(onDate).ProjectTo<Product>(_mapper.ConfigurationProvider).ToList();

        public IEnumerable<Product> GetAll() =>
            GetAll(_clock.GetCurrentInstant().ToDateTimeUtc());

        public IEnumerable<ProductAdvertiserModel> GetAdvertisers(ICollection<string> advertiserIds)
        {
            var distinctAdvertiserIds = advertiserIds.Distinct().ToList();
            var result = new List<ProductAdvertiserModel>();
            for (int i = 0, page = 0; i < distinctAdvertiserIds.Count; i += MaxClauseCount, page++)
            {
                var ids = distinctAdvertiserIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                result.AddRange(_dbContext.Query<Entities.Tenant.Advertiser>()
                    .Where(a => ids.Contains(a.ExternalIdentifier)).ProjectTo<ProductAdvertiserModel>(_mapper.ConfigurationProvider).ToArray());
            }

            return result;
        }

        public IEnumerable<ProductAgencyModel> GetAgencies(ICollection<string> agencyIds)
        {
            var distinctAgencyIds = agencyIds.Distinct().ToList();
            var result = new List<ProductAgencyModel>();
            for (int i = 0, page = 0; i < distinctAgencyIds.Count; i += MaxClauseCount, page++)
            {
                var ids = distinctAgencyIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                result.AddRange(_dbContext.Query<Entities.Tenant.Agency>()
                    .Where(a => ids.Contains(a.ExternalIdentifier)).ProjectTo<ProductAgencyModel>(_mapper.ConfigurationProvider).ToArray());
            }

            return result;
        }

        public IEnumerable<AgencyGroup> GetAgencyGroups(ICollection<string> agencyGroupIds)
        {
            var distinctAgencyGroupIds = agencyGroupIds.Distinct().ToList();
            var result = new List<AgencyGroup>();
            for (int i = 0, page = 0; i < distinctAgencyGroupIds.Count; i += MaxClauseCount, page++)
            {
                var codes = distinctAgencyGroupIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                result.AddRange(_dbContext.Query<Entities.Tenant.AgencyGroup>()
                    .Where(a => codes.Contains(a.Code)).ProjectTo<AgencyGroup>(_mapper.ConfigurationProvider).ToArray());
            }

            return result;
        }

        public IEnumerable<ProductNameModel> GetByExternalIds(ICollection<string> externalIds)
        {
            var distinctExternalIds = externalIds.Distinct().ToList();
            var result = new List<ProductNameModel>();
            for (int i = 0, page = 0; i < distinctExternalIds.Count; i += MaxClauseCount, page++)
            {
                var refs = distinctExternalIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                result.AddRange(_dbContext.Query<Entities.Tenant.Products.Product>()
                    .Where(x => refs.Contains(x.Externalidentifier)).ProjectTo<ProductNameModel>(_mapper.ConfigurationProvider).ToArray());
            }

            return result;
        }

        public IEnumerable<string> GetReportingCategories() =>
            _dbContext.Query<Entities.Tenant.Products.Product>()
                .Where(x => !string.IsNullOrEmpty(x.ReportingCategory))
                .Select(x => x.ReportingCategory)
                .Distinct()
                .ToList();

        public IEnumerable<SalesExecutiveModel> GetSalesExecutives(ICollection<int> salesExecIds)
        {
            var distinctSalesExecIds = salesExecIds.Distinct().ToList();
            var result = new List<SalesExecutiveModel>();
            for (int i = 0, page = 0; i < distinctSalesExecIds.Count; i += MaxClauseCount, page++)
            {
                var ids = distinctSalesExecIds.Skip(MaxClauseCount * page).Take(MaxClauseCount).ToArray();
                result.AddRange(_dbContext.Query<Entities.Tenant.Person>()
                    .Where(p => ids.Contains(p.ExternalIdentifier)).ProjectTo<SalesExecutiveModel>(_mapper.ConfigurationProvider).ToArray());
            }

            return result;
        }

        public void Remove(Guid uid) => Delete(uid);

        public void SaveChanges() => _dbContext.SaveChanges();

        public PagedQueryResult<ProductNameModel> Search(ProductSearchQueryModel searchQuery, DateTime onDate)
        {
            var where = new List<Expression<Func<Entities.Tenant.Products.Product, bool>>>();

            if (!string.IsNullOrWhiteSpace(searchQuery.Externalidentifier))
            {
                where.Add(p => p.Externalidentifier == searchQuery.Externalidentifier);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.Name))
            {
                where.Add(p => p.Name == searchQuery.Name);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.ClashCode))
            {
                where.Add(p => p.ClashCode == searchQuery.ClashCode);
            }

            if (searchQuery.FromDateInclusive != default)
            {
                where.Add(p => p.EffectiveStartDate >= searchQuery.FromDateInclusive);
            }

            if (searchQuery.ToDateInclusive != default)
            {
                where.Add(p => p.EffectiveEndDate < searchQuery.ToDateInclusive.AddTicks(1));
            }

            if (!string.IsNullOrWhiteSpace(searchQuery.NameOrRef))
            {
                var query = _searchConditionBuilder.StartAllWith(
                    searchQuery.NameOrRef.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)).Build();
                where.Add(p => EF.Functions.Contains(EF.Property<string>(p, Entities.Tenant.Products.Product.SearchFieldName), query));
            }

            string sortBy;
            switch (searchQuery.OrderBy)
            {
                case ProductOrder.StartDate:
                    sortBy = "EffectiveStartDate";
                    break;

                case ProductOrder.EndDate:
                    sortBy = "EffectiveEndDate";
                    break;

                case null:
                    sortBy = null;
                    break;

                default:
                    sortBy = searchQuery.OrderBy.ToString();
                    break;
            }

            var items = !where.Any()
                ? ActualProductQuery(onDate)
                : ActualProductQuery(onDate, where.AggregateAnd());

            var sortedItems = items.OrderBySingleItem(sortBy,
                    searchQuery.OrderByDirection ?? Domain.Generic.OrderDirection.Asc)
                .ApplyPaging(searchQuery.Skip, searchQuery.Top);

            var totalCount = searchQuery.IncludeTotalCount ? items.Count() : 0;

            return new PagedQueryResult<ProductNameModel>(totalCount, sortedItems.ProjectTo<ProductNameModel>(_mapper.ConfigurationProvider).ToList());
        }

        public PagedQueryResult<ProductAdvertiserModel> Search(AdvertiserSearchQueryModel searchQuery, DateTime onDate)
        {
            if (searchQuery == null)
            {
                throw new ArgumentNullException(nameof(searchQuery));
            }

            var query = string.Empty;
            if (!string.IsNullOrWhiteSpace(searchQuery.AdvertiserNameorRef))
            {
                query = _searchConditionBuilder.StartAllWith(
                    searchQuery.AdvertiserNameorRef.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)).Build();
            }

            var dbQuery =
                (from p in _dbContext.Query<Entities.Tenant.Products.Product>()
                 join pa in _dbContext.Query<ProductAdvertiser>() on p.Uid equals pa.ProductId
                 join a in _dbContext.Query<Advertiser>() on pa.AdvertiserId equals a.Id
                 where pa.StartDate <= onDate && pa.EndDate > onDate
                 select a).AsQueryable();

            var items = (string.IsNullOrWhiteSpace(query)
                    ? dbQuery
                    : dbQuery
                        .Where(a => EF.Functions.Contains(
                            EF.Property<string>(a, Entities.Tenant.Advertiser.SearchFieldName), query)))
                .Distinct().OrderBy(x => x.ExternalIdentifier)
                .ProjectTo<ProductAdvertiserModel>(_mapper.ConfigurationProvider);

            var totalCount = searchQuery.IncludeTotalCount ? items.Count() : 0;

            return new PagedQueryResult<ProductAdvertiserModel>(totalCount,
                items.ApplyPaging(searchQuery.Skip, searchQuery.Top).ToList());
        }

        public void Truncate() =>
            _dbContext.Truncate<Entities.Tenant.Products.Product>();

        public Task TruncateAsync() => Task.Run(Truncate);

        public void Update(Product product)
        {
            if (product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            var entity = ProductQuery.FirstOrDefault(x => x.Uid == product.Uid);

            if (entity != null)
            {
                UpdateProductEntity(entity, product);
                _dbContext.Update(entity, post => post.MapTo(product), _mapper);
            }
        }
    }

    internal class ActualProductQueryParameterVisitor : ExpressionVisitor
    {
        private readonly MemberExpression _replaceExpression;

        public ActualProductQueryParameterVisitor(MemberExpression replaceExpression)
        {
            _replaceExpression = replaceExpression;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node.Type == _replaceExpression.Type ? (Expression)_replaceExpression : node;
        }
    }
}
