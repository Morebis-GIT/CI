namespace xggameplan.Model
{
    public class CreateRunTypeAnalysisGroupModel
    {
        /// <summary>
        /// Analysis Group Id
        /// </summary>
        public int AnalysisGroupId { get; set; }

        /// <summary>
        /// KPI value based on RunTypeAnalysisGroupKPINames class
        /// </summary>
        public string KPI { get; set; }

        /// <summary>
        /// Analysis Group Name
        /// </summary>
        public string AnalysisGroupName { get; set; }
    }
}
