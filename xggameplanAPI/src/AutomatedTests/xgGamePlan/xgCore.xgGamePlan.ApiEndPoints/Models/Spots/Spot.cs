using System;
using NodaTime;
using xgCore.xgGamePlan.ApiEndPoints.Models.Breaks;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Spots
{
    public class Spot
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

        public override bool Equals(object obj)
        {
            if (!(obj is Spot))
            {
                return false;
            }

            return ReferenceEquals(this, obj) || Equals((Spot) obj);
        }

        protected bool Equals(Spot other)
        {
            return
                string.Equals(ExternalCampaignNumber, other.ExternalCampaignNumber, StringComparison.InvariantCulture) &&
                string.Equals(SalesArea, other.SalesArea, StringComparison.InvariantCulture) &&
                string.Equals(GroupCode, other.GroupCode, StringComparison.InvariantCulture) &&
                string.Equals(ExternalSpotRef, other.ExternalSpotRef, StringComparison.InvariantCulture) &&
                StartDateTime.Equals(other.StartDateTime) &&
                EndDateTime.Equals(other.EndDateTime) &&
                SpotLength.Equals(other.SpotLength) &&
                string.Equals(BreakType, other.BreakType, StringComparison.InvariantCulture) &&
                string.Equals(Product, other.Product, StringComparison.InvariantCulture) &&
                string.Equals(Demographic, other.Demographic, StringComparison.InvariantCulture) &&
                ClientPicked == other.ClientPicked &&
                string.Equals(MultipartSpot, other.MultipartSpot, StringComparison.InvariantCulture) &&
                string.Equals(MultipartSpotPosition, other.MultipartSpotPosition, StringComparison.InvariantCulture) &&
                string.Equals(MultipartSpotRef, other.MultipartSpotRef, StringComparison.InvariantCulture) &&
                string.Equals(RequestedPositioninBreak, other.RequestedPositioninBreak, StringComparison.InvariantCulture) &&
                string.Equals(ActualPositioninBreak, other.ActualPositioninBreak, StringComparison.InvariantCulture) &&
                string.Equals(BreakRequest, other.BreakRequest, StringComparison.InvariantCulture) &&
                string.Equals(ExternalBreakNo, other.ExternalBreakNo, StringComparison.InvariantCulture) &&
                Sponsored == other.Sponsored &&
                Preemptable == other.Preemptable &&
                Preemptlevel == other.Preemptlevel &&
                string.Equals(IndustryCode, other.IndustryCode, StringComparison.InvariantCulture) &&
                string.Equals(ClearanceCode, other.ClearanceCode, StringComparison.InvariantCulture);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (ExternalCampaignNumber != null ? StringComparer.InvariantCulture.GetHashCode(ExternalCampaignNumber) : 0);
                hashCode = (hashCode * 397) ^ (SalesArea != null ? StringComparer.InvariantCulture.GetHashCode(SalesArea) : 0);
                hashCode = (hashCode * 397) ^ (GroupCode != null ? StringComparer.InvariantCulture.GetHashCode(GroupCode) : 0);
                hashCode = (hashCode * 397) ^ (ExternalSpotRef != null ? StringComparer.InvariantCulture.GetHashCode(ExternalSpotRef) : 0);
                hashCode = (hashCode * 397) ^ StartDateTime.GetHashCode();
                hashCode = (hashCode * 397) ^ EndDateTime.GetHashCode();
                hashCode = (hashCode * 397) ^ SpotLength.GetHashCode();
                hashCode = (hashCode * 397) ^ (BreakType != null ? StringComparer.InvariantCulture.GetHashCode(BreakType) : 0);
                hashCode = (hashCode * 397) ^ (Product != null ? StringComparer.InvariantCulture.GetHashCode(Product) : 0);
                hashCode = (hashCode * 397) ^ (Demographic != null ? StringComparer.InvariantCulture.GetHashCode(Demographic) : 0);
                hashCode = (hashCode * 397) ^ ClientPicked.GetHashCode();
                hashCode = (hashCode * 397) ^ (MultipartSpot != null ? StringComparer.InvariantCulture.GetHashCode(MultipartSpot) : 0);
                hashCode = (hashCode * 397) ^ (MultipartSpotPosition != null ? StringComparer.InvariantCulture.GetHashCode(MultipartSpotPosition) : 0);
                hashCode = (hashCode * 397) ^ (MultipartSpotRef != null ? StringComparer.InvariantCulture.GetHashCode(MultipartSpotRef) : 0);
                hashCode = (hashCode * 397) ^ (RequestedPositioninBreak != null ? StringComparer.InvariantCulture.GetHashCode(RequestedPositioninBreak) : 0);
                hashCode = (hashCode * 397) ^ (ActualPositioninBreak != null ? StringComparer.InvariantCulture.GetHashCode(ActualPositioninBreak) : 0);
                hashCode = (hashCode * 397) ^ (BreakRequest != null ? StringComparer.InvariantCulture.GetHashCode(BreakRequest) : 0);
                hashCode = (hashCode * 397) ^ (ExternalBreakNo != null ? StringComparer.InvariantCulture.GetHashCode(ExternalBreakNo) : 0);
                hashCode = (hashCode * 397) ^ Sponsored.GetHashCode();
                hashCode = (hashCode * 397) ^ Preemptable.GetHashCode();
                hashCode = (hashCode * 397) ^ Preemptlevel.GetHashCode();
                hashCode = (hashCode * 397) ^ (IndustryCode != null ? StringComparer.InvariantCulture.GetHashCode(IndustryCode) : 0);
                hashCode = (hashCode * 397) ^ (ClearanceCode != null ? StringComparer.InvariantCulture.GetHashCode(ClearanceCode) : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Spot left, Spot right) => Equals(left, right);

        public static bool operator !=(Spot left, Spot right) => !Equals(left, right);

        /// <summary>
        /// Changes spot placement
        /// </summary>
        /// <param name="spotBreak"></param>
        public void MoveTo(Break spotBreak)
        {
            ExternalBreakNo = spotBreak.ExternalBreakRef;
            SalesArea = spotBreak.SalesArea;
            StartDateTime = spotBreak.ScheduledDate;
        }
    }
}
