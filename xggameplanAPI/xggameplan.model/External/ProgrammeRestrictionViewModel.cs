using System;
using System.Collections.Generic;

namespace xggameplan.Model
{
    public class ProgrammeRestrictionViewModel : ICloneable
    {
        public List<string> SalesAreas { get; set; }

        /// <summary>
        ///   Contains Category or Programme External Reference
        /// </summary>
        public List<string> CategoryOrProgramme { get; set; }

        /// <summary>
        ///   Contains Category or Programme Name 
        /// </summary>
        public List<string> CategoryOrProgrammeName { get; set; }

        public string IsCategoryOrProgramme { get; set; }

        public string IsIncludeOrExclude { get; set; }

        public object Clone()
        {
            var programmeRestriction = (ProgrammeRestrictionViewModel)MemberwiseClone();

            if (SalesAreas != null)
            {
                programmeRestriction.SalesAreas = new List<string>();
                SalesAreas.ForEach(sa => programmeRestriction.SalesAreas.Add((string)sa.Clone()));
            }
            if (CategoryOrProgramme != null)
            {
                programmeRestriction.CategoryOrProgramme = new List<string>();
                CategoryOrProgramme.ForEach(sa => programmeRestriction.CategoryOrProgramme.Add((string)sa.Clone()));
            }
            if (CategoryOrProgrammeName != null)
            {
                programmeRestriction.CategoryOrProgrammeName = new List<string>();
                CategoryOrProgrammeName.ForEach(sa => programmeRestriction.CategoryOrProgrammeName.Add((string)sa.Clone()));
            }

            return programmeRestriction;
        }
    }
}
