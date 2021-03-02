namespace xggameplan.AutoBooks
{
    /// <summary>
    /// AutoBook Environment details from Provisioning API. This class is generic and hides cloud specific (e.g. AWS etc) details.
    /// </summary>
    public class Environment
    {
        public string Name { get; set; }
        public string GameplanAPIURL { get; set; }
        public string VPCID { get; set; }
        public string VPCSubnets { get; set; }
    }

    /// <summary>
    /// AutoBook details from Provisioning API
    /// </summary>
    public class PAAutoBook
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public bool Provisioned { get; set; }
    }
}
