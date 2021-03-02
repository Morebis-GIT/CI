using System;
using System.Collections.Generic;
using System.Text;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.BRS
{
    public class CreateOrUpdateBRSConfigurationTemplateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BRSConfigurationForKPIModel> KPIConfigurations { get; set; } = new List<BRSConfigurationForKPIModel>();
    }
}
