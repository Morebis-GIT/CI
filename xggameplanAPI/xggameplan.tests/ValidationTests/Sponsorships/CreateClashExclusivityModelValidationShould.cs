using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: Sponsorship :: CreateClashExclusivityModelValidationShould")]
    public class CreateClashExclusivityModelValidationShould
               : ClashExclusivityModelBaseValidationShould<CreateClashExclusivityModelValidation, CreateClashExclusivityModel>
    {
    }
}
