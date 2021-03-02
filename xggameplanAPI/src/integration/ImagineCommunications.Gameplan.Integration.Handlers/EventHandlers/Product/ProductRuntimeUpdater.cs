using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Product;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.BulkInsert;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products;
using Microsoft.EntityFrameworkCore;
using Contract = ImagineCommunications.Gameplan.Integration.Contracts.Models;
using ProductEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products.Product;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Product
{
    public class ProductRuntimeUpdater
    {
        private const int BatchSize = 1000;
        private readonly ISqlServerTenantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IEnumerable<IProductCreatedOrUpdated> _items;

        private readonly Dictionary<string, ProductEntity> _products = new Dictionary<string, ProductEntity>();
        private readonly List<Advertiser> _advertisers = new List<Advertiser>();
        private readonly List<Agency> _agencies = new List<Agency>();
        private readonly List<Person> _persons = new List<Person>();
        private readonly List<AgencyGroup> _agencyGroups = new List<AgencyGroup>();
        private readonly List<ProductAdvertiser> _productAdvertisers = new List<ProductAdvertiser>();
        private readonly List<ProductAgency> _productAgencies = new List<ProductAgency>();
        private readonly List<ProductPerson> _productPersons = new List<ProductPerson>();
        private readonly List<Guid> _dbProductUIds = new List<Guid>();

        public ProductRuntimeUpdater(ISqlServerTenantDbContext dbContext, IMapper mapper,
            IEnumerable<IProductCreatedOrUpdated> items)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public void Update()
        {
            if (!_items.Any())
            {
                return;
            }

            var dbAdvertisers = _dbContext.Query<Advertiser>().AsNoTracking()
                .ToDictionary(k => k.ExternalIdentifier, v => v);

            var dbAgencies = _dbContext.Query<Agency>().AsNoTracking()
                .ToDictionary(k => k.ExternalIdentifier, v => v);

            var dbPersons = _dbContext.Query<Person>().AsNoTracking()
                .ToDictionary(k => k.ExternalIdentifier, v => v);

            var dbAgencyGroups = _dbContext.Query<AgencyGroup>().AsNoTracking()
                .ToDictionary(k => HashCode.Combine(k.ShortName?.ToUpperInvariant(), k.Code?.ToUpperInvariant()));

            var idx = 0;
            var productBatchIds = new List<string>();

            foreach (var item in _items)
            {
                productBatchIds.Add(item.Externalidentifier);
                var product = _mapper.Map<ProductEntity>(item);
                _products.Add(product.Externalidentifier, product);

                //process advertisers and advertiser links
                ProcessAdvertisers(dbAdvertisers, product,
                    item.Advertisers ?? Enumerable.Empty<Contract.Product.Advertiser>());

                //process agencies and agency links
                ProcessAgencies(dbAgencies, dbAgencyGroups, product,
                    item.Agencies ?? Enumerable.Empty<Contract.Product.Agency>());

                //process persons and person links
                ProcessPersons(dbPersons, product,
                    item.Persons ?? Enumerable.Empty<Contract.Product.Person>());

                idx++;

                if (idx >= BatchSize)
                {
                    ProcessBatch();
                }
            }

            ProcessBatch();
            UpdateDatabase();

            void ProcessBatch()
            {
                var productInfo = _dbContext.Query<ProductEntity>()
                    .Where(x => productBatchIds.Contains(x.Externalidentifier))
                    .Select(x => new { x.Id, x.Externalidentifier })
                    .ToDictionary(k => k.Externalidentifier, v => v.Id);

                foreach (var extId in productBatchIds)
                {
                    if (productInfo.TryGetValue(extId, out var uid))
                    {
                        _dbProductUIds.Add(uid);
                        _products[extId].Id = uid;
                    }
                    else
                    {
                        _products[extId].Id = Guid.NewGuid();
                    }
                }

                idx = 0;
                productBatchIds.Clear();
            }
        }

        protected void ProcessAdvertisers(IDictionary<string, Advertiser> dbAdvertisers, ProductEntity product,
            IEnumerable<Contract.Product.Advertiser> advertisers)
        {
            foreach (var adv in advertisers)
            {
                if (dbAdvertisers.TryGetValue(adv.AdvertiserIdentifier, out var dbAdv))
                {
                    if (!string.Equals(dbAdv.ShortName, adv.ShortName) || !string.Equals(dbAdv.Name, adv.Name))
                    {
                        dbAdv.Name = adv.Name;
                        dbAdv.ShortName = adv.ShortName;
                        _advertisers.Add(dbAdv);
                    }
                }
                else
                {
                    dbAdv = _mapper.Map<Advertiser>(adv);
                    _advertisers.Add(dbAdv);
                    dbAdvertisers.Add(dbAdv.ExternalIdentifier, dbAdv);
                }

                // Product Advertiser activity period should fit the StartDate <= date < EndDate pattern in the database.
                // Due to the fact that Landmark sends StartDate/EndDate as inclusive dates without time part,
                // the Advertiser activity period is adjusted to fit the pattern above.
                _productAdvertisers.Add(new ProductAdvertiser
                {
                    Product = product,
                    Advertiser = dbAdv,
                    StartDate = adv.StartDate.Date,
                    EndDate = adv.EndDate.Date.AddDays(1)
                });
            }
        }

        protected void ProcessAgencies(
            IDictionary<string, Agency> dbAgencies,
            IDictionary<int, AgencyGroup> dbAgencyGroups,
            ProductEntity product,
            IEnumerable<Contract.Product.Agency> agencies)
        {
            foreach (var agency in agencies)
            {
                AgencyGroup dbAgencyGroup = null;

                if (!(string.IsNullOrEmpty(agency.AgencyGroupShortName) &&
                      string.IsNullOrEmpty(agency.AgencyGroupCode)))
                {
                    var aGroupKey = HashCode.Combine(agency.AgencyGroupShortName?.ToUpperInvariant(),
                        agency.AgencyGroupCode?.ToUpperInvariant());

                    if (!dbAgencyGroups.TryGetValue(aGroupKey, out dbAgencyGroup))
                    {
                        dbAgencyGroup = _mapper.Map<AgencyGroup>(agency);
                        dbAgencyGroups.Add(aGroupKey, dbAgencyGroup);
                        _agencyGroups.Add(dbAgencyGroup);
                    }
                }

                if (dbAgencies.TryGetValue(agency.AgencyIdentifier, out var dbAgency))
                {
                    if (!string.Equals(dbAgency.ShortName, agency.ShortName) ||
                        !string.Equals(dbAgency.Name, agency.Name))
                    {
                        dbAgency.Name = agency.Name;
                        dbAgency.ShortName = agency.ShortName;
                        _agencies.Add(dbAgency);
                    }
                }
                else
                {
                    dbAgency = _mapper.Map<Agency>(agency);
                    _agencies.Add(dbAgency);
                    dbAgencies.Add(dbAgency.ExternalIdentifier, dbAgency);
                }

                // Product Agency activity period should fit the StartDate <= date < EndDate pattern in the database.
                // Due to the fact that Landmark sends StartDate/EndDate as inclusive dates without time part,
                // the Agency activity period is adjusted to fit the pattern above.
                _productAgencies.Add(new ProductAgency
                {
                    Product = product,
                    Agency = dbAgency,
                    AgencyGroup = dbAgencyGroup,
                    StartDate = agency.StartDate.Date,
                    EndDate = agency.EndDate.Date.AddDays(1)
                });
            }
        }

        protected void ProcessPersons(IDictionary<int, Person> dbPersons, ProductEntity product,
            IEnumerable<Contract.Product.Person> persons)
        {
            foreach (var person in persons)
            {
                if (dbPersons.TryGetValue(person.PersonIdentifier, out var dbPerson))
                {
                    if (!string.Equals(dbPerson.Name, person.Name))
                    {
                        dbPerson.Name = person.Name;
                        _persons.Add(dbPerson);
                    }
                }
                else
                {
                    dbPerson = _mapper.Map<Person>(person);
                    _persons.Add(dbPerson);
                    dbPersons.Add(dbPerson.ExternalIdentifier, dbPerson);
                }

                // Product Person activity period should fit the StartDate <= date < EndDate pattern in the database.
                // Due to the fact that Landmark sends StartDate/EndDate as inclusive dates without time part,
                // the Person activity period is adjusted to fit the pattern above.
                _productPersons.Add(new ProductPerson
                {
                    Product = product,
                    Person = dbPerson,
                    StartDate = person.StartDate.Date,
                    EndDate = person.EndDate.Date.AddDays(1)
                });
            }
        }

        protected bool HasChanges => _advertisers.Count > 0 || _agencies.Count > 0 || _agencyGroups.Count > 0 ||
                                     _persons.Count > 0 || _products.Count > 0 || _dbProductUIds.Count > 0 ||
                                     _productAdvertisers.Count > 0 || _productAgencies.Count > 0 ||
                                     _productPersons.Count > 0;

        protected void UpdateDatabase()
        {
            if (!HasChanges)
            {
                return;
            }

            using (var transaction = _dbContext.Specific.Database.BeginTransaction())
            {
                if (_advertisers.Count > 0)
                {
                    _dbContext.BulkInsertEngine.BulkInsertOrUpdate(_advertisers,
                        new BulkInsertOptions { SetOutputIdentity = true, PreserveInsertOrder = true });
                }

                if (_agencies.Count > 0)
                {
                    _dbContext.BulkInsertEngine.BulkInsertOrUpdate(_agencies,
                        new BulkInsertOptions { SetOutputIdentity = true, PreserveInsertOrder = true });
                }

                if (_agencyGroups.Count > 0)
                {
                    _dbContext.BulkInsertEngine.BulkInsertOrUpdate(_agencyGroups,
                        new BulkInsertOptions { SetOutputIdentity = true, PreserveInsertOrder = true });
                }

                if (_persons.Count > 0)
                {
                    _dbContext.BulkInsertEngine.BulkInsertOrUpdate(_persons,
                        new BulkInsertOptions { SetOutputIdentity = true, PreserveInsertOrder = true });
                }

                if (_products.Count > 0)
                {
                    _dbContext.BulkInsertEngine.BulkInsertOrUpdate(_products.Values.ToList());
                }

                if (_dbProductUIds.Count > 0)
                {
                    var partitions = Partitioner.Create(_dbProductUIds, EnumerablePartitionerOptions.NoBuffering)
                        .GetPartitions(Math.DivRem(_dbProductUIds.Count, BatchSize, out var mod) + (mod > 0 ? 1 : 0))
                        .Select(e =>
                        {
                            var res = new List<Guid>();

                            while (e.MoveNext())
                            {
                                res.Add(e.Current);
                            }

                            return res.ToArray();
                        }).ToArray();

                    foreach (var partition in partitions)
                    {
                        var advLinkIds = _dbContext.Query<ProductAdvertiser>()
                            .Where(x => partition.Contains(x.ProductId))
                            .Select(x => x.Id).ToArray();
                        _dbContext.Specific.RemoveByIdentityIds<ProductAdvertiser>(advLinkIds);

                        var agencyLinkIds = _dbContext.Query<ProductAgency>()
                            .Where(x => partition.Contains(x.ProductId))
                            .Select(x => x.Id).ToArray();
                        _dbContext.Specific.RemoveByIdentityIds<ProductAgency>(agencyLinkIds);

                        var personLinkIds = _dbContext.Query<ProductPerson>()
                            .Where(x => partition.Contains(x.ProductId))
                            .Select(x => x.Id).ToArray();
                        _dbContext.Specific.RemoveByIdentityIds<ProductPerson>(personLinkIds);
                    }
                }

                if (_productAdvertisers.Count > 0)
                {
                    foreach (var item in _productAdvertisers)
                    {
                        item.ProductId = item.Product.Id;
                        item.AdvertiserId = item.Advertiser.Id;
                    }

                    _dbContext.BulkInsertEngine.BulkInsert(_productAdvertisers);
                }

                if (_productAgencies.Count > 0)
                {
                    foreach (var item in _productAgencies)
                    {
                        item.ProductId = item.Product.Id;
                        item.AgencyId = item.Agency.Id;
                        item.AgencyGroupId = item.AgencyGroup?.Id;
                    }

                    _dbContext.BulkInsertEngine.BulkInsert(_productAgencies);
                }

                if (_productPersons.Count > 0)
                {
                    foreach (var item in _productPersons)
                    {
                        item.ProductId = item.Product.Id;
                        item.PersonId = item.Person.Id;
                    }

                    _dbContext.BulkInsertEngine.BulkInsert(_productPersons);
                }

                transaction.Commit();
            }
        }
    }
}
