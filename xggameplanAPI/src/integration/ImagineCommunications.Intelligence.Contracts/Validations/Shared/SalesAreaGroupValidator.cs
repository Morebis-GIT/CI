using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared
{
    public class SalesAreaGroupValidator : AbstractValidator<SalesAreaGroup>
    {
        public SalesAreaGroupValidator()
        {
            RuleFor(sg => sg.GroupName).NotEmpty().WithMessage("SalesArea Group Name is missing");

            RuleFor(sg => sg.SalesAreas).NotEmpty().WithMessage("SalesArea name list is empty");

            RuleForEach(sg => sg.SalesAreas).NotEmpty().WithMessage("SalesArea name can not be empty");
        }
    }
}
