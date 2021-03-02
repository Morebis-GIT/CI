using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class ProductResultChecker : ResultCheckerService<Product>
    {
        private readonly IProductRepository _productRepository;

        public ProductResultChecker(ITestDataImporter dataImporter, IProductRepository productRepository) : base(dataImporter) =>
            _productRepository = productRepository;

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var products = _productRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    if (products.Count != featureTestData.Count)
                    {
                        return false;
                    }
                    
                    foreach (var entity in featureTestData)
                    {
                        if (products.Count(c => c.Externalidentifier == entity.Externalidentifier) != 1)
                        {
                            return false;
                        }
                        
                        var storedProduct = products.FirstOrDefault(c => c.Externalidentifier == entity.Externalidentifier);
                        return CompareProduct(entity, storedProduct);
                    }
                    
                    return true;
                }
                default:
                    return false;
            }
        }

        public bool CompareProduct(Product source, Product target) =>
            source.Externalidentifier == target.Externalidentifier &&
            source.ParentExternalidentifier == target.ParentExternalidentifier &&
            source.Name == target.Name &&
            source.EffectiveStartDate == target.EffectiveStartDate &&
            source.EffectiveEndDate == target.EffectiveEndDate &&
            source.ClashCode == target.ClashCode;
    }
}
