using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.Demographics
{
    public class Demographic
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// External Ref
        /// </summary>
        public string ExternalRef { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Shortname
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Whether visible in Gameplan
        /// </summary>
        public bool Gameplan { get; set; }

        /// <summary>
        /// Indexes list by Id
        /// </summary>
        /// <param name="demographics"></param>
        /// <returns></returns>
        public static Dictionary<int, Demographic> IndexListById(IEnumerable<Demographic> demographics)
        {
            var dataIndexed = new Dictionary<int, Demographic>();

            foreach (var demographic in demographics)
            {
                if (!dataIndexed.ContainsKey(demographic.Id))
                {
                    dataIndexed.Add(demographic.Id, demographic);
                }
            }

            return dataIndexed;
        }

        /// <summary>
        /// Updates the Demographic
        /// </summary>
        /// <param name="name"></param>
        /// <param name="shortName"></param>
        /// <param name="displayOrder"></param>
        /// <param name="gameplan"></param>
        public void Update(
            string name,
            string shortName,
            int displayOrder,
            bool gameplan
            )
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(name));
            }

            if (string.IsNullOrEmpty(shortName))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(shortName));
            }

            Name = name;
            ShortName = shortName;
            DisplayOrder = displayOrder;
            Gameplan = gameplan;
        }

        public override string ToString() => $"{Name} [{ShortName}]";
    }
}
