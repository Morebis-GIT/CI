namespace ImagineCommunications.BusClient.Domain
{
    public enum MessageState
    {
        Pending,
        ReadyForRetry,
        InProgress,
        Failed,
        Completed,
    }

    public enum MessageBodyType
    {
        NormalMessage,
        BigMessage,
    }
}
