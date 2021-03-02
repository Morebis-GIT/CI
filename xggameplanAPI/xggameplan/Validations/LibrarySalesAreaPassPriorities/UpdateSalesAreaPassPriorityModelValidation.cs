using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Extensions;
using xggameplan.Model;

namespace xggameplan.Validations
{
    public class UpdateLibrarySalesAreaPassPriorityModelValidation :
                 LibrarySalesAreaPassPriorityModelValidationBase<UpdateLibrarySalesAreaPassPriorityModel>
    {
        public UpdateLibrarySalesAreaPassPriorityModelValidation(ILibrarySalesAreaPassPrioritiesRepository repository,
                                                                 ISalesAreaRepository salesAreaRepository) : base(repository, salesAreaRepository)
        {
            RuleFor(a => a).MustAsync(ContainUniqueName).When(ContainsValidName).WithName("Name").WithMessage(NameAlreadyExistsMessage);
        }

        private async Task<bool> ContainUniqueName(UpdateLibrarySalesAreaPassPriorityModel model, CancellationToken token)
        {
            return await _librarySalesAreaPassPrioritiesRepository.IsNameUniqueForUpdateAsync(model.Name.ReduceExcessSpace(), model.Uid);
        }
    }
}
