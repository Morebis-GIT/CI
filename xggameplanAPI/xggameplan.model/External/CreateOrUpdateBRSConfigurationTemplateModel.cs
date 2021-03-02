using System.Collections.Generic;

namespace xggameplan.model.External
{
    public class CreateOrUpdateBRSConfigurationTemplateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BRSConfigurationForKPIModel> KPIConfigurations { get; set; } = new List<BRSConfigurationForKPIModel>();
    }
}
