using System;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel
{
    public class Cursor: ICloneable
    {
        public int CurrentColumn { get; set; } = 1;
        public int CurrentRow { get; set; } = 1;

        public Cursor()
        {
        }

        public Cursor(int currentColumn, int currentRow)
        {
            CurrentColumn = currentColumn;
            CurrentRow = currentRow;
        }

        public object Clone()
        {
            return new Cursor(CurrentColumn, CurrentRow);
        }
    }
}
