namespace ImagineCommunications.GamePlan.Domain.Generic.Queries
{
    public class Order<T>
    {
        public T OrderBy { get; set; }

        public OrderDirection OrderDirection { get; set; }
    }
}
