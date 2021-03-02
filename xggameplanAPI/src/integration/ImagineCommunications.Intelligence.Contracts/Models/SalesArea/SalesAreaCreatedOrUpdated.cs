using System;
using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.SalesArea;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.SalesArea
{
    public class SalesAreaCreatedOrUpdated : ISalesAreaCreatedOrUpdated
    {
        public int CustomId { get; }
        public string Name { get; }
        public string ShortName { get; }
        public string CurrencyCode { get; }
        public string BaseDemographic1 { get; }
        public string BaseDemographic2 { get; }
        public string TargetAreaName { get; }
        public List<string> ChannelGroup { get; }
        public TimeSpan StartOffset { get; }
        public TimeSpan DayDuration { get; }
        public List<SalesAreaDemographic> Demographics { get; }

        public SalesAreaCreatedOrUpdated(int customId, string name, string shortName, string currencyCode, string baseDemographic1, string baseDemographic2,
            string targetAreaName, List<string> channelGroup, TimeSpan startOffset, TimeSpan dayDuration, List<SalesAreaDemographic> demographics)
        {
            CustomId = customId;
            Name = name;
            ShortName = shortName;
            CurrencyCode = currencyCode;
            BaseDemographic1 = baseDemographic1;
            BaseDemographic2 = baseDemographic2;
            TargetAreaName = targetAreaName;
            ChannelGroup = channelGroup;
            StartOffset = startOffset;
            DayDuration = dayDuration;
            Demographics = demographics;
        }
    }
}
