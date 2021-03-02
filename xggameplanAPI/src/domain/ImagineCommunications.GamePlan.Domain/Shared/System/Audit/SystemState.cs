using System.Collections.Generic;
using System.Text;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Audit
{
    /// <summary>
    /// System state for run, so that we can detect attempts to change data
    /// </summary>
    public class SystemState
    {
        /// <summary>
        /// Object counts
        /// </summary>
        public Dictionary<string, int> ObjectCounts = new Dictionary<string, int>();

        /// <summary>
        /// Returns whether the system state is the same as last state
        /// </summary>
        /// <param name="lastRunSystemState"></param>
        /// <returns></returns>
        public bool IsSame(SystemState lastRunSystemState)
        {
            bool isSame = true;

            // Check object counts
            if (isSame)
            {
                foreach (var objectType in ObjectCounts.Keys)
                {
                    if (ObjectCounts[objectType] != lastRunSystemState.ObjectCounts[objectType])
                    {
                        isSame = false;
                        break;
                    }
                }
            }

            return isSame;
        }

        public string Description
        {
            get
            {
                StringBuilder description = new StringBuilder("");

                foreach (var objectType in ObjectCounts.Keys)
                {
                    if (description.Length > 0)
                    {
                        _ = description.Append(", ");
                    }
                    _ = description.Append(string.Format("{0}={1}", objectType.ToString(), ObjectCounts[objectType]));
                }

                return description.ToString();
            }
        }
    }
}
