using System.Collections.Generic;
using System.Xml.Serialization;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace xggameplan.Model.AutoGen
{
    [XmlRoot(ElementName = "boost_serialization")]
    public class AgProgTxDetailsSerialization : BoostSerialization
    {
        //Size - needs to be populated based on the AgProgrammes list size
        //Version - remains as 1 for now unless autogen wants otherwise.

        [XmlArray("list")]
        [XmlArrayItem("item")]
        public List<AgProgTxDetail> AgProgTxDetails { get; set; }

        public AgProgTxDetailsSerialization MapFrom(List<AgProgTxDetail> agProgTxDetails)
        {
            AgProgTxDetails = agProgTxDetails;
            Size = agProgTxDetails.Count;
            return this;
        }
    }
}
