using System;
using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings.Objects
{
    /// <summary>
    /// Demographic settings for Inefficient Spot Removal
    /// </summary>
    public class ISRDemographicSettings
    {
        public string DemographicId { get; set; }

        public int EfficiencyThreshold { get; set; }

        /// <summary>
        /// Returns whether the list of demographic settings are the same
        /// </summary>
        /// <param name="demographicSettings1"></param>
        /// <param name="demographicSettings2"></param>
        /// <returns></returns>
        public static bool IsSame(List<ISRDemographicSettings> demographicSettings1, List<ISRDemographicSettings> demographicSettings2)
        {
            if (demographicSettings1 != null && demographicSettings2 != null && demographicSettings1.Count == demographicSettings1.Count)
            {
                System.Text.StringBuilder demographicString1 = new System.Text.StringBuilder("");
                demographicSettings1.OrderBy(ds => ds.DemographicId).ToList().ForEach(ds => demographicString1.Append(string.Format("#{0}", Serialize(ds))));
                System.Text.StringBuilder demographicString2 = new System.Text.StringBuilder("");
                demographicSettings2.OrderBy(ds => ds.DemographicId).ToList().ForEach(ds => demographicString2.Append(string.Format("#{0}", Serialize(ds))));
                return demographicString1.ToString() == demographicString2.ToString();
            }
            return false;
        }

        private static string Serialize(ISRDemographicSettings demographicSettings)
        {
            return string.Format("ID={1}{0}ET={2}", (Char)0, demographicSettings.DemographicId, demographicSettings.EfficiencyThreshold);
        }
    }
}
