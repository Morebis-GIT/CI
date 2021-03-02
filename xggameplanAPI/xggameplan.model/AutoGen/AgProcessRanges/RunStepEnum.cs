using System.ComponentModel;

namespace xggameplan.Model.AutoGen
{
    public enum RunStepEnum
    {
        [Description("Smooth")]
        Smooth = 1,
        [Description("ISR")]
        ISR = 2,
        [Description("Right Sizer")]
        RightSizer = 3,
        [Description("Optimisation")]
        Optimiser = 4
    }
}
