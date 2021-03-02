using System;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Runs.Settings;
using xggameplan.Model;

namespace xggameplan.Profile
{
    internal class CampaignProcessesSettingsProfile: AutoMapper.Profile
    {
        public CampaignProcessesSettingsProfile()
        {
            CreateMap<CampaignRunProcessesSettings, CampaignRunProcessesSettingsModel>()
                .ForMember(d => d.IncludeRightSizer,
                    m => m.MapFrom(src => ResolveIncludeRightSizer(src.IncludeRightSizer, src.RightSizerLevel)));

            CreateMap<CampaignRunProcessesSettingsModel, CampaignRunProcessesSettings>()
                .ForMember(d => d.IncludeRightSizer, m => m.MapFrom(src => ResolveRightSizerEnabled(src.IncludeRightSizer)))
                .ForMember(d => d.RightSizerLevel, m => m.MapFrom(src => ResolveRightSizerLevel(src.IncludeRightSizer)));
        }

        private bool? ResolveRightSizerEnabled(IncludeRightSizer? includeRightSizer)
        {
            if (!includeRightSizer.HasValue)
            {
                return null;
            }

            switch (includeRightSizer.Value)
            {
                case IncludeRightSizer.No:
                    return false;
                case IncludeRightSizer.CampaignLevel:
                case IncludeRightSizer.DetailLevel:
                    return true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(includeRightSizer));
            }
        }

        private RightSizerLevel? ResolveRightSizerLevel(IncludeRightSizer? includeRightSizer)
        {
            if (!includeRightSizer.HasValue || includeRightSizer.Value == IncludeRightSizer.No)
            {
                return null;
            }

            switch (includeRightSizer.Value)
            {
                case IncludeRightSizer.CampaignLevel:
                    return RightSizerLevel.CampaignLevel;
                case IncludeRightSizer.DetailLevel:
                    return RightSizerLevel.DetailLevel;
                default:
                    throw new ArgumentOutOfRangeException(nameof(includeRightSizer));
            }
        }

        private IncludeRightSizer? ResolveIncludeRightSizer(bool? enabled, RightSizerLevel? rightSizerLevel)
        {
            if (!enabled.HasValue)
            {
                return null;
            }

            if (!enabled.Value)
            {
                return IncludeRightSizer.No;
            }

            if (!rightSizerLevel.HasValue)
            {
                throw new ArgumentException("Right Sizer Level cannot be null while Right Sizer is enabled");
            }

            switch (rightSizerLevel.Value)
            {
                case RightSizerLevel.CampaignLevel:
                    return IncludeRightSizer.CampaignLevel;
                case RightSizerLevel.DetailLevel:
                    return IncludeRightSizer.DetailLevel;
                default:
                    throw new ArgumentOutOfRangeException(nameof(rightSizerLevel));
                        
            }
        }
    }
}
