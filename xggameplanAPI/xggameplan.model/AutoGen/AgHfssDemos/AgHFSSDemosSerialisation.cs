using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgHfssDemosSerialisation : BoostSerialization
    {
        //Size - needs to be populated based on the AgTimeRestrictions list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgHfssDemo> AgHfssDemos { get; set; }

        public AgHfssDemosSerialisation PopulateStaticHfssDemos(List<AgHfssDemo> agHfssDemos)
        {
            AgHfssDemos = agHfssDemos;
            Size = agHfssDemos.Count;
            return this;
        }

        public AgHfssDemosSerialisation MapFrom(List<AgHfssDemo> agHfssDemos)
        {
            AgHfssDemos = agHfssDemos;
            Size = agHfssDemos.Count;
            return this;
        }
    }
}
