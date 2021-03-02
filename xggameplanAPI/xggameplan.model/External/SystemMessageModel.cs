using System.Collections.Generic;

namespace xggameplan.Model
{
    public class SystemMessageModel
    {
        /// <summary>
        /// Id. This is hidden because we may then start getting requests to return the ID in the HTTP header for failures. This is something
        /// that we do not want to do.
        /// </summary>
        //public int Id { get; set; }

        /// <summary>
        /// Groups that message is a member of
        /// </summary>
        //public List<SystemMessageGroups> MessageGroups { get; set; }

        /// <summary>
        /// Description (Multi-language)
        /// </summary>
        public Dictionary<string, string> Description { get; set; }

        /// <summary>
        /// Link with additional information
        /// </summary>
        public string Link { get; set; }

        public SystemMessageModel()
        {

        }

        public SystemMessageModel(Dictionary<string, string> description, string link)
        {
            //MessageGroups = messageGroups;
            Description = description;
            Link = link;
        }
    }
}
