using System;
using System.Collections.Generic;
using System.Linq;

namespace xggameplan.Updates
{
    /// <summary>
    /// Version utilities
    /// </summary>
    internal class VersionUtilities
    {
        /// <summary>
        /// Returns versions in ascending order
        /// </summary>
        /// <param name="versions"></param>
        /// <returns></returns>
        public static List<string> GetVersionsInOrder(IEnumerable<string> versions)
        {
            return versions.OrderBy(v => GetVersionElements(v)).ToList();
        }

        /// <summary>
        /// Returns version order number for version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static int GetVersionOrder(string version)
        {
            if (String.IsNullOrEmpty(version))
            {
                return 0;
            }
            int[] versionElements = GetVersionElements(version);
            return Convert.ToInt32(string.Format("{0:00000}{1:00000}{2:00000}", versionElements[0], versionElements[1], versionElements[2]));
        }

        /// <summary>
        /// Gets the elements of the version. E.g. V1.2.3 returns array[1,2,3]
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public static int[] GetVersionElements(string version)
        {
            if (String.IsNullOrEmpty(version))
            {
                return new int[0];
            }
            version = version.ToUpper().Replace("V", "");
            return version.Split('.').Select(e => Convert.ToInt32(e)).ToArray();
        }

        /// <summary>
        /// Compares two versions
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns></returns>
        public static int Compare(string version1, string version2)
        {
            int versionOrder1 = GetVersionOrder(version1);
            int versionOrder2 = GetVersionOrder(version2);
            if (versionOrder1 == versionOrder2)
            {
                return 0;
            }
            else if (versionOrder1 < versionOrder2)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
