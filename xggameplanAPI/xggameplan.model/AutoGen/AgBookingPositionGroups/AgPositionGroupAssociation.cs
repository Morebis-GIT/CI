using System.Collections.Generic;
using System.Xml.Serialization;

namespace xggameplan.Model.AutoGen
{
    public class AgPositionGroupAssociation
    {
        [XmlElement(ElementName = "bkpo_data.bkpo_posn_reqm")]
        public int BookingPosition { get; set; }

        [XmlElement(ElementName = "bkpo_data.booking_order")]
        public int BookingOrder { get; set; }

        [XmlElement(ElementName = "bkpo_data.sub_booking_order")]
        public int SubBookingOrder { get; set; }
    }

    public class AgPositionGroupAssociations : List<AgPositionGroupAssociation>
    {
    }
}
