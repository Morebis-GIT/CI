using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using xggameplan.Model;
using xggameplan.Validations;

namespace xggameplan.tests.ValidationTests
{
    [TestFixture(Category = "Validations :: LibrarySalesAreaPassPriorities :: CreateLibrarySalesAreaPassPriorityModelValidatorShould")]
    public class CreateLibrarySalesAreaPassPriorityModelValidatorShould :
                LibrarySalesAreaPassPriorityModelValidatorTestBase<CreateLibrarySalesAreaPassPriorityModelValidator,
                                                                     CreateLibrarySalesAreaPassPriorityModel>
    {
        [Test]
        public async Task UseRepositoryIsNameUniqueForCreateToCheckIfNameAlreadyExistsInTheLibrary()
        {
            var model = CreateValidModel();
            AssumeRepositoryIsSetupToReturnNameIsNotUnique();

            var validationResult = _target.Validate(model);

            _librarySalesAreaPassPrioritiesRepositoryMock.Verify(a => a.IsNameUniqueForCreateAsync(It.IsAny<string>()));
        }

        protected override void AssumeRepositoryIsNameUniqueIsSetupToReturn(bool valueToReturn)
        {
            _ = _librarySalesAreaPassPrioritiesRepositoryMock
            .Setup(e => e.IsNameUniqueForCreateAsync(It.IsAny<string>())).ReturnsAsync(valueToReturn);
        }

        protected override void AssumeTargetIsInitialised()
        {
            var modelValidator = new CreateLibrarySalesAreaPassPriorityModelValidation(_librarySalesAreaPassPrioritiesRepositoryMock.Object, _salesAreaRepositoryMock.Object);
            _target = new CreateLibrarySalesAreaPassPriorityModelValidator(modelValidator);
        }
    }
}
