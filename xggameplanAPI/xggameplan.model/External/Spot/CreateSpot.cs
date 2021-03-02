using System;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Exceptions;
using NodaTime;
using Raven.Imports.Newtonsoft.Json;
using xggameplan.model;

namespace xggameplan.Model
{
    public class CreateSpot
        : ISelfValidate
    {
        public string ExternalCampaignNumber { get; set; }
        public string SalesArea { get; set; }
        public string GroupCode { get; set; }
        public string ExternalSpotRef { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public Duration SpotLength { get; set; }
        public string BreakType { get; set; }
        public string Product { get; set; }
        public string Demographic { get; set; }
        public bool ClientPicked { get; set; }
        public string MultipartSpot { get; set; }
        public string MultipartSpotPosition { get; set; }
        public string MultipartSpotRef { get; set; }
        public string RequestedPositioninBreak { get; set; }
        public string ActualPositioninBreak { get; set; }
        public string BreakRequest { get; set; }
        public string ExternalBreakNo { get; set; }
        public bool Sponsored { get; set; }
        public bool Preemptable { get; set; }
        public int Preemptlevel { get; set; }
        public string IndustryCode { get; set; }
        public string ClearanceCode { get; set; }

        /// <summary>
        /// Whether spot is booked.
        /// </summary>
        [JsonIgnore]
        public bool IsBooked =>
            !String.IsNullOrWhiteSpace(ExternalBreakNo) &&
            ExternalBreakNo.Equals(Globals.UnplacedBreakString, StringComparison.OrdinalIgnoreCase);

        public bool IsUnplaced =>
            string.IsNullOrWhiteSpace(ExternalBreakNo) ||
            ExternalBreakNo.Equals(Globals.UnplacedBreakString, StringComparison.OrdinalIgnoreCase);

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
                return;
            }

            var value = BreakType.Trim();

            if (!BreakType.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                var guruMeditation = new BreakTypeTooShortException();
                guruMeditation.Data.Add(nameof(ExternalSpotRef), ExternalSpotRef);

                throw guruMeditation;
            }

            if (value.Length < BreakConstants.MinimumBreakTypeLength)
            {
                var guruMeditation = new BreakTypeTooShortException();
                guruMeditation.Data.Add(nameof(ExternalSpotRef), ExternalSpotRef);

                throw guruMeditation;
            }

            // Check that the first two characters are would be long enough.
            // Optimiser uses just the first two character so they must be valid
            // in isolation.
            var prefix = BreakType.Substring(0, 2).Trim();
            if (prefix.Length < BreakConstants.MinimumBreakTypeLength)
            {
                var guruMeditation = new BreakTypePrefixTooShortException();
                guruMeditation.Data.Add(nameof(ExternalSpotRef), ExternalSpotRef);

                throw guruMeditation;
            }
        }
    }
}
