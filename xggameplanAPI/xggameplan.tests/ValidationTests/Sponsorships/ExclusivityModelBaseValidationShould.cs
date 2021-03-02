using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: Sponsorship :: ExclusivityModelBaseValidationShould")]
    public class ExclusivityModelBaseValidationShould
               : SponsorshipModelValidationTestBase<ExclusivityModelBaseValidation<ExclusivityModelBase>,
                                                    ExclusivityModelBase>
    {
        [TearDown]
        public async Task AfterEach()
        {
            CleanUpTarget();
        }

        [Test(Description = "Fail When Applicability Is EachCompetitor And RestrictionType Is Not Provided")]
        public async Task FailWhenApplicabilityIsEachCompetitorAndRestrictionTypeIsNotProvided()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.Applicability = SponsorshipApplicability.EachCompetitor;
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.Flat;

            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();

            model.RestrictionType = null;

            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When Applicability Is AllCompetitors And RestrictionType Is Provided")]
        public async Task FailWhenApplicabilityIsAllCompetitorsAndRestrictionTypeIsProvided()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.Applicability = SponsorshipApplicability.AllCompetitors;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;

            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is None And RestrictionType Is Provided")]
        public async Task FailWhenCalculationTypeIsNoneAndRestrictionTypeIsProvided()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.Applicability = SponsorshipApplicability.EachCompetitor;
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.None;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;

            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Exclusive And RestrictionType Is Provided")]
        public async Task FailWhenCalculationTypeIsExclusiveAndRestrictionTypeIsProvided()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.Applicability = SponsorshipApplicability.EachCompetitor;
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.None;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotCount;

            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When Applicability Is EachCompetitor And RestrictionValue Is Not Provided")]
        public async Task FailWhenApplicabilityIsEachCompetitorAndRestrictionValueIsNotProvided()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.RestrictionType = null;
            sponsoredItemModel.Applicability = SponsorshipApplicability.EachCompetitor;
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.Flat;

            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionValue = null;

            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is None And RestrictionType Is Provided And RestrictionValue Is Not Provided")]
        public async Task FailWhenCalculationTypeIsNoneAndRestrictionTypeIsProvidedAndRestrictionValueIsNotProvided()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.None;
            sponsoredItemModel.RestrictionType = SponsorshipRestrictionType.SpotCount;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionValue = null;
            model.RestrictionType = SponsorshipRestrictionType.SpotDuration;

            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Exclusive And RestrictionType Is Provided And RestrictionValue Is Not Provided")]
        public async Task FailWhenCalculationTypeIsExclusiveAndRestrictionTypeIsProvidedAndRestrictionValueIsNotProvided()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.Exclusive;
            sponsoredItemModel.RestrictionType = SponsorshipRestrictionType.SpotCount;
            sponsoredItemModel.Applicability = SponsorshipApplicability.AllCompetitors;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotDuration;
            model.RestrictionValue = null;

            var validationResult = Target.Validate(model, a => a.RestrictionType);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Flat RestrictionType Is Provided And RestrictionValue Is LessThan Zero")]
        public async Task FailWhenCalculationTypeIsFlatRestrictionTypeIsProvidedAndRestrictionValueIsNotGreaterThanZero()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.Flat;
            sponsoredItemModel.Applicability = SponsorshipApplicability.EachCompetitor;
            sponsoredItemModel.RestrictionType = SponsorshipRestrictionType.SpotCount;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionValue = -1;

            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Percentage RestrictionType Is Provided And RestrictionValue Is LessThan Zero")]
        public async Task FailWhenCalculationTypeIsPercentageRestrictionTypeIsProvidedAndRestrictionValueIsNotGreaterThanZero()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.Percentage;
            sponsoredItemModel.Applicability = SponsorshipApplicability.EachCompetitor;
            sponsoredItemModel.RestrictionType = SponsorshipRestrictionType.SpotCount;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionValue = -1;

            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Flat RestrictionType Is Provided And RestrictionValue Is Zero")]
        public async Task FailWhenCalculationTypeIsFlatRestrictionTypeIsProvidedAndRestrictionValueIsZero()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.Flat;
            sponsoredItemModel.Applicability = SponsorshipApplicability.EachCompetitor;
            sponsoredItemModel.RestrictionType = SponsorshipRestrictionType.SpotCount;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionValue = 0;

            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Percentage RestrictionType Is Provided And RestrictionValue Is Zero")]
        public async Task FailWhenCalculationTypeIsPercentageRestrictionTypeIsProvidedAndRestrictionValueIsZero()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.Percentage;
            sponsoredItemModel.Applicability = SponsorshipApplicability.EachCompetitor;
            sponsoredItemModel.RestrictionType = SponsorshipRestrictionType.SpotCount;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionValue = 0;

            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Percentage RestrictionType Is Provided And RestrictionValue Is Greather Than 100")]
        public async Task FailWhenCalculationTypeIsPercentageRestrictionTypeIsProvidedAndRestrictionValueIsGreatherThan100()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.Percentage;
            sponsoredItemModel.Applicability = SponsorshipApplicability.EachCompetitor;
            sponsoredItemModel.RestrictionType = SponsorshipRestrictionType.SpotCount;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionValue = 101;

            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When Applicability Is AllCompetitors And RestrictionValue Is Provided")]
        public async Task FailWhenApplicabilityIsAllCompetitorsAndRestrictionValueIsProvided()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.Applicability = SponsorshipApplicability.AllCompetitors;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionValue = 6;

            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Flat And Applicability Is AllCompetitors And RestrictionValue Is Provided")]
        public async Task FailWhenCalculationTypeIsFlatAndApplicabilityIsAllCompetitorsAndRestrictionValueIsProvided()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.Applicability = SponsorshipApplicability.AllCompetitors;
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.Flat;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionValue = 6;

            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When CalculationType Is Flat And Applicability Is AllCompetitors And RestrictionValue Is Provided")]
        public async Task FailWhenCalculationTypeIsFlatAndApplicabilityIsAllCompetitorsAndRestrictionTypeIsProvided()
        {
            var sponsoredItemModel = AssumeDependencyIsSupplied();
            sponsoredItemModel.Applicability = SponsorshipApplicability.AllCompetitors;
            sponsoredItemModel.CalculationType = SponsorshipCalculationType.Flat;
            AssumeTargetIsInitialised(sponsoredItemModel);
            var model = AssumeValidModelIsSupplied();
            model.RestrictionType = SponsorshipRestrictionType.SpotDuration;

            var validationResult = Target.Validate(model, a => a.RestrictionValue);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        private SponsoredItemModelBase AssumeDependencyIsSupplied()
        {
            return CreateValidModel<SponsoredItemModelBase>();
        }
    }
}
