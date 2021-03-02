using Raven.Client.UniqueConstraints;

namespace ImagineCommunications.GamePlan.Domain.Shared.Channels
{
    public class Channel
    {
        /// <summary>
        /// Unique Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Channel name
        /// </summary>
        [UniqueConstraint]
        public string Name { get; set; }

        /// <summary>
        /// Channel short name
        /// </summary>
        [UniqueConstraint]
        public string ShortName { get; set; }
    }
}
