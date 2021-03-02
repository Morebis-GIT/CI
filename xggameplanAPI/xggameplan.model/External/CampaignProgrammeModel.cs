using System;
using System.Collections.Generic;
using System.Linq;

namespace xggameplan.Model
{
    public class CampaignProgrammeModel : ICloneable
    {
        public string IsCategoryOrProgramme { get; set; }
        public IList<string> CategoryOrProgramme { get; set; }
        public IList<string> SalesAreas { get; set; }

        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IList<TimesliceModel> Timeband { get; set; }

        public object Clone()
        {
            var model = (CampaignProgrammeModel)MemberwiseClone();

            model.SalesAreas = SalesAreas?.ToList();
            model.CategoryOrProgramme = CategoryOrProgramme?.ToList();
            model.Timeband = Timeband?.Select(x => (TimesliceModel) x.Clone()).ToList();

            return model;
        }
    }
}
