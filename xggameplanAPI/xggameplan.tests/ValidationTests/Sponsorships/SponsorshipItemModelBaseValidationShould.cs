using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using FluentValidation.TestHelper;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    public abstract class SponsorshipItemModelBaseValidationShould<TTarget, TModel>
                        : SponsorshipModelValidationTestBase<TTarget, TModel>
    where TTarget : SponsorshipItemModelBaseValidation<TModel>
    where TModel : SponsorshipItemModelBase
    {
        protected IValidator<SponsoredDayPartModelBase> SponsoredDayPartModelValidation { get; set; }
        protected SponsorshipModelBase SponsorshipModelBase { get; set; }

        [SetUp]
        public async Task BeforeEach()
        {
            AssumeDependenciesAreSupplied();
            AssumeTargetIsInitialised(SponsorshipModelBase);
        }

        [TearDown]
        public async Task AfterEach()
        {
            CleanUpTarget();
            CleanUpDependencies();
        }

        [Test(Description = "Fail When SalesAreas Is Not Supplied")]
        public async Task FailWhenSalesAreasIsNotSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.SalesAreas = null;

            var validationResult = Target.Validate(model, a => a.SalesAreas);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When SalesAreas Supplied Contains Some Invalid SalesAreas")]
        public async Task FailWhenSalesAreasSuppliedContainsSomeInvalidSalesAreas()
        {
            var model = AssumeValidModelIsSupplied();
            model.SalesAreas = CreateValidModels<string>(2).Append(null);

            var validationResult = Target.Validate(model, a => a.SalesAreas);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When SalesAreas Supplied Contains NonExisting SalesAreas")]
        public async Task FailWhenSalesAreasSuppliedContainsNonExistingSalesAreas()
        {
            var model = AssumeValidModelIsSupplied();
            var nonExistingSalesAreas = CreateValidModels<string>(2).ToList();
            AssumeSalesAreaRepositoryGetListOfNamesReturns(nonExistingSalesAreas);

            var validationResult = Target.Validate(model, a => a.SalesAreas);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Pass When SalesAreas Supplied Contains Only Existing SalesAreas")]
        public async Task PassWhenSalesAreasSuppliedContainsOnlyExistingSalesAreas()
        {
            var model = AssumeValidModelIsSupplied();
            AssumeSalesAreaRepositoryGetListOfNamesReturns(model.SalesAreas.ToList());

            var validationResult = Target.Validate(model, a => a.SalesAreas);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Fail When StartDate Is Less Than Today")]
        public async Task FailWhenStartDateIsLessThenToday()
        {
            var model = AssumeValidModelIsSupplied();
            model.StartDate = DateTime.UtcNow.Date.AddDays(-5);

            var validationResult = Target.Validate(model, a => a.StartDate);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Pass When StartDate Is Greater Than Today")]
        public async Task PassWhenStartDateIsGreaterThanToday()
        {
            var model = AssumeValidModelIsSupplied();
            model.StartDate = DateTime.UtcNow.Date.AddDays(1);

            var validationResult = Target.Validate(model, a => a.StartDate);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Pass When StartDate Is Today")]
        public async Task PassWhenStartDateIsToday()
        {
            var model = AssumeValidModelIsSupplied();
            model.StartDate = DateTime.UtcNow.Date;

            var validationResult = Target.Validate(model, a => a.StartDate);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Fail When EndDate Is Less Than Today")]
        public async Task FailWhenEndDateIsLessThenToday()
        {
            var model = AssumeValidModelIsSupplied();
            model.EndDate = DateTime.UtcNow.Date.AddDays(-2);

            var validationResult = Target.Validate(model, a => a.EndDate);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Fail When EndDate Is Less Than StartDate")]
        public async Task FailWhenEndDateIsLessThenStartDate()
        {
            var model = AssumeValidModelIsSupplied();
            model.StartDate = DateTime.UtcNow.Date.AddDays(5);
            model.EndDate = model.StartDate.AddDays(-2);

            var validationResult = Target.Validate(model, a => a.EndDate);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Pass When EndDate Is Greater Than StartDate")]
        public async Task PassWhenEndDateIsGreaterThanStartDate()
        {
            var model = AssumeValidModelIsSupplied();
            model.StartDate = DateTime.UtcNow.Date;
            model.EndDate = model.StartDate.AddDays(2);

            var validationResult = Target.Validate(model, a => a.EndDate);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Pass When EndDate Is Equal To StartDate")]
        public async Task PassWhenEndDateIsEqualToStartDate()
        {
            var model = AssumeValidModelIsSupplied();
            model.StartDate = DateTime.Now.Date;
            model.EndDate = model.StartDate;

            var validationResult = Target.Validate(model, a => a.EndDate);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Fail When RestrictionLevel Is Programme And ProgrammeName Is Not Supplied")]
        public async Task FailWhenRestrictionLevelIsProgrammeAndProgrammeNameIsNotSupplied()
        {
            var sponsorshipModelBase = CreateValidModel<SponsorshipModelBase>();
            sponsorshipModelBase.RestrictionLevel = SponsorshipRestrictionLevel.Programme;
            AssumeTargetIsInitialised(sponsorshipModelBase);
            var model = AssumeValidModelIsSupplied();
            model.ProgrammeName = null;

            var validationResult = Target.Validate(model, a => a.ProgrammeName);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse();
                _ = validationResult.Errors.Count.Should().BeGreaterThan(0);
            }
        }

        [Test(Description = "Pass When RestrictionLevel Is Programme And ProgrammeName Supplied Exists")]
        public async Task PassWhenRestrictionLevelIsProgrammeAndProgrammeNamesSuppliedExists()
        {
            AssumeDependenciesAreSupplied();
            var sponsorshipModelBase = CreateValidModel<SponsorshipModelBase>();
            sponsorshipModelBase.RestrictionLevel = SponsorshipRestrictionLevel.Programme;
            AssumeTargetIsInitialised(sponsorshipModelBase);
            var model = AssumeValidModelIsSupplied();

            AssumeProgrammeRepositoryExistsReturns(true);

            var validationResult = Target.Validate(model, a => a.ProgrammeName);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue();
                _ = validationResult.Errors.Count.Should().Be(0);
            }
        }

        [Test(Description = "Fail When DayParts Is Not Supplied")]
        public async Task FailWhenDayPartsIsNotSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.DayParts = null;

            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.DayParts, model);

            _ = validationFailures.Count().Should().BeGreaterThan(0);
        }

        [Test(Description = "Fail When DayParts Is Supplied And Contains Some Invalid DayParts")]
        public async Task FailWhenDayPartsIsSuppliedAndContainsSomeInvalidDayParts()
        {
            var model = AssumeValidModelIsSupplied();
            model.DayParts = CreateValidModels<CreateSponsoredDayPartModel>(3).Append(null);

            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.DayParts, model);

            _ = validationFailures.Count().Should().BeGreaterThan(0);
        }                                                        

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
                                      SponsoredDayPartModelValidation, sponsorshipModelBase);
        }

        protected void AssumeProgrammeRepositoryExistsReturns(bool valueToReturn)
        {
            _ = ProgrammeRepository.Setup(a => a.Exists(It.IsAny<Expression<Func<Programme, bool>>>())).Returns(valueToReturn);
        }

        protected void AssumeSalesAreaRepositoryGetListOfNamesReturns(List<string> dataToReturn)
        {
            _ = SalesAreaRepository.Setup(a => a.GetListOfNames()).Returns(dataToReturn);
        }                                       
    }
}
