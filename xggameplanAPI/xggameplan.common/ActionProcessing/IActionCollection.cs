namespace xggameplan.common.ActionProcessing
{
    public interface IActionCollection
    {
        void Add(IAction action);

        void Clear();

        int Count { get; }
    }
}
