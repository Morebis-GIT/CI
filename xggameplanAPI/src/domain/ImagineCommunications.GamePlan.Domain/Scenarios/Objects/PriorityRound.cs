namespace ImagineCommunications.GamePlan.Domain.Scenarios.Objects
{
    public struct PriorityRound
    {
        public int Number { get; set; }
        public int PriorityFrom { get; set; }
        public int PriorityTo { get; set; }

        public PriorityRound(int number, int priorityFrom, int priorityTo)
        {
            Number = number;
            PriorityFrom = priorityFrom;
            PriorityTo = priorityTo;
        }
    }
}
