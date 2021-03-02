using System.Runtime.Serialization;

namespace xggameplan.Model
{
    [DataContract(Name = "ABPayload", Namespace = "")]
    public class LandmarkInputFilePayload
    {
        /// <summary>
        /// Name of the xml file - Includes .xml extension
        /// </summary>
        [DataMember(Name = "filename", Order = 0)]
        public string FileName { get; set; }

        /// <summary>
        /// XML File Contents - To be supplied as XML DATA
        /// </summary>
        [DataMember(Name = "payload", Order = 1)]
        public string Payload { get; set; }
    }
}
