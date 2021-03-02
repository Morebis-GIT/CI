using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: Sponsorship :: CreateSponsorshipItemModelValidationShould")]
    public class CreateSponsorshipItemModelValidationShould
               : SponsorshipItemModelBaseValidationShould<CreateSponsorshipItemModelValidation, CreateSponsorshipItemModel>
    {
    }
}
