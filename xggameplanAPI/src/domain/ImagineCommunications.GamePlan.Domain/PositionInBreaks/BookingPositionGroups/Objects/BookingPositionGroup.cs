using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects
{
    public class BookingPositionGroup
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool UserDefined { get; set; }
        public List<PositionGroupAssociation> PositionGroupAssociations { get; set; }
    }
}
