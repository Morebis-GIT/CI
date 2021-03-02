namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder
{
    public interface IBaseSheetBuilder<out TSheetBuilder>
        where TSheetBuilder : BaseSheetBuilder<TSheetBuilder>

    {
        TSheetBuilder Freeze(int freezeFirstRows, int freezeFirstColumns);

        TSheetBuilder StartColumn(int startColumn);

        TSheetBuilder Skip(int rowsCount = 1);
    }
}
