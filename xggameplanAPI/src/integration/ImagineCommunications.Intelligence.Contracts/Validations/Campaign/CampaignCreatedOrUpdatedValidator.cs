using System;
using FluentValidation;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;
using ImagineCommunications.Gameplan.Integration.Contracts.Shared.Enums;
using ImagineCommunications.Gameplan.Integration.Contracts.Validations.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Validations.Campaign
{
    public class CampaignCreatedOrUpdatedValidator : AbstractValidator<ICampaignCreatedOrUpdated>
    {
        public CampaignCreatedOrUpdatedValidator()
        {
            RuleFor(c => c.ExternalId).NotEmpty().WithMessage("External Id must not be null or empty");

            RuleFor(c => c.Name).NotEmpty().WithMessage("Name is missing");

            RuleFor(c => c.DemoGraphic).NotEmpty().WithMessage("Demographic is missing");

            RuleFor(c => c.StartDateTime)
                .NotEmpty().WithMessage("Start Date Time is missing");

            RuleFor(c => c.EndDateTime).NotEmpty().WithMessage("End Date Time is missing");

            RuleFor(c => c.EndDateTime).Must((c, d) => DateTime.Compare(c.StartDateTime.Date, d.Date) <= 0)
                .WithMessage("Start date should not exceed end date");

            RuleFor(c => c.Product).NotEmpty().WithMessage("Product is missing");

            //RuleFor(c => c.TargetRatings).NotEmpty().WithMessage("Target Ratings are missing");

            RuleFor(c => c.IncludeRightSizer).NotEmpty().WithMessage("Include Right Sizer value is missing");

            RuleFor(c => c.DeliveryType)
                .NotEmpty().WithMessage("Delivery type is missing")
                .IsEnumName(typeof(CampaignDeliveryType)).WithMessage("Invalid delivery type");

            RuleFor(c => c.CampaignGroup)
                .MaximumLength(20)
                .When(g => g.CampaignGroup != null)
                .WithMessage("Campaign Group can't be more than 20 characters");

            RuleFor(c => c.CampaignSpotMaxRatings).GreaterThanOrEqualTo(0).WithMessage("Campaign Spot Max Ratings should be greater than or equal to 0");

            RuleFor(c => c.BreakType).NotEmpty().WithMessage("Break Types are missing");

            RuleForEach(c => c.BreakType).NotEmpty().WithMessage("Break Type is empty");

            RuleFor(c => c.SalesAreaCampaignTarget).NotEmpty().WithMessage("Sales Area Campaign Targets are missing");

            RuleForEach(c => c.SalesAreaCampaignTarget)
                .SetValidator(s => new SalesAreaCampaignTargetValidator())
                .When(ct => ct.SalesAreaCampaignTarget != null);

            RuleForEach(c => c.TimeRestrictions)
                .SetValidator(tr => new TimeRestrictionValidator())
                .When(tr => tr.TimeRestrictions != null);

            RuleForEach(c => c.ProgrammeRestrictions)
                .SetValidator(pr => new ProgrammeRestrictionValidator())
                .When(pr => pr.ProgrammeRestrictions != null);
        }
    }
}
