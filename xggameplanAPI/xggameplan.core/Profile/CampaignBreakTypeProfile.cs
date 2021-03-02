using System;
using xggameplan.Model.AutoGen;

namespace xggameplan.Profile
{
    internal class CampaignBreakTypeProfile : AutoMapper.Profile
    {
        public CampaignBreakTypeProfile()
        {
            CreateMap<Tuple<int, string>, AgCampaignBreakType>().ConstructUsing(t => LoadAgCampaignBreakType(t.Item1, t.Item2));
        }

        private static AgCampaignBreakType LoadAgCampaignBreakType(int campaignNo, string breakType)
        {
            return new AgCampaignBreakType
            {
                CampaignNo = campaignNo,
                BreakType = breakType.Substring(0,2).ToUpper()
            };
        }
    }
}
