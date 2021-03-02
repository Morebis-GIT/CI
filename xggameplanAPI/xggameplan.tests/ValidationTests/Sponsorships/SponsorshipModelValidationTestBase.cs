using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Sponsorships;
using Moq;
using NUnit.Framework;

namespace xggameplan.tests.ValidationTests
{
    public abstract class SponsorshipModelValidationTestBase<TTarget, TModel>
    where TTarget : AbstractValidator<TModel>
    where TModel : class
    {
        protected Mock<ISalesAreaRepository> SalesAreaRepository { get; set; }
        protected Mock<IProgrammeRepository> ProgrammeRepository { get; set; }
        protected Mock<IProductRepository> ProductRepository { get; set; }
        protected Mock<ISponsorshipRepository> SponsorshipRepository { get; set; }
        protected Mock<IClashRepository> ClashRepository { get; set; }
        protected TTarget Target { get; set; }
        protected Fixture Fixture { get; set; }

        [OneTimeSetUp]
        public async Task OnInit()
        {
            SetUpFixture();
        }

        [OneTimeTearDown]
        public async Task OnDestroy()
        {
            CleanUp();
        }

        protected virtual void AssumeDependenciesAreSupplied()
        {
            SalesAreaRepository = new Mock<ISalesAreaRepository>();
            ProgrammeRepository = new Mock<IProgrammeRepository>();
            ProductRepository = new Mock<IProductRepository>();
            SponsorshipRepository = new Mock<ISponsorshipRepository>();
            ClashRepository = new Mock<IClashRepository>();
        }

        protected virtual void AssumeTargetIsInitialised(params object[] withDependencies)
        {
            Target = (TTarget)Activator.CreateInstance(typeof(TTarget), withDependencies);
        }

        protected virtual TModel AssumeValidModelIsSupplied()
        {
            return CreateValidModel<TModel>();
        }

        protected virtual T CreateValidModel<T>()
        {
            return CreateValidModels<T>(1).FirstOrDefault();
        }

        protected virtual IEnumerable<T> CreateValidModels<T>(int noOfItems)
        {
            return Fixture.CreateMany<T>(noOfItems);
        }

        protected void CleanUp()
        {
            CleanUpFixture();
            CleanUpTarget();
            CleanUpDependencies();
        }

        protected void CleanUpTarget()
        {
            Target = null;
        }

        protected virtual void CleanUpDependencies()
        {
            SalesAreaRepository = null;
            ProgrammeRepository = null;
            ProductRepository = null;
            SponsorshipRepository = null;
            ClashRepository = null;
        }

        protected void SetUpFixture()
        {
            Fixture = new Fixture();
        }

        protected void CleanUpFixture()
        {
            Fixture = null;
        }
    }
}
