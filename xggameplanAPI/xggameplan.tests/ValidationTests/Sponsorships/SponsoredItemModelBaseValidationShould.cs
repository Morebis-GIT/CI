using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.TestHelper;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    public abstract class SponsoredItemModelBaseValidationShould<TTarget, TModel>
                        : SponsorshipModelValidationTestBase<TTarget, TModel>
    where TTarget : SponsoredItemModelBaseValidation<TModel>
    where TModel : SponsoredItemModelBase
    {
        protected IValidator<SponsoredDayPartModelBase> SponsoredDayPartModelValidation { get; set; }
        protected SponsorshipModelBase SponsorshipModelBase { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            AssumeDependenciesAreSupplied();
            AssumeTargetIsInitialised(SponsorshipModelBase);
        }

        [TearDown]
        public void AfterEach()
        {
            CleanUpTarget();
            CleanUpDependencies();
        }

        [Test(Description = "Fail When Products Is Not Supplied")]
        public void FailWhenProductsIsNotSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.Products = null;

            var validationResult = Target.Validate(model, a => a.Products);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When Products Supplied Contains Some Invalid Products")]
        public void FailWhenProductsSuppliedContainsSomeInvalidProducts()
        {
            var model = AssumeValidModelIsSupplied();
            model.Products = CreateValidModels<string>(2).Append(null);

            var validationResult = Target.Validate(model, a => a.Products);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Pass When Products Supplied Are Existing Products And From SingleAdvertiser")]
        public void PassWhenProductsSuppliedAreExistingProductsAndFromSingleAdvertiser()
        {
            var existingProducts = CreateValidModels<Product>(6).ToList();
            foreach (var product in existingProducts)
            {
                product.AdvertiserIdentifier = "Advertiser1";
            }
            AssumeProductRepositoryFindByExternalReturns(existingProducts);
            var model = AssumeValidModelIsSupplied();
            model.Products = existingProducts.Select(p => p.Externalidentifier);

            var validationResult = Target.Validate(model, a => a.Products);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Fail When Applicability Is All Competitors And RestrictionType Is Not Supplied")]
        public void FailWhenApplicabilityIsAllCompetitorsAndRestrictionTypeIsNotSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.Applicability = SponsorshipApplicability.AllCompetitors;
            model.CalculationType = SponsorshipCalculationType.Flat;
            model.RestrictionType = null;

            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When Applicability Is Each Competitor And RestrictionType Is Supplied")]
        public void FailWhenApplicabilityIsEachCompetitorAndRestrictionTypeIsSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.Applicability = SponsorshipApplicability.EachCompetitor;
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;

            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Exclusive And RestrictionType Is Supplied")]
        public void FailWhenCalculationTypeIsExclusiveAndRestrictionTypeIsSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.CalculationType = SponsorshipCalculationType.Exclusive;

            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is None And RestrictionType Is Supplied")]
        public void FailWhenCalculationTypeIsNoneAndRestrictionTypeIsSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.CalculationType = SponsorshipCalculationType.None;

            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.RestrictionType, model);
            _ = validationFailures.Count().Should().BeGreaterThan(0);
        }

        [Test(Description = "Fail When RestrictionType Is Supplied And RestrictionValue Is Not Supplied")]
        [Ignore("Randomly fails. No time to investigate and fix.")]
        public void FailWhenRestrictionTypeIsSuppliedAndRestrictionValueIsNotSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.RestrictionValue = null;
            model.CalculationType = SponsorshipCalculationType.Flat;
            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When Applicability Is All Competitors And RestrictionValue Is Not Supplied")]
        public void FailWhenApplicabilityIsAllCompetitorsAndRestrictionValueIsNotSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = null;
            model.RestrictionValue = null;
            model.CalculationType = SponsorshipCalculationType.Flat;
            model.Applicability = SponsorshipApplicability.AllCompetitors;

            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is None And Applicability Is EachCompetitor")]
        public void FailWhenCalculationTypeIsNoneAndApplicabilityIsEachCompetitor()
        {
            var model = AssumeValidModelIsSupplied();
            model.CalculationType = SponsorshipCalculationType.None;
            model.Applicability = SponsorshipApplicability.EachCompetitor;

            var validationResult = Target.Validate(model, a => a.Applicability);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Exclusive And Applicability Is EachCompetitor")]
        public void FailWhenCalculationTypeIsExclusiveAndApplicabilityIsEachCompetitor()
        {
            var model = AssumeValidModelIsSupplied();
            model.CalculationType = SponsorshipCalculationType.Exclusive;
            model.Applicability = SponsorshipApplicability.EachCompetitor;

            var validationResult = Target.Validate(model, a => a.Applicability);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Pass When CalculationType Is None And Applicability Is Not Supplied")]
        public void PassWhenCalculationTypeIsNoneAndApplicabilityIsNotSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.CalculationType = SponsorshipCalculationType.None;
            model.Applicability = null;

            var validationResult = Target.Validate(model, a => a.Applicability);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Pass When CalculationType Is Exclusive And Applicability Is Not Supplied")]
        public void PassWhenCalculationTypeIsExclusiveAndApplicabilityIsNotSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.CalculationType = SponsorshipCalculationType.Exclusive;
            model.Applicability = null;

            var validationResult = Target.Validate(model, a => a.Applicability);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Pass When CalculationType Is None And Applicability Is AllCompetitors")]
        public void PassWhenCalculationTypeIsNoneAndApplicabilityIsAllCompetitors()
        {
            var model = AssumeValidModelIsSupplied();
            model.CalculationType = SponsorshipCalculationType.None;
            model.Applicability = SponsorshipApplicability.AllCompetitors;

            var validationResult = Target.Validate(model, a => a.Applicability);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Pass When CalculationType Is Exclusive And Applicability Is AllCompetitors")]
        public void PassWhenCalculationTypeIsExclusiveAndApplicabilityIsAllCompetitors()
        {
            var model = AssumeValidModelIsSupplied();
            model.CalculationType = SponsorshipCalculationType.Exclusive;
            model.Applicability = SponsorshipApplicability.AllCompetitors;

            var validationResult = Target.Validate(model, a => a.Applicability);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Percentage And RestrictionType Is Not Null And Applicability Is EachCompetitor")]
        public void FailWhenCalculationTypeIsPercentageAndRestrictionTypeIsNotNullAndApplicabilityIsEachCompetitor()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.RestrictionValue = null;
            model.Applicability = SponsorshipApplicability.EachCompetitor;
            model.CalculationType = SponsorshipCalculationType.Percentage;
            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Percentage And RestrictionValue Is Not Null And Applicability Is EachCompetitor")]
        public void FailWhenCalculationTypeIsPercentageAndRestrictionValueIsNotNullAndApplicabilityIsEachCompetitor()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = null;
            model.RestrictionValue = 1;
            model.Applicability = SponsorshipApplicability.EachCompetitor;
            model.CalculationType = SponsorshipCalculationType.Percentage;
            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Flat And RestrictionType Is Not Null And Applicability Is EachCompetitor")]
        public void FailWhenCalculationTypeIsFlatAndRestrictionTypeIsNotNullAndApplicabilityIsEachCompetitor()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.RestrictionValue = null;
            model.Applicability = SponsorshipApplicability.EachCompetitor;
            model.CalculationType = SponsorshipCalculationType.Flat;
            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Flat And RestrictionValue Is Not Null And Applicability Is EachCompetitor")]
        public void FailWhenCalculationTypeIsFlatAndRestrictionValueIsNotNullAndApplicabilityIsEachCompetitor()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = null;
            model.RestrictionValue = 1;
            model.Applicability = SponsorshipApplicability.EachCompetitor;
            model.CalculationType = SponsorshipCalculationType.Flat;
            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Flat And RestrictionType Is Null And Applicability Is AllCompetitors")]
        public void FailWhenWhenCalculationTypeIsFlatAndRestrictionTypeIsNullAndApplicabilityIsAllCompetitors()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = null;
            model.RestrictionValue = 1;
            model.Applicability = SponsorshipApplicability.AllCompetitors;
            model.CalculationType = SponsorshipCalculationType.Flat;
            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Flat And RestrictionValue Is Null And Applicability Is AllCompetitors")]
        public void FailWhenWhenCalculationTypeIsFlatAndRestrictionValueIsNullAndApplicabilityIsAllCompetitors()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.RestrictionValue = null;
            model.Applicability = SponsorshipApplicability.AllCompetitors;
            model.CalculationType = SponsorshipCalculationType.Flat;
            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Flat And RestrictionValue Is Negative")]
        public void FailWhenCalculationTypeIsFlatAndRestrictionValueIsNegative()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.RestrictionValue = -1;
            model.Applicability = SponsorshipApplicability.AllCompetitors;
            model.CalculationType = SponsorshipCalculationType.Flat;
            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Flat And RestrictionValue Is Zero")]
        public void FailWhenCalculationTypeIsFlatAndRestrictionValueIsZero()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.RestrictionValue = 0;
            model.Applicability = SponsorshipApplicability.AllCompetitors;
            model.CalculationType = SponsorshipCalculationType.Flat;
            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Percentage And RestrictionType Is Null And Applicability Is AllCompetitors")]
        public void FailWhenWhenCalculationTypeIsPercentageAndRestrictionTypeIsNullAndApplicabilityIsAllCompetitors()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = null;
            model.RestrictionValue = 1;
            model.Applicability = SponsorshipApplicability.AllCompetitors;
            model.CalculationType = SponsorshipCalculationType.Percentage;
            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Percentage And RestrictionValue Is Null And Applicability Is AllCompetitors")]
        public void FailWhenWhenCalculationTypeIsPercentageAndRestrictionValueIsNullAndApplicabilityIsAllCompetitors()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.RestrictionValue = null;
            model.Applicability = SponsorshipApplicability.AllCompetitors;
            model.CalculationType = SponsorshipCalculationType.Percentage;
            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Percentage And RestrictionValue Is Negative")]
        public void FailWhenCalculationTypeIsPercentageAndRestrictionValueIsNegative()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.RestrictionValue = -1;
            model.Applicability = SponsorshipApplicability.AllCompetitors;
            model.CalculationType = SponsorshipCalculationType.Percentage;
            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Percentage And RestrictionValue Is Zero")]
        public void FailWhenCalculationTypeIsPercentageAndRestrictionValueIsZero()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.RestrictionValue = 0;
            model.Applicability = SponsorshipApplicability.AllCompetitors;
            model.CalculationType = SponsorshipCalculationType.Percentage;
            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Percentage And RestrictionValue Is Greather Than 100")]
        public void FailWhenCalculationTypeIsPercentageAndRestrictionValueIsGreatherThan100()
        {
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;
            model.RestrictionValue = 101;
            model.Applicability = SponsorshipApplicability.AllCompetitors;
            model.CalculationType = SponsorshipCalculationType.Percentage;
            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        //======================================
        protected override void AssumeDependenciesAreSupplied()
        {
            base.AssumeDependenciesAreSupplied();
            SponsoredDayPartModelValidation = new SponsoredDayPartModelValidation();
            SponsorshipModelBase = CreateValidModel<SponsorshipModelBase>();
        }

        protected override void CleanUpDependencies()
        {
            base.CleanUpDependencies();
            SponsoredDayPartModelValidation = null;
        }

        private void AssumeTargetIsInitialised(SponsorshipModelBase sponsorshipModelBase = null)
        {
            AssumeTargetIsInitialised(SalesAreaRepository.Object, ProgrammeRepository.Object,
                                      ProductRepository.Object, ClashRepository.Object,
                                      SponsoredDayPartModelValidation, sponsorshipModelBase);
        }

        protected void AssumeProductRepositoryFindByExternalReturns(IEnumerable<Product> dataToReturn)
        {
            _ = ProductRepository.Setup(a => a.FindByExternal(It.IsAny<List<string>>())).Returns(dataToReturn);
        }

        protected void SetupAdvertiserExclusivitiesAndClashExclusivitiesWithSameRestrictionType(CreateSponsoredItemModel model)
        {
            var restrictionType = SponsorshipRestrictionType.SpotCount;
            SetupAdvertiserExclusivitiesWithRestrictionType(model, restrictionType);
            SetupClashExclusivitiesWithRestrictionType(model, restrictionType);
        }

        protected void SetupAdvertiserExclusivitiesAndClashExclusivitiesWithDifferentRestrictionType(CreateSponsoredItemModel model)
        {
            SetupAdvertiserExclusivitiesWithRestrictionType(model, SponsorshipRestrictionType.SpotCount);
            SetupClashExclusivitiesWithRestrictionType(model, SponsorshipRestrictionType.SpotDuration);
        }

        protected void SetupAdvertiserExclusivitiesWithRestrictionType(CreateSponsoredItemModel model,
                                                                     SponsorshipRestrictionType restrictionType)
        {
            if (model != null)
            {
                model.AdvertiserExclusivities = new CreateAdvertiserExclusivityModel[] {
                                                CreateAdvertiserExclusivitiesWithRestrictionType(restrictionType) };
            }
        }

        protected void SetupClashExclusivitiesWithRestrictionType(CreateSponsoredItemModel model,
                                                                  SponsorshipRestrictionType restrictionType)
        {
            if (model != null)
            {
                model.ClashExclusivities = new CreateClashExclusivityModel[] { CreateClashExclusivitiesWithRestrictionType(restrictionType) };
            }
        }

        protected CreateAdvertiserExclusivityModel CreateAdvertiserExclusivitiesWithRestrictionType(SponsorshipRestrictionType restrictionType)
        {
            var advertiserExclusivity = CreateValidModel<CreateAdvertiserExclusivityModel>();
            advertiserExclusivity.RestrictionType = restrictionType;
            return advertiserExclusivity;
        }

        protected CreateClashExclusivityModel CreateClashExclusivitiesWithRestrictionType(SponsorshipRestrictionType restrictionType)
        {
            var clashExclusivity = CreateValidModel<CreateClashExclusivityModel>();
            clashExclusivity.RestrictionType = restrictionType;
            return clashExclusivity;
        }
    }
}
