using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.DayPartGroups;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.DayPartGroups
{
    public class StandardDayPartGroupCreated : IStandardDayPartGroupCreated
    {
        public int GroupId { get; }
        public string SalesArea { get; }
        public string Demographic { get; }
        public bool Optimizer { get; }
        public bool Policy { get; }
        public bool RatingReplacement { get; }
        public List<StandardDayPartSplit> Splits { get; }

        public StandardDayPartGroupCreated(int groupId, string salesArea, string demographic, bool optimizer, bool policy, bool ratingReplacement, List<StandardDayPartSplit> splits)
        {
            GroupId = groupId;
            SalesArea = salesArea;
            Demographic = demographic;
            Optimizer = optimizer;
            Policy = policy;
            RatingReplacement = ratingReplacement;
            Splits = splits;
        }
    }
}
