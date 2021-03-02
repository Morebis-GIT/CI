using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentValidation;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: Sponsorship :: SponsoredDayPartModelValidationShould")]
    public class SponsoredDayPartModelValidationShould
               : SponsorshipModelValidationTestBase<SponsoredDayPartModelValidation, SponsoredDayPartModelBase>
    {
        [SetUp]
        public async Task BeforeEach()
        {
            AssumeTargetIsInitialised();
        }

        [TearDown]
        public async Task AfterEach()
        {
            CleanUpTarget();
        }

        [Test(Description = "Fail When StartTime Is Later Than EndTime")]
        public async Task FailWhenStartTimeIsLaterThanEndTime()
        {
            var model = AssumeValidModelIsSupplied();
            model.StartTime = new TimeSpan(4, 0, 0);
            model.EndTime = new TimeSpan(6, 30, 0);

            var validationResult = Target.Validate(model, a => a.EndTime);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().BePositive(becauseArgs: null);
            }
        }

        [Test(Description = "Pass When StartTime Is Earlier Than EndTime")]
        public async Task PassWhenStartTimeIsEarlierThanEndTime()
        {
            var model = AssumeValidModelIsSupplied();
            model.StartTime = new TimeSpan(6, 0, 0);
            model.EndTime = new TimeSpan(6, 30, 0);

            var validationResult = Target.Validate(model, a => a.EndTime);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().Be(0, becauseArgs: null);
            }
        }

        [Test(Description = "Pass When EndTime Is Later Than StartTime")]
        public async Task PassWhenEndTimeIsLaterThanStartTime()
        {
            var model = AssumeValidModelIsSupplied();
            model.StartTime = new TimeSpan(23, 0, 0);
            model.EndTime = new TimeSpan(4, 30, 0);

            var validationResult = Target.Validate(model, a => a.EndTime);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().Be(0, becauseArgs: null);
            }
        }

        [Test(Description = "Fail When EndTime Is Earlier Than StartTime")]
        public async Task FailWhenEndTimeIsEarlierThanStartTime()
        {
            var model = AssumeValidModelIsSupplied();
            model.StartTime = new TimeSpan(9, 0, 0);
            model.EndTime = new TimeSpan(8, 0, 0);

            var validationResult = Target.Validate(model, a => a.EndTime);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().BePositive(becauseArgs: null);
            }
        }

        [Test(Description = "Fail When DaysOfWeek Is Null")]
        public async Task FailWhenDaysOfWeekIsNull()
        {
            var model = AssumeValidModelIsSupplied();
            model.DaysOfWeek = null;

            var validationResult = Target.Validate(model, a => a.DaysOfWeek);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().BePositive(becauseArgs: null);
            }
        }

        [Test(Description = "Fail When DaysOfWeek Is Empty")]
        public async Task FailWhenDaysOfWeekIsEmpty()
        {
            var model = AssumeValidModelIsSupplied();
            model.DaysOfWeek = null;

            var validationResult = Target.Validate(model, a => a.DaysOfWeek);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().BePositive(becauseArgs: null);
            }
        }

        [Test(Description = "Fail When DaysOfWeek Contains Only WhiteSpaces")]
        public async Task FailWhenDaysOfWeekContainsOnlyWhiteSpaces()
        {
            var model = AssumeValidModelIsSupplied();
            model.DaysOfWeek = new string[] { };

            var validationResult = Target.Validate(model, a => a.DaysOfWeek);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().BePositive(becauseArgs: null);
            }
        }

        [Test(Description = "Fail When DaysOfWeek Contains Duplicates")]
        public async Task FailWhenDaysOfWeekContainsDuplicates()
        {
            var model = AssumeValidModelIsSupplied();
            model.DaysOfWeek = new string[] { "Monday", "Mon" };

            var validationResult = Target.Validate(model, a => a.DaysOfWeek);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().BePositive(becauseArgs: null);
            }
        }

        [Test(Description = "Fail When DaysOfWeek Format Is InCorrect")]
        public async Task FailWhenDaysOfWeekFormatIsInCorrect()
        {
            var model = AssumeValidModelIsSupplied();
            model.DaysOfWeek = new string[] { "foo", "bar", "Wed", "Monday" };

            var validationResult = Target.Validate(model, a => a.DaysOfWeek);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().BePositive(becauseArgs: null);
            }
        }

        [Test(Description = "Fail When DaysOfWeek are more than 7 days")]
        public async Task FailWhenDaysOfWeekAreMoreThanSevenDays()
        {
            var model = AssumeValidModelIsSupplied();
            model.DaysOfWeek = new string[] { "Monday", "tue", "fri", "wed", "Sat", "thu", "Sunday", "Friday" };

            var validationResult = Target.Validate(model, a => a.DaysOfWeek);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeFalse(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().BePositive(becauseArgs: null);
            }
        }

        [Test(Description = "Pass When DaysOfWeek Format Is Correct")]
        public async Task PassWhenDaysOfWeekFormatIsCorrect()
        {
            var model = AssumeValidModelIsSupplied();
            model.DaysOfWeek = new string[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

            var validationResult = Target.Validate(model, a => a.DaysOfWeek);

            using (new AssertionScope())
            {
                _ = validationResult.IsValid.Should().BeTrue(becauseArgs: null);
                _ = validationResult.Errors.Count.Should().Be(0, becauseArgs: null);
            }
        }
    }
}
