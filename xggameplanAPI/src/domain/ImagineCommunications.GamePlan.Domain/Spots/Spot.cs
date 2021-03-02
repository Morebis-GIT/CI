using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic;
using NodaTime;
using Raven.Imports.Newtonsoft.Json;
using static ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions.BookingPosition;

namespace ImagineCommunications.GamePlan.Domain.Spots
{
    /// <summary>
    /// Represents a commercial for a product.
    /// </summary>
    [DebuggerDisplay("{ExternalSpotRef}")]
    public class Spot : ICloneable
    {
        [JsonIgnore]
        private static char[] MultipartSpotReferenceSeparator => new char[] { ',' };

        public Guid Uid { get; set; }

        public int CustomId { get; set; }

        public string ExternalCampaignNumber { get; set; }
        public string SalesArea { get; set; }
        public string GroupCode { get; set; }

        /// <summary>
        /// <para>A unique spot identifier.</para>
        /// <para>
        /// For TopTail multipart spots, this is used to link the two spots. The
        /// value of this property is placed in the other spot's
        /// <see cref="MultipartSpotRef"/> property.
        /// </para>
        /// </summary>
        public string ExternalSpotRef { get; set; }

        /// <summary>
        /// <para>
        /// The earliest time that the spot can be placed in the programme break.
        /// </para>
        /// <para>
        /// Typically it is the start time of the programme. Occasionally it can
        /// be after the programme start time.
        /// </para>
        /// </summary>
        /// <example>
        /// If the advertiser wants the commercial to be shown in the last hour
        /// of the movie then it will be the programme end time minus one hour.
        /// </example>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// The latest time the Spot can be placed in the programme break.
        /// </summary>
        /// <value>The end date time of the Spot.</value>
        public DateTime EndDateTime { get; set; }

        /// <summary>Gets or sets the length of the spot.</summary>
        /// <value>The length of the spot.</value>
        public Duration SpotLength { get; set; }

        public string BreakType { get; set; }

        /// <summary>
        /// The external identifier of a Product.
        /// </summary>
        public string Product { get; set; }

        public string Demographic { get; set; }

        /// <summary>
        /// <para>
        /// Will be <c>true</c> for handheld spots, and <c>false</c> for dynamic spots.
        /// </para>
        /// <para>
        /// The Smooth process will only consider handheld (client picked)
        /// spots. Dynamic spots are never smoothed.
        /// </para>
        /// </summary>
        public bool ClientPicked { get; set; }

        /// <summary>
        /// Defines what type of multipart spot this belongs to. Set to "TT" for
        /// TopTail pairs. Set to null if this spot is not part of a multipart pair.
        /// </summary>
        public string MultipartSpot { get; set; }

        /// <summary>
        /// Indicates whether a spot is part of a multipart spot.
        /// </summary>
        /// <returns>
        /// Returns <see langword="true"/> is the spot is part of a multipart
        /// spot, otherwise returns <see langword="false"/>.
        /// </returns>
        [JsonIgnore]
        public bool IsMultipartSpot => !String.IsNullOrWhiteSpace(MultipartSpot);

        /// <summary>
        /// Defines which part of the multipart spot this is. For TopTail pairs
        /// this will be either "TOP" or "TAIL". Set to null if this spot is not
        /// part of a multipart pair.
        /// </summary>
        public string MultipartSpotPosition { get; set; }

        /// <summary>
        /// For TopTail multipart spots, this must be the same value as the
        /// <see cref="ExternalSpotRef"/> in the other <see cref="Spot"/> of the
        /// spot pair. This links them together. Set to null if this spot is not
        /// part of a multipart pair.
        /// </summary>
        public string MultipartSpotRef { get; set; }

        public string RequestedPositioninBreak { get; set; }
        public string ActualPositioninBreak { get; set; }

        /// <summary>
        /// A spot is placed in the break which has its
        /// <see cref="Break.ExternalBreakRef"/> property set to the same value
        /// as the spot's <see cref="BreakRequest"/> property.
        /// </summary>
        public string BreakRequest { get; set; }

        /// <summary>
        /// The <see cref="Break.ExternalBreakRef"/> of the break the spot is
        /// placed in.
        /// </summary>
        public string ExternalBreakNo { get; set; }
        public bool Sponsored { get; set; }
        public bool Preemptable { get; set; }
        public int Preemptlevel { get; set; }
        public string IndustryCode { get; set; }
        public string ClearanceCode { get; set; }
        public int BookingPosition { get; set; } = NoDefaultPosition;
        public decimal NominalPrice { get; set; }

        public object Clone() => MemberwiseClone();

        /// <summary>
        /// A list of multipart spot references.
        /// </summary>
        [JsonIgnore]
        public List<string> MultipartSpotRefs => String.IsNullOrEmpty(MultipartSpotRef)
            ? null
            : MultipartSpotRef.Split(MultipartSpotReferenceSeparator).ToList();

        [JsonIgnore]
        public bool IsUnplaced => string.IsNullOrWhiteSpace(ExternalBreakNo) ||
                                  ExternalBreakNo.Equals(Globals.UnplacedBreakString,
                                      StringComparison.InvariantCultureIgnoreCase);

        public void SetUnplaced() => ExternalBreakNo = Globals.UnplacedBreakString;
    }
}
