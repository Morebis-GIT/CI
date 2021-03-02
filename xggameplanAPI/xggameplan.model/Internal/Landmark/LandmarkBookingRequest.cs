using System.Collections.Generic;
using System.Runtime.Serialization;

namespace xggameplan.Model
{
    [DataContract(Name = "AutobookRequest", Namespace = "")]
    public class LandmarkBookingRequest
    {
        /// <summary>
        /// Organization Code of the Person making the request
        /// </summary>
        [DataMember(Name = "orgr_code", Order = 0)]
        public string OrganizationCode { get; set; }

        /// <summary>
        /// Position Code of the Person making the request
        /// </summary>
        [DataMember(Name = "posn_code", Order = 1)]
        public string PositionCode { get; set; }

        /// <summary>
        /// List of files for AutoBook to consume
        /// </summary>
        [DataMember(Name = "payload_list", Order = 2)]
        public List<LandmarkInputFilePayload> InputFiles { get; set; }
    }
}
