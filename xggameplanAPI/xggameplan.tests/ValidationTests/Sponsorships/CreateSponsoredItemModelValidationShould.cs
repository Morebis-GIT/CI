using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation.TestHelper;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: Sponsorship :: CreateSponsoredItemModelValidationShould")]
    public class CreateSponsoredItemModelValidationShould
               : SponsoredItemModelBaseValidationShould<CreateSponsoredItemModelValidation, CreateSponsoredItemModel>
    {
        [Test(Description = "Fail When SponsorshipItems Is Not Supplied")]
        public async Task FailWhenSponsorshipItemsIsNotSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.SponsorshipItems = null;

            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.SponsorshipItems, model);

            _ = validationFailures.Should().HaveCountGreaterThan(0);
        }

        [Test(Description = "Fail When SponsorshipItems Is Supplied And Contains Some Invalid SponsorshipItems")]
        public async Task FailWhenSponsorshipItemsIsSuppliedAndContainsSomeInvalidSponsorshipItems()
        {
            var model = AssumeValidModelIsSupplied();
            model.SponsorshipItems = CreateValidModels<CreateSponsorshipItemModel>(3).Append(null);

            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.SponsorshipItems, model);

            _ = validationFailures.Count().Should().BeGreaterThan(0);
        }

        [Test(Description = "Fail When Neither AdvertiserExclusivities Nor ClashExclusivities Is Supplied")]
        public async Task FailWhenNeitherAdvertiserExclusivitiesNorClashExclusivitiesIsSupplied()
        {
            var model = AssumeValidModelIsSupplied();
            model.AdvertiserExclusivities = null;
            model.ClashExclusivities = null;

            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.AdvertiserExclusivities, model);

            _ = validationFailures.Count().Should().BeGreaterThan(0);
        }

        [Test(Description = "Fail When AdvertiserExclusivities And ClashExclusivities Are Supplied With Different RestrictionType")]
        public async Task FailWhenAdvertiserExclusivitiesAndClashExclusivitiesAreSuppliedWithDifferentRestrictionType()
        {
            var model = AssumeValidModelIsSupplied();
            SetupAdvertiserExclusivitiesAndClashExclusivitiesWithDifferentRestrictionType(model);

            var validationFailures = Target.ShouldHaveValidationErrorFor(a => a.AdvertiserExclusivities, model);

            _ = validationFailures.Count().Should().BeGreaterThan(0);
        }

        [Test(Description = "Pass When AdvertiserExclusivities And ClashExclusivities Are Supplied With Same RestrictionType")]
        public async Task PassWhenAdvertiserExclusivitiesAndClashExclusivitiesAreSuppliedWithSameRestrictionType()
        {
            var model = AssumeValidModelIsSupplied();
            SetupAdvertiserExclusivitiesAndClashExclusivitiesWithSameRestrictionType(model);

            Target.ShouldNotHaveValidationErrorFor(a => a.AdvertiserExclusivities, model);
        }
    }
}
