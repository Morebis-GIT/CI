using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using xggameplan.specification.tests.Interfaces;
using ProductEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Products.Product;

namespace xggameplan.specification.tests.Infrastructure.SqlServer.DomainModelHandlers
{
    /// <summary>
    /// Product entity is too complicated and product repository add/addrange methods are used in order to populate data
    /// </summary>
    public class ProductDomainModelHandler : SimpleDomainModelMappingHandler<ProductEntity, Product>
    {
        private readonly IProductRepository _repository;

        public ProductDomainModelHandler(IProductRepository repository, ISqlServerTestDbContext dbContext,
            IMapper mapper) : base(dbContext, mapper)
        {
            _repository = repository;
        }

        public override Product Add(Product model)
        {
            _repository.Add(model);
            return model;
        }

        public override void AddRange(params Product[] models)
        {
            _repository.Add(models);
        }

        public override IEnumerable<Product> GetAll() => _repository.GetAll();
    }
}
