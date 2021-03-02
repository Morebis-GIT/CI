using System.Collections.Generic;
using System.Text;

namespace xggameplan.AuditEvents.Sample
{
    /// <summary>
    /// Test values
    /// </summary>
    public class TestValues
    {
        public Dictionary<string, object> Values { get; set; }

        public override string ToString()
        {
            StringBuilder values = new StringBuilder("");
            if (Values != null)
            {
                foreach (string key in Values.Keys)
                {
                    if (values.Length > 0)
                    {
                        _ = values.Append("; ");
                    }
                    if (Values[key] == null)
                    {
                        _ = values.Append(string.Format("{0}={1}", key, "null"));
                    }
                    else
                    {
                        _ = values.Append(string.Format("{0}={1}", key, Values[key].ToString()));
                    }
                }
            }
            return values.ToString();
        }
    }
}
