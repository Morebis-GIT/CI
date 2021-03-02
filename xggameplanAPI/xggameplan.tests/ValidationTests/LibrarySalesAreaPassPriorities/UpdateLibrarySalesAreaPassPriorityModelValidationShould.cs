using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: LibrarySalesAreaPassPriorities :: UpdateLibrarySalesAreaPassPriorityModelValidationShould")]
    public class UpdateLibrarySalesAreaPassPriorityModelValidationShould :
                 LibrarySalesAreaPassPriorityModelValidationTestBase<UpdateLibrarySalesAreaPassPriorityModelValidation,
                                                                     UpdateLibrarySalesAreaPassPriorityModel>
    {
        [Test]
        public async Task UseRepositoryIsNameUniqueForUpdateToCheckIfNameAlreadyExistsInTheLibrary()
        {
            var model = CreateValidModel();
            AssumeRepositoryIsSetupToReturnNameIsNotUnique();

            var validationResult = _target.Validate(model);

            _librarySalesAreaPassPrioritiesRepositoryMock.Verify(a => a.IsNameUniqueForUpdateAsync(It.IsAny<string>(), It.IsAny<Guid>()));
        }

        protected override void AssumeRepositoryIsNameUniqueIsSetupToReturn(bool valueToReturn)
        {
            _ = _librarySalesAreaPassPrioritiesRepositoryMock
            .Setup(e => e.IsNameUniqueForUpdateAsync(It.IsAny<string>(), It.IsAny<Guid>())).ReturnsAsync(valueToReturn);
        }
    }
}
