using System;
using ImagineCommunications.GamePlan.Utils.DataPurging.Handlers;

namespace ImagineCommunications.GamePlan.Utils.DataPurging.Attributes
{
    /// <summary>
    /// Represents configuration section of purging handler.
    /// Should be used for descendants of <see cref="DataPurgingHandlerBase{TOptions}"/> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PurgingOptionsAttribute : Attribute
    {
        private const string RootSectionName = "purging";

        public PurgingOptionsAttribute(string sectionName)
        {
            SectionName = sectionName ?? throw new ArgumentNullException(nameof(sectionName));
        }

        /// <summary>Gets the name of the section.</summary>
        public string SectionName { get; }

        /// <summary>
        /// Gets or sets the name of named options.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the full name of the configuration section.
        /// </summary>
        public string FullSectionName => string.Join(":", RootSectionName, SectionName);
    }
}
