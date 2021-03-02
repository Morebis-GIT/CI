﻿using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Universe;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Universe
{
    public class BulkUniverseCreatedValidator : AbstractValidator<IBulkUniverseCreated>
    {
        public BulkUniverseCreatedValidator()
        {
            RuleFor(r => r.Data).NotEmpty();
            RuleForEach(r => r.Data).SetValidator(x => new UniverseCreatedValidator());
        }
    }
}