using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.Products;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Products
{
    [Binding]
    public class ProductsSteps : BaseSteps<IProductsApi>
    {
        public ProductsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I have a valid Product")]
        public async Task GivenIHaveAValidProduct()
        {
            var products = BuildValidProducts().ToList();
            await Api.Create(products).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            ScenarioContext.Set(products.First());
        }

        [Given(@"I have some Products with same Advertiser Identifier")]
        public async Task GivenIHaveSomeValidProducts()
        {
            var advertiserIdentifier = Fixture.Create<string>();
            var products = await AddProductsWithAdvertiserIdentifier(3, advertiserIdentifier).ConfigureAwait(false);
            ScenarioContext.Set(products);
        }

        [Given(@"I have a Product with not existing external id")]
        public void GivenIHaveAProductWithNotExistingExternalId()
        {
            var products = BuildValidProducts();
            ScenarioContext.Set(products.First());
        }

        [Given(@"I know how many Products there are")]
        public async Task GivenIKnowHowManyProductsThereAre()
        {
            var products = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(products == null ? 0 : products.Count());
        }

        [Given(@"I have added (.*) Products")]
        public async Task GivenIHaveAddedProducts(int count)
        {
            var products = Fixture.CreateMany<Product>(count);
            await Api.Create(products).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Given(@"I know Product externalRef")]
        public async Task GivenIKnowProductExternalRef()
        {
            var products = await Api.GetAll().ConfigureAwait(false);
            var externalRef = products.FirstOrDefault()?.Externalidentifier;
            ScenarioContext.Set(externalRef);
        }

        [Given(@"I know how many Products with Name there are")]
        public async Task GivenIKnowHowManyProductsWithNameThereAre()
        {
            var products = await Api.GetAll().ConfigureAwait(false);
            var name = products.FirstOrDefault()?.Name;
            ScenarioContext.Set(name);
            var count = products.Where(x => x.Name == name).Count();
            ScenarioContext.Set(count);
        }

        [Given(@"I know how many Products with Advertiser Name there are")]
        public async Task GivenIKnowHowManyProductsWithAdvertiserNameThereAre()
        {
            var products = await Api.GetAll().ConfigureAwait(false);
            var advertiserName = products.FirstOrDefault()?.AdvertiserName;
            ScenarioContext.Set(advertiserName);
            var count = products.Where(x => x.AdvertiserName == advertiserName).Count();
            ScenarioContext.Set(count);
        }

        [When(@"I add (.*) Products")]
        public async Task WhenIAddProducts(int count)
        {
            var products = Fixture.CreateMany<Product>(count);
            await Api.Create(products).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I add (.*) Products with Name")]
        public async Task WhenIAddProductsWithName(int count)
        {
            var name = ScenarioContext.Get<string>();
            var products = Fixture.Build<Product>().With(x => x.Name, name).CreateMany(count);
            await Api.Create(products).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            ScenarioContext.Set(name);
        }

        [When(@"I upsert Product by external id")]
        public async Task WhenIUpsertProductByExternalId()
        {
            var product = ScenarioContext.Get<Product>();
            product.AdvertiserName = "test advertiser name 1";
            var updatedProduct = await Api.Put(product.Externalidentifier, product).ConfigureAwait(false);
            ScenarioContext.Set(updatedProduct);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I search for Products with Name")]
        public async Task WhenISearchForProductsWithName()
        {
            var name = ScenarioContext.Get<string>();
            var products = await Api.Search(new ProductSearchQueryModel()
            {
                Name = name
            }).ConfigureAwait(false);
            ScenarioContext.Set(products);
        }

        [When(@"I add (.*) Products with Advertiser Name")]
        public async Task WhenIAddProductsWithAdvertiserName(int count)
        {
            var advertiserName = ScenarioContext.Get<string>();
            var products = Fixture.Build<Product>().With(x => x.AdvertiserName, advertiserName).CreateMany(count);
            await Api.Create(products).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            ScenarioContext.Set(advertiserName);
        }

        [When(@"I search Products by Advertiser Name")]
        public async Task WhenISearchProductsByAdvertiserName()
        {
            var advertiserName = ScenarioContext.Get<string>();
            var products = await Api.SearchByAdvertiser(new AdvertiserSearchQueryModel()
            {
                AdvertiserNameorRef = advertiserName
            }).ConfigureAwait(false);
            ScenarioContext.Set(products);
        }

        [When(@"I get Product by externalRef")]
        public async Task WhenIGetProductByExternalRef()
        {
            var externalRef = ScenarioContext.Get<string>();
            var product = await Api.GetByExternalRef(externalRef).ConfigureAwait(false);
            ScenarioContext.Set(product);
        }

        [When(@"I delete all Products")]
        public async Task WhenIDeleteAllProducts()
        {
            await Api.DeleteAll().ConfigureAwait(false);
        }

        [Then(@"(.*) additional Products are returned")]
        public async Task ThenAdditionalProductsAreReturned(int count)
        {
            var allProducts = await Api.GetAll().ConfigureAwait(false);

            var knownCount = ScenarioContext.Get<int>();
            Assert.AreEqual(allProducts.Count(), knownCount + count);
        }

        [Then(@"(.*) additional Products are found")]
        public void ThenAdditionalProductsAreFound(int count)
        {
            var result = ScenarioContext.Get<SearchResultModel<Product>>();
            var knownCount = ScenarioContext.Get<int>();
            Assert.AreEqual(result.TotalCount, knownCount + count);
        }

        [Then(@"Product is returned")]
        public void ThenProductIsReturned()
        {
            var product = ScenarioContext.Get<Product>();
            Assert.IsNotNull(product);
        }

        [Then(@"updated Product is returned")]
        public async Task ThenUpdatedProductIsReturned()
        {
            var updatedProduct = ScenarioContext.Get<Product>();
            var product = await Api.GetByExternalRef(updatedProduct.Externalidentifier).ConfigureAwait(false);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(product.Externalidentifier, updatedProduct.Externalidentifier);
                Assert.AreEqual(product.AdvertiserName, updatedProduct.AdvertiserName);
            });
        }

        [Then(@"no Products are returned")]
        public async Task ThenNoProductsAreReturned()
        {
            var allProducts = await Api.GetAll().ConfigureAwait(false);
            Assert.IsNull(allProducts);
        }

        private async Task<IEnumerable<Product>> AddProductsWithAdvertiserIdentifier(int count, string advertiserIdentifier)
        {
            var products = Fixture.Build<Product>().With(x => x.AdvertiserIdentifier, advertiserIdentifier).CreateMany(count);
            await Api.Create(products).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            return products;
        }

        private IEnumerable<Product> BuildValidProducts(int count = 1)
        {
            return Fixture
                .Build<Product>()
                .With(p => p.EffectiveStartDate, DateTime.Now)
                .With(p => p.EffectiveEndDate, DateTime.Now.AddDays(1))
                .CreateMany(count);
        }
    }
}
