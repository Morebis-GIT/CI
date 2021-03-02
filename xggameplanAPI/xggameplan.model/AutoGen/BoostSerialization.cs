using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public abstract class BoostSerialization
    {
        public BoostSerialization()
        {
            Size = 1;                               // value - to be changed in derived class.
            Signature = "serialization::archive";   // Default value - leave it as it is.
            Version = 1;                          // Default value unless autogen has any specific requirements.
        }

        [XmlElement(ElementName = "size")]
        public int Size { get; set; }

        [XmlAttribute(AttributeName = "signature")]
        public string Signature { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public int Version { get; set; }
    }
}
