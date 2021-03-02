using System.Collections.Generic;
using Raven.Client.UniqueConstraints;

namespace ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries
{
    public class ProgrammeDictionary
    {
        [UniqueConstraint]
        public int Id { get; set; }

        [UniqueConstraint]
        public string ExternalReference { get; set; }

        public string ProgrammeName { get; set; }

        public string Description { get; set; }

        public string Classification { get; set; }

        /// <summary>
        /// Indexes list by Id
        /// </summary>
        /// <param name="programmeDictionaries"></param>
        /// <returns></returns>
        public static Dictionary<int, ProgrammeDictionary> IndexListById(IEnumerable<ProgrammeDictionary> programmeDictionaries)
        {
            var programmeDictionariesIndexed = new Dictionary<int, ProgrammeDictionary>();

            foreach (var programmeDictionary in programmeDictionaries)
            {
                if (!programmeDictionariesIndexed.ContainsKey(programmeDictionary.Id))
                {
                    programmeDictionariesIndexed.Add(programmeDictionary.Id, programmeDictionary);
                }
            }

            return programmeDictionariesIndexed;
        }
    }
}
