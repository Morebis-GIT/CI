﻿using System;
using System.Collections.Generic;
using System.Text;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.BRS
{
    public class BRSConfigurationTemplateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsDefault { get; set; }
        public List<BRSConfigurationForKPIModel> KPIConfigurations { get; set; } = new List<BRSConfigurationForKPIModel>();
    }
}
