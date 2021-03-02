using System;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class CampaignPayback : ICloneable, IEquatable<CampaignPayback>
    {
        public string Name { get; set; }
        public double Amount { get; set; }

        /// <summary>
        /// Gets computed hash for sorting
        /// </summary>
        /// <returns></returns>
        public string GetSortHash() => Name + Amount;

        public object Clone() => MemberwiseClone();

        public bool Equals(CampaignPayback other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name && double.Equals(Amount, other.Amount);
        }

        public override bool Equals(object obj)
        {
            if (obj is CampaignPayback agm)
            {
                return Equals(agm);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name?.GetHashCode() ?? 0) * 397) ^ Amount.GetHashCode();
            }
        }
    }
}
