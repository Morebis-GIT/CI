using ImagineCommunications.GamePlan.Domain.Shared.Languages;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class LanguageProfile : AutoMapper.Profile
    {
        public LanguageProfile()
        {
            CreateMap<Language, LanguageModel>();
        }
    }
}
