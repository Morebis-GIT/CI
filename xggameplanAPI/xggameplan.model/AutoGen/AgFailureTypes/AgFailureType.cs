using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgFailureType
    {
        /// <summary>
        /// Failure type id
        /// </summary>
        [XmlElement(ElementName = "fail_type.type")]
        public int FailureTypeId { get; set; }
    }
}
