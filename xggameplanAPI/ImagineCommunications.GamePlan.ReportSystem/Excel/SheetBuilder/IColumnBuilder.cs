namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder
{
    public interface IColumnBuilder
    {
        IColumnBuilder Width(double width);

        IColumnBuilder AutoFitColumn();

        IColumnBuilder Format(string format);
    }
}
