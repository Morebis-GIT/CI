namespace xggameplan.AutoBooks.AWS
{
    /// <summary>
    /// AutoBook details from AutoBooks API (AWS)
    /// </summary>
    public class AWSPAAutoBook
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public bool Provisioned { get; set; }
        public string InstanceType { get; set; }
        public string StorageSizeGb { get; set; }

        public static int IdLength = 24;        // Length of Id
    }
}
