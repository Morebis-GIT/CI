using System.Collections.Generic;

namespace xggameplan.Model
{
    public class RuleTypeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsCustom { get; set; }
        public bool AllowedForAutopilot { get; set; }
        public List<RuleModel> Rules { get; set; }
    }
}
