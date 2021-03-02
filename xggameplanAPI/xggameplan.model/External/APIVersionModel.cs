using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xggameplan.Model
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public sealed class APIVersionModel
        : IEquatable<APIVersionModel>
    {
        /// <summary>
        /// The GitVersion generated for this build.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Git hash of the check-in this version was built from.
        /// </summary>
        public string Hash { get; }

        /// <summary>
        /// The branch name this version was built from.
        /// </summary>
        public string Branch { get; }

        /// <summary>
        /// The CI build number that made this build.
        /// </summary>
        public string Build { get; }

        private static char[] _versionSegmentSeparator = new[] { '.' };

        private APIVersionModel()
            : this(string.Empty, string.Empty, string.Empty, string.Empty)
        {
        }

        private APIVersionModel(string version, string hash, string branch, string build)
        {
            Version = version;
            Hash = hash;
            Branch = branch;
            Build = build;
        }

        /// <summary>
        /// Create a version model from a GitVersion string.
        /// </summary>
        /// <param name="versionData">
        /// Version strings look like this:
        /// <para>
        /// <list type="bullet">
        /// <item>3.11.0-master.372.20116+ci.0adbd77.master</item>
        /// <item>3.11.0-bugfix.380.20127+ci.562ce5a.bugfix-xggt-15608-master-embed-correct-versiontxt</item>
        /// </list>
        /// </para>
        /// </param>
        /// <returns></returns>
        public static APIVersionModel Create(string versionData)
        {
            if (string.IsNullOrWhiteSpace(versionData))
            {
                return VersionDataMissing();
            }

            int endOfVersionMarkerPosition = versionData.IndexOf('-');
            if (endOfVersionMarkerPosition < 0)
            {
                return VersionDataMissing();
            }

            var version = versionData.Substring(0, endOfVersionMarkerPosition);
            var versionTail = versionData.Substring(endOfVersionMarkerPosition);

            if (versionTail.Length == 1)
            {
                // There is nothing after the marker
                return VersionDataMissing();
            }

            var branch = string.Empty;
            var buildNumber = string.Empty;
            var hash = string.Empty;

            var versionSegments = versionTail.Split(
                _versionSegmentSeparator,
                StringSplitOptions.RemoveEmptyEntries
                );

            if (versionSegments.Length == 0 || versionSegments.Length < 5)
            {
                return VersionDataMissing();
            }

            buildNumber = versionSegments[2];
            hash = versionSegments[3];
            branch = versionSegments[4];

            return new APIVersionModel(version, hash, branch, buildNumber);

            APIVersionModel VersionDataMissing() =>
                new APIVersionModel();
        }

        public override string ToString() =>
            $"{Version}-{Build}.{Hash}.{Branch}";

        public override bool Equals(object obj) => Equals(obj as APIVersionModel);

        public bool Equals(APIVersionModel other) =>
            other != null &&
            Version == other.Version &&
            Hash == other.Hash &&
            Branch == other.Branch && Build == other.Build;

        public override int GetHashCode()
        {
            int hashCode = 1376292912;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Version);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Hash);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Branch);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Build);
            return hashCode;
        }

        public static bool operator ==(APIVersionModel left, APIVersionModel right) =>
            EqualityComparer<APIVersionModel>.Default.Equals(left, right);

        public static bool operator !=(APIVersionModel left, APIVersionModel right) =>
            !(left == right);

        private string GetDebuggerDisplay() => ToString();
    }
}
