
namespace xggameplan.Model
{
    public class PriorityRoundModel
    {
        public int? Number { get; set; }
        public int PriorityFrom { get; set; }
        public int PriorityTo { get; set; }

        public PriorityRoundModel(int? number, int priorityFrom, int priorityTo)
        {
            Number = number;
            PriorityFrom = priorityFrom;
            PriorityTo = priorityTo;
        }
    }
}
