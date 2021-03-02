namespace xggameplan.AutoBooks.AWS
{
    /// <summary>
    /// Environment details from AutoBooks API (AWS)
    /// </summary>
    public class AWSPAEnvironment
    {
        public string BinariesBucket { get; set; }
        public string DataExchangeBucket { get; set; }
        public string DynamoDBAutoBooksTable { get; set; }
        public string DynamoDBLogsTable { get; set; }
        public string IamRole { get; set; }
        public string Name { get; set; }

        public string Region { get; set; }
        public string EC2KeyPair { get; set; }
        public string GameplanAPIURL { get; set; }

        public string VPCID { get; set; }
        public string VPCSubnets { get; set; }
    }
}
