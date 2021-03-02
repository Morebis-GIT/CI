using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Extensions;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class CreateLibrarySalesAreaPassPriorityModelValidation :
                 LibrarySalesAreaPassPriorityModelValidationBase<CreateLibrarySalesAreaPassPriorityModel>
    {
        public CreateLibrarySalesAreaPassPriorityModelValidation(ILibrarySalesAreaPassPrioritiesRepository repository,
                                                                 ISalesAreaRepository salesAreaRepository) : base(repository, salesAreaRepository)
        {
            RuleFor(a => a.Name).MustAsync(ContainUniqueName).When(ContainsValidName).WithMessage(NameAlreadyExistsMessage);
        }

        private async Task<bool> ContainUniqueName(string name, CancellationToken token)
        {
            return await _librarySalesAreaPassPrioritiesRepository.IsNameUniqueForCreateAsync(name.ReduceExcessSpace());
        }
    }
}
