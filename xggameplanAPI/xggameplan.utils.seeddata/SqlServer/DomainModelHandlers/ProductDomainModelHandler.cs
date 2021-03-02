using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DomainModelContext;

namespace xggameplan.utils.seeddata.SqlServer.DomainModelHandlers
{
    public class ProductDomainModelHandler : IDomainModelHandler<Product>
    {
        private readonly IProductRepository _productRepository;

        public ProductDomainModelHandler(IProductRepository productRepository) =>
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));

        public Product Add(Product model)
        {
            _productRepository.Add(model);
            return model;
        }

        public void AddRange(params Product[] models) => _productRepository.Add(models);

        public int Count() => _productRepository.CountAll;

        public void DeleteAll() => _productRepository.Truncate();

        public IEnumerable<Product> GetAll() => _productRepository.GetAll();
    }
}
