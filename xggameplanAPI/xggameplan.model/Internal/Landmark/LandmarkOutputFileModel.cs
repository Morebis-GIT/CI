using System.Runtime.Serialization;

namespace xggameplan.Model
{
    [DataContract(Name = "ABResult", Namespace = "")]
    public class LandmarkOutputFileModel
    {
        [DataMember(Name = "filename", Order = 0)]
        public string FileName { get; set; }

        [DataMember(Name = "result", Order = 1)]
        public string Payload { get; set; }
    }
}
