using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: Sponsorship :: CreateAdvertiserExclusivityModelValidationShould")]
    public class CreateAdvertiserExclusivityModelValidationShould
               : AdvertiserExclusivityModelBaseValidationShould<CreateAdvertiserExclusivityModelValidation, CreateAdvertiserExclusivityModel>
    {
    }
}
