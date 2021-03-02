using System.Collections.Generic;
using xgCore.xgGamePlan.ApiEndPoints.Models.Outputfiles;

namespace xgCore.xgGamePlan.AutomationTests.Contexts
{
    public class OutputFilesContext
    {
        public IEnumerable<OutputFileModel> OutputFileModels { get; set; }
    }
}
