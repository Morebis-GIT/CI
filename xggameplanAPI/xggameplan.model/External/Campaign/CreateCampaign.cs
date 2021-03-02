using System;
using System.Collections.Generic;
using System.ComponentModel;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Exceptions;
using xggameplan.common.Extensions;
using xggameplan.model;
using xggameplan.model.External.Campaign;
using IncludeRightSizerEnum = ImagineCommunications.GamePlan.Domain.Campaigns.IncludeRightSizer;

namespace xggameplan.Model
{
    public class CreateCampaign
        : ISelfValidate
    {
        public string ExternalId { get; set; }
        public string Name { get; set; }
        public string DemoGraphic { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Product { get; set; }
        public double RevenueBudget { get; set; }
        public decimal? TargetRatings { get; set; }
        public decimal ActualRatings { get; set; }
        public string CampaignGroup { get; set; }

        /// <summary>
        /// This will be true if the rating is a percentage and false if the
        /// rating is a value.
        /// </summary>
        public bool IsPercentage { get; set; }

        public string Status { get; set; }
        public string BusinessType { get; set; }
        public string DeliveryType { get; set; }

        [DefaultValue(true)]
        public bool IncludeOptimisation { get; set; }

        public bool TargetZeroRatedBreaks { get; set; }

        [DefaultValue(true)]
        public bool InefficientSpotRemoval { get; set; }

        public string IncludeRightSizer { get; set; }
        public string ExpectedClearanceCode { get; set; }
        public int CampaignPassPriority { get; set; }
        public List<string> BreakType { get; set; }
        public int CampaignSpotMaxRatings { get; set; }

        public List<SalesAreaCampaignTargetViewModel> SalesAreaCampaignTarget { get; set; }

        public List<TimeRestriction> TimeRestrictions { get; set; }

        public List<ProgrammeRestriction> ProgrammeRestrictions { get; set; }

        public List<CampaignProgrammeModel> ProgrammesList { get; set; }

        public CreateCampaign()
        {
            IncludeOptimisation = true;
            InefficientSpotRemoval = true;
            IncludeRightSizer = IncludeRightSizerEnum.CampaignLevel.GetDescription();
        }

        /// <summary>
        /// Validates this instance follows business rules for the
        /// object to be created. If the validation fails an exception will be
        /// thrown.
        /// </summary>
        public void Validate()
        {
            ValidateBreakType();
        }

        private void ValidateBreakType()
        {
            if (BreakType is null)
            {
                var guruMeditation = new BreakTypeNullException();
                guruMeditation.Data.Add(nameof(ExternalId), ExternalId);

                throw guruMeditation;
            }

            foreach (string bt in BreakType)
            {
                if (String.IsNullOrWhiteSpace(bt))
                {
                    var guruMeditation = new BreakTypeNullException();
                    guruMeditation.Data.Add(nameof(ExternalId), ExternalId);

                    throw guruMeditation;
                }

                if (bt.Length < BreakConstants.MinimumBreakTypeLength)
                {
                    var guruMeditation = new BreakTypeTooShortException();
                    guruMeditation.Data.Add(nameof(ExternalId), ExternalId);

                    throw guruMeditation;
                }

                // Check that the first two characters are would be long enough.
                // Optimiser uses just the first two character so they must be valid
                // in isolation.
                var prefix = bt.Substring(0, 2).Trim();
                if (prefix.Length < BreakConstants.MinimumBreakTypeLength)
                {
                    var guruMeditation = new BreakTypePrefixTooShortException();
                    guruMeditation.Data.Add(nameof(ExternalId), ExternalId);

                    throw guruMeditation;
                }
            }
        }
    }
}
