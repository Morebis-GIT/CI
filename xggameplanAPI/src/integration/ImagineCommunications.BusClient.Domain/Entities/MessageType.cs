namespace ImagineCommunications.BusClient.Domain.Entities
{
    public class MessageType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public int? BatchSize { get; set; }
        public int MessageEntityTypeId { get; set; }
        public string Description { get; set; }
    }
}
