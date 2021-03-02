using System;
using System.Linq;
using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Clash
{
    public class BulkClashCreatedOrUpdatedValidator : AbstractValidator<IBulkClashCreatedOrUpdated>
    {
        public BulkClashCreatedOrUpdatedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
            RuleForEach(r => r.Data).SetValidator(x => new ClashCreatedOrUpdatedValidator());

            RuleFor(r => r.Data).Custom((clashes, context) =>
            {
                var duplicateRefs = clashes
                    .GroupBy(x => x.Externalref, StringComparer.OrdinalIgnoreCase)
                    .Where(g => g.Count() > 1)
                    .Select(x => x.Key)
                    .ToList();

                if (duplicateRefs.Any())
                {
                    context.AddFailure("DuplicateExternalRefs", $"Duplicate externalRefs for multiple objects: {string.Join(", ", duplicateRefs)}");
                }
            });
        }
    }
}
