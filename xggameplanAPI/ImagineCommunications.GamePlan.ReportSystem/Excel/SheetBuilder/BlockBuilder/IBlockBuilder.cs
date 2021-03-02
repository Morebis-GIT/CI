namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder.BlockBuilder
{
    public interface IBlockBuilder
    {
        IBlockOptionsBuilder Add<T>(T value);
    }
}
