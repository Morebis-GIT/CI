using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Outputfiles
{
    public class OutputFileModel
    {
        public string FileId { get; set; }

        public string Description { get; set; }

        public IEnumerable<OutputFileColumn> Columns { get; set; }
    }
}
