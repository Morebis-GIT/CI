using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "Item")]
    public class AgChannelGroup
    {
        [XmlElement(ElementName = "channel_group_data.camp_no")]
        public int CampaignNo { get; set; }

        [XmlElement(ElementName = "channel_group_data.channel_group_no")]
        public int ChannelGroupNo { get; set; }

        [XmlElement(ElementName = "channel_group_data.nbr_sares")]
        public int NbrAgChannelGroupSalesArea { get; set; }

        [XmlArray("sare_list")]
        [XmlArrayItem("item")]
        public AgChannelGroupSalesAreas AgChannelGroupSalesAreas { get; set; }
    }

    public class AgChannelGroupSalesAreas : List<AgChannelGroupSalesArea> { }

    /// <summary>
    /// DTO class
    /// </summary>
    public class AgCampaignWithChannelGroup
    {
        public List<AgCampaign> Agcampaigns { get; set; }
        public List<Tuple<int, int, SalesAreaGroup>> ChannelGroups { get; set; }
    }
}
