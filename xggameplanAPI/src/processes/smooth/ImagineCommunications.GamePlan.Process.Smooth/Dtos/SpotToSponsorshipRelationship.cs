using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    public struct SpotToSponsorshipRelationship
    {
        public bool IsSponsorSpot { get; }
        public SponsorshipApplicability Applicability { get; }
        public SponsorshipCompetitorType CompetitorType { get; }
        public SponsorshipRestrictionType RestrictionType { get; }
        public SponsorshipCalculationType CalculationType { get; }

        public SpotToSponsorshipRelationship(
            bool isSponsorSpot,
            SponsorshipApplicability applicability,
            SponsorshipCompetitorType competitorType,
            SponsorshipRestrictionType restrictionType,
            SponsorshipCalculationType calculationType)
        {
            IsSponsorSpot = isSponsorSpot;
            Applicability = applicability;
            CompetitorType = competitorType;
            RestrictionType = restrictionType;
            CalculationType = calculationType;
        }

        public override bool Equals(object obj)
        {
            return obj is SpotToSponsorshipRelationship other &&
                   IsSponsorSpot == other.IsSponsorSpot &&
                   Applicability == other.Applicability &&
                   CompetitorType == other.CompetitorType &&
                   RestrictionType == other.RestrictionType &&
                   CalculationType == other.CalculationType;
        }

        public override int GetHashCode()
        {
            int hashCode = 1516578759;
            hashCode = (hashCode * -1521134295) + IsSponsorSpot.GetHashCode();
            hashCode = (hashCode * -1521134295) + Applicability.GetHashCode();
            hashCode = (hashCode * -1521134295) + CompetitorType.GetHashCode();
            hashCode = (hashCode * -1521134295) + RestrictionType.GetHashCode();
            return hashCode;
        }

        public void Deconstruct(out bool isSponsorSpot, out SponsorshipApplicability applicability, out SponsorshipCompetitorType competitorType, out SponsorshipRestrictionType restrictionType, out SponsorshipCalculationType calculationType)
        {
            isSponsorSpot = IsSponsorSpot;
            applicability = Applicability;
            competitorType = CompetitorType;
            restrictionType = RestrictionType;
            calculationType = CalculationType;
        }

        public static implicit operator (bool isSponsorSpot, SponsorshipApplicability applicability, SponsorshipCompetitorType competitorType, SponsorshipRestrictionType restrictionType, SponsorshipCalculationType calculationType)(SpotToSponsorshipRelationship value)
        {
            return (value.IsSponsorSpot, value.Applicability, value.CompetitorType, value.RestrictionType, value.CalculationType);
        }

        public static implicit operator SpotToSponsorshipRelationship((bool isSponsorSpot, SponsorshipApplicability applicability, SponsorshipCompetitorType competitorType, SponsorshipRestrictionType restrictionType, SponsorshipCalculationType calculationType) value)
        {
            return new SpotToSponsorshipRelationship(value.isSponsorSpot, value.applicability, value.competitorType, value.restrictionType, value.calculationType);
        }

        public static bool operator ==(SpotToSponsorshipRelationship left, SpotToSponsorshipRelationship right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SpotToSponsorshipRelationship left, SpotToSponsorshipRelationship right)
        {
            return !(left == right);
        }
    }
}
