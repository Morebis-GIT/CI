using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.ProgrammeClassification;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.ProgrammeClassification
{
    public class BulkProgrammeClassificationCreatedValidator : AbstractValidator<IBulkProgrammeClassificationCreated>
    {
        public BulkProgrammeClassificationCreatedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
            RuleForEach(r => r.Data).SetValidator(x => new ProgrammeClassificationCreatedValidator());

            RuleFor(r => r.Data)
                .Must(x => x.IsUnique(u => u.Uid))
                .WithErrorCode("Uid must be unique");

            RuleFor(r => r.Data)
                .Must(x => x.IsUnique(u => u.Code))
                .WithErrorCode("Code must be unique");

            RuleFor(r => r.Data)
                .Must(x => x.IsUnique(u => u.Description))
                .WithErrorCode("Description must be unique");
        }
    }
}
