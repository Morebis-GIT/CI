using System;

namespace xggameplan.Services
{
    /// <summary>
    /// Custom attribute to control whether method/class appears in documentation (E.g. Swagger)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class DocsTagsAttribute : System.Attribute
    {     
        private string[] _tags = new string[0];

        public DocsTagsAttribute(params string[] tags)
        {
            _tags = tags;
        }        

        /// <summary>
        /// Whether attribute contains any of the input tags. If input tags contains null then this allows any tag.      
        /// </summary>
        /// <param name="tags">List of tags</param>
        /// <returns></returns>
        public bool ContainsAnyTag(string[] tags)
        {
            foreach(string tag in tags)
            {
                if (tag == null || Array.IndexOf(_tags, tag) != -1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}