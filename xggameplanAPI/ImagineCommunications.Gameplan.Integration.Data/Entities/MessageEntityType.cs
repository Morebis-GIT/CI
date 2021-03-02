using System.Collections.Generic;

namespace ImagineCommunications.Gameplan.Integration.Data.Entities
{
    public class MessageEntityType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual List<MessageType> MessageTypes { get; set; }
    }
}
