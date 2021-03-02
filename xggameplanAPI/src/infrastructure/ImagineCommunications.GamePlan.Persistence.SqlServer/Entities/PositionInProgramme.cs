using System.ComponentModel;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities
{
    public enum PositionInProgramme
    {
        [Description("A")]
        All = 0,
        [Description("C")]
        Centre = 1,
        [Description("E")]
        End = 2
    }
}
