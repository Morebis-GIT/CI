namespace xggameplan.Model
{
    // this model is used to build a request package to tell AWS what autobook is required in the new distributed world
    public class AutoBookRequestModel
    {
        public string respondTo { get; set; }
        public string runId { get; set; }

        public string scenarioId { get; set; }

        public string instanceType { get; set; }

        public int storageSizeGB { get; set; }

        public string version { get; set; }
        public string binariesVersion { get; set; }

        public int maxInstances { get; set; }

        public string mock { get; set; }
    }
}
