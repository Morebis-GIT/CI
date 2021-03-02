using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Tenants;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Contexts;

namespace xgCore.xgGamePlan.AutomationTests.Features.Tenants
{
    [Binding]
    public class TenantsSteps
    {
        private readonly ITenantsApi _tenantsApi;
        private readonly IFixture _autoFixture;
        private readonly TenantsContext _tenantsContext;

        public TenantsSteps(ITenantsApi tenantsApi, IFixture autoFixture, TenantsContext tenantsContext)
        {
            _tenantsApi = tenantsApi;
            _autoFixture = autoFixture;
            _tenantsContext = tenantsContext;
        }

        [Given(@"I know how many Tenants there are")]
        public async Task GivenHowManyTenantsThereAreAsync()
        {
            _tenantsContext.InitialTenants = await _tenantsApi.GetAll().ConfigureAwait(false);
        }

        [Given(@"I have added a Tenant")]
        public async Task GivenIHaveAddedTenantAsync()
        {
            _tenantsContext.GivenTenant =
                (await _tenantsApi.Create(BuildTenantCreateModel(1).Single()).ConfigureAwait(false));
        }

        [When(@"I add (\d+) Tenants")]
        public async Task WhenIAddTenantsAsync(int count)
        {
            await Task.WhenAll(BuildTenantCreateModel(count)
                .Select(async t => await _tenantsApi.Create(t).ConfigureAwait(false))).ConfigureAwait(false);
        }

        [When(@"I request my Tenant by ID")]
        public async Task WhenIRequestMyTenantByIdAsync()
        {
            Assert.NotNull(_tenantsContext.GivenTenant, "There is no given tenant.");
            _tenantsContext.ReturnedTenant =
                await _tenantsApi.GetById(_tenantsContext.GivenTenant.Id).ConfigureAwait(false);
        }

        [When(@"I update Tenant by ID")]
        public async Task WhenIUpdateTenantByIdAsync()
        {
            Assert.NotNull(_tenantsContext.GivenTenant, "There is no given tenant.");
            var tenantUpdate = BuildTenantCreateModel(1).Single();
            _tenantsContext.ReturnedTenant = await _tenantsApi.Update(tenantUpdate, _tenantsContext.GivenTenant.Id)
                .ConfigureAwait(false);
        }

        [Then(@"(\d+) additional Tenants are returned")]
        public async Task ThenAdditionalTenantsAreReturnedAsync(int count)
        {
            var tenants = await _tenantsApi.GetAll().ConfigureAwait(false);
            Assert.AreEqual(count, tenants.Count - _tenantsContext.InitialTenants.Count);
        }

        [Then(@"requested Tenant with ID is returned")]
        public void ThenRequestedTenantWithIdIsReturnedAsync()
        {
            Assert.AreEqual(_tenantsContext.GivenTenant?.Id, _tenantsContext.ReturnedTenant?.Id);
        }

        [Then(@"updated Tenant is returned")]
        public async void ThenUpdatedTenantIsReturnedAsync()
        {
            Assert.NotNull(_tenantsContext.ReturnedTenant, "There is no updated tenant.");
            var tenant = await _tenantsApi.GetById(_tenantsContext.ReturnedTenant.Id).ConfigureAwait(false);
            Assert.AreEqual(_tenantsContext.ReturnedTenant, tenant);
        }

        private IEnumerable<TenantCreate> BuildTenantCreateModel(int count)
        {
            return _autoFixture.Build<TenantCreate>()
                .CreateMany(count).ToList();
        }
    }
}
