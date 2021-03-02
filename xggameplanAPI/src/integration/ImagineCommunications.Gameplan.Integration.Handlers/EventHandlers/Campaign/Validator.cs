using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Campaign;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Campaign;
using ImagineCommunications.Gameplan.Integration.Handlers.Common;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using xggameplan.common.Utilities;

namespace ImagineCommunications.Gameplan.Integration.Handlers.EventHandlers.Campaign
{
    internal sealed class Validator
    {
        public static void ValidateCampaign(ICampaignCreatedOrUpdated campaign, ValidateContext context)
        {
            var isValidIncludeRightRizer = EnumUtilities.ToDescriptionList<IncludeRightSizer>()
                .Select(value => value.ToUpperInvariant())
                .Contains(campaign.IncludeRightSizer.ToUpperInvariant());

            if (!isValidIncludeRightRizer)
            {
                throw new DataSyncException(DataSyncErrorCode.InvalidRightSizer, $"Invalid right sizer value: ${campaign.IncludeRightSizer}");
            }

            ValidateCampaignPassPriority(campaign.IncludeOptimisation, campaign.CampaignPassPriority);

            if (!context.ExistingDemographics.Contains(campaign.DemoGraphic))
            {
                throw new DataSyncException(DataSyncErrorCode.DemographicNotFound, $"Invalid Demographic in Campaign: {campaign.DemoGraphic}");
            }

            if (!context.Products.Contains(campaign.Product))
            {
                throw new DataSyncException(DataSyncErrorCode.ProductNotFound, $"Invalid Product in Campaign: {campaign.Product}");
            }

            if (campaign.BreakType?.Any() == true)
            {
                CommonValidations.ValidateBreakType(context.BreakTypes, campaign.BreakType);
            }

            if (campaign.SalesAreaCampaignTarget != null && campaign.SalesAreaCampaignTarget.Any())
            {
                var salesAreas = campaign.SalesAreaCampaignTarget
                    .Where(s => s.SalesAreaGroup?.SalesAreas != null && s.SalesAreaGroup.SalesAreas.Any())
                    .SelectMany(s => s.SalesAreaGroup.SalesAreas)
                    .ToArray();

                if (salesAreas.Any())
                {
                    CommonValidations.ValidateSalesArea(context.ExistingSalesAreas, salesAreas);
                }
            }

            if (campaign.ProgrammeRestrictions != null && campaign.ProgrammeRestrictions.Any())
            {
                var salesAreas = campaign.ProgrammeRestrictions.Where(p => p.SalesAreas != null && p.SalesAreas.Any())
                    .SelectMany(p => p.SalesAreas)
                    .ToArray();

                if (salesAreas.Any())
                {
                    CommonValidations.ValidateSalesArea(context.ExistingSalesAreas, salesAreas);
                }

                var categoryNames = campaign.ProgrammeRestrictions
                    .Where(p => p.IsCategoryOrProgramme.Equals(CategoryOrProgramme.C.ToString(), StringComparison.OrdinalIgnoreCase))
                    .SelectMany(p => p.CategoryOrProgramme)
                    .ToArray();

                if (categoryNames.Any())
                {
                    CommonValidations.ValidateProgrammeCategory(context.ProgrammeCategories, categoryNames);
                }
            }

            if (campaign.TimeRestrictions != null && campaign.TimeRestrictions.Any())
            {
                var salesAreas = campaign.TimeRestrictions
                    .Where(p => p.SalesAreas != null && p.SalesAreas.Any())
                    .SelectMany(p => p.SalesAreas)
                    .ToArray();

                if (salesAreas.Any())
                {
                    CommonValidations.ValidateSalesArea(context.ExistingSalesAreas, salesAreas);
                }
            }

            if (campaign.BookingPositionGroups?.Any() == true)
            {
                ValidateBookingPositionGroups(campaign.BookingPositionGroups, context);

                var salesAreas = campaign.BookingPositionGroups
                    .Where(p => p.SalesAreas != null && p.SalesAreas.Any())
                    .SelectMany(p => p.SalesAreas)
                    .ToArray();

                if (salesAreas.Any())
                {
                    CommonValidations.ValidateSalesArea(context.ExistingSalesAreas, salesAreas);
                }
            }
        }

        private static void ValidateBookingPositionGroups(
            IReadOnlyCollection<CampaignBookingPositionGroup> bookingPositionGroups, ValidateContext validateContext)
        {
            if (!bookingPositionGroups.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.BookingPositionGroupNotFound,
                    "Empty booking position groups list");
            }

            var groupIdsToValidate = bookingPositionGroups
                .Select(g => g.GroupId)
                .ToList();

            var invalidGroupIds = groupIdsToValidate
                .Except(validateContext.BookingPositionGroups)
                .ToList();

            if (invalidGroupIds.Any())
            {
                throw new DataSyncException(DataSyncErrorCode.BookingPositionGroupNotFound,
                    "Invalid booking position group ids: " + string.Join(",", invalidGroupIds.ToList()));
            }
        }

        private static void ValidateCampaignPassPriority(bool includeOptimisation, int campaignPassPriority)
        {
            var minimumCampaignPassPriority = (int)PassPriorityType.Exclude;
            var maximumCampaignPassPriority = (int)PassPriorityType.Include;

            var campaignPassPriorityErrorMessage = "Invalid CampaignPassPriority, CampaignPassPriority must be '0' when IncludeOptimization is false.";

            if (includeOptimisation)
            {
                campaignPassPriorityErrorMessage = $"Invalid CampaignPassPriority, Accepted values from {minimumCampaignPassPriority} to {maximumCampaignPassPriority}";
            }

            if (!(campaignPassPriority >= minimumCampaignPassPriority && campaignPassPriority <= maximumCampaignPassPriority))
            {
                throw new DataSyncException(DataSyncErrorCode.InvalidCampaignPassPriority, campaignPassPriorityErrorMessage);
            }
        }
    }
}
