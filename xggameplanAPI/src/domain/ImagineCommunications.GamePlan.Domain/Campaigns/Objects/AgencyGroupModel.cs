using System;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class AgencyGroupModel : IEquatable<AgencyGroupModel>
    {
        public string ShortName { get; set; }
        public string Code { get; set; }

        public bool Equals(AgencyGroupModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ShortName == other.ShortName && Code == other.Code;
        }

        public override bool Equals(object obj)
        {
            if (obj is AgencyGroupModel agm)
            {
                return Equals(agm);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ShortName?.GetHashCode() ?? 0) * 397) ^ (Code?.GetHashCode() ?? 0);
            }
        }
    }
}
