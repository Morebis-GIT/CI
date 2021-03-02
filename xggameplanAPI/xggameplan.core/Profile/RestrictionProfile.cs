using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions.Objects;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.ProgrammeDictionaries;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.core.Extensions;
using xggameplan.Extensions;
using xggameplan.Model;
using xggameplan.Model.AutoGen;

namespace xggameplan.Profile
{
    public class TimeRestrictionProfile : AutoMapper.Profile
    {
        public TimeRestrictionProfile()
        {
            CreateMap<Tuple<TimeRestriction, int, List<SalesArea>>, List<AgTimeRestriction>>()
                .ConstructUsing(t => LoadAgTimeRestriction(t.Item1, t.Item2, t.Item3));
        }

        private List<AgTimeRestriction> LoadAgTimeRestriction(
            TimeRestriction timeRestriction,
            int campaignNo,
            List<SalesArea> allSalesAreas)
        {
            var dayNumbers = timeRestriction.DowPattern?.ToArray().GetDayNumbers();
            List<SalesArea> salesAreas;
            if (timeRestriction.SalesAreas == null || !timeRestriction.SalesAreas.Any())
            {
                salesAreas = allSalesAreas;
            }
            else
            {
                salesAreas = allSalesAreas.Where(s => timeRestriction.SalesAreas.Contains(s.Name)).ToList();
            }

            return salesAreas
                .SelectMany(salesArea =>
                {
                    return dayNumbers?.Distinct().Select(dayNumber => new AgTimeRestriction
                    {
                        CampaignNo = campaignNo,
                        DayNo = dayNumber,
                        EndDate = timeRestriction.EndDateTime.ToString("yyyyMMdd"),
                        EndTime = timeRestriction.EndDateTime.ToString("HHmmss"),
                        IncludeExcludeFlag = timeRestriction.IsIncludeOrExclude,
                        SalesAreaNo = salesArea?.CustomId ?? 0,
                        StartDate = timeRestriction.StartDateTime.ToString("yyyyMMdd"),
                        StartTime = timeRestriction.StartDateTime.ToString("HHmmss")
                    }).ToList();
                }).ToList();
        }
    }

    [Obsolete("AutoMapper should be phased out for conditional mapping.")]
    public class ProgrammeRestrictionProfile : AutoMapper.Profile
    {
        public ProgrammeRestrictionProfile()
        {
            CreateMap<Tuple<ProgrammeRestriction, int, List<SalesArea>, List<ProgrammeDictionary>, AgProgRestriction>, List<AgProgRestriction>>()
                .ConstructUsing(t => LoadAgProgrammeRestriction(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));

            CreateMap<Tuple<ProgrammeRestriction, int, List<SalesArea>, List<Data>, AgProgRestriction>, List<AgProgRestriction>>()
                .ConstructUsing(t => LoadAgProgrammeRestriction(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));
        }

        [Obsolete("Not required once AutoMapping of Programme Restrictions is removed.")]
        private List<AgProgRestriction> LoadAgProgrammeRestriction(
            ProgrammeRestriction programmeRestriction,
            int campaignNo,
            List<SalesArea> allSalesAreas,
            dynamic categoriesOrProgrammes,
            AgProgRestriction agProgRestriction)
        {
            List<SalesArea> salesAreas;
            if (programmeRestriction.SalesAreas == null || !programmeRestriction.SalesAreas.Any())
            {
                salesAreas = allSalesAreas;
            }
            else
            {
                salesAreas = allSalesAreas.Where(s => programmeRestriction.SalesAreas.Contains(s.Name)).ToList();
            }

            if (programmeRestriction.IsCategoryOrProgramme.Equals(CategoryOrProgramme.C.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var categories = ((List<Data>)categoriesOrProgrammes)
                    .Where(c => programmeRestriction.CategoryOrProgramme.Any(p => p.Equals(c.Value.ToString(), StringComparison.OrdinalIgnoreCase)))
                    .ToList();

                if (!categories.Any())
                {
                    return null;
                }

                return salesAreas
                    .SelectMany(salesArea =>
                        {
                            var agProgRestrictionClone = agProgRestriction.Clone();

                            return categories.Select(category =>
                            {
                                agProgRestrictionClone.CampaignNo = campaignNo;
                                agProgRestrictionClone.SalesAreaNo = salesArea?.CustomId ?? 0;
                                agProgRestrictionClone.PrgcNo = category.Id;
                                agProgRestrictionClone.IncludeExcludeFlag = programmeRestriction.IsIncludeOrExclude?.ToUpper();

                                return agProgRestrictionClone;
                            }).ToList();
                        }
                    ).ToList();
            }

            var programmes = ((List<ProgrammeDictionary>)categoriesOrProgrammes)
                .Where(c => programmeRestriction.CategoryOrProgramme.Any(p => p.Equals(c.ExternalReference.ToString(), StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (!programmes.Any())
            {
                return null;
            }

            return salesAreas
                .SelectMany(salesArea =>
                    {
                        var agProgRestrictionClone = agProgRestriction.Clone();

                        return programmes.Select(programme =>
                        {
                            agProgRestrictionClone.CampaignNo = campaignNo;
                            agProgRestrictionClone.SalesAreaNo = salesArea?.CustomId ?? 0;
                            agProgRestrictionClone.ProgNo = programme.Id;
                            agProgRestrictionClone.IncludeExcludeFlag = programmeRestriction.IsIncludeOrExclude?.ToUpper();

                            return agProgRestrictionClone;
                        }).ToList();
                    }
                ).ToList();
        }
    }

    public class RestrictionProfile : AutoMapper.Profile
    {
        public RestrictionProfile()
        {
            CreateMap<Tuple<Restriction, RestrictionDescription>, RestrictionWithDescModel>().ConstructUsing(_ => LoadRestrictionWithDesc(_.Item1, _.Item2));
            CreateMap<RestrictionDescription, RestrictionWithDescModel>();

            CreateMap<CreateRestriction, Restriction>();
            CreateMap<List<CreateRestriction>, List<Restriction>>();
            CreateMap<UpdateRestrictionModel, Restriction>();
            CreateMap<UpdateRestrictionModel, CreateRestriction>();
            CreateMap<Restriction, RestrictionModel>();
            CreateMap<Tuple<List<Restriction>, List<ProgrammeCategoryHierarchy>, IReadOnlyCollection<ProgrammeDictionary>, List<SalesArea>, DateTime, List<Clash>, AgRestriction>, List<AgRestriction>>()
                .ConstructUsing(t => LoadAgRestriction(
                    t.Item1,
                    t.Item2,
                    t.Item3,
                    t.Item4,
                    t.Item5,
                    t.Item6,
                    t.Item7));
        }

        private RestrictionWithDescModel LoadRestrictionWithDesc(Restriction item1, RestrictionDescription item2)
        {
            return new RestrictionWithDescModel
            {
                ClashCode = item1.ClashCode,
                ClearanceCode = item1.ClearanceCode,
                ClockNumber = string.IsNullOrWhiteSpace(item1.ClockNumber)
                    ? "0"
                    : item1.ClockNumber,
                EndDate = item1.EndDate,
                ExternalProgRef = item1.ExternalProgRef,
                EndTime = item1.EndTime,
                IndexThreshold = item1.IndexThreshold,
                IndexType = item1.IndexType,
                LiveProgrammeIndicator = item1.LiveProgrammeIndicator,
                StartDate = item1.StartDate,
                StartTime = item1.StartTime,
                Uid = item1.Uid,
                SalesAreas = item1.SalesAreas,
                RestrictionDays = item1.RestrictionDays,
                SchoolHolidayIndicator = item1.SchoolHolidayIndicator,
                PublicHolidayIndicator = item1.PublicHolidayIndicator,
                RestrictionType = item1.RestrictionType,
                RestrictionBasis = item1.RestrictionBasis,
                ProgrammeCategory = item1.ProgrammeCategory,
                ExternalIdentifier = item1.ExternalIdentifier,
                ProgrammeClassification = item1.ProgrammeClassification,
                ProgrammeClassificationIndicator = item1.ProgrammeClassificationIndicator,
                TimeToleranceMinsBefore = item1.TimeToleranceMinsBefore,
                TimeToleranceMinsAfter = item1.TimeToleranceMinsAfter,
                ProductCode = item1.ProductCode,
                ProgrammeDescription = item2.ProgrammeDescription,
                ProductDescription = item2.ProductDescription,
                ClashDescription = item2.ClashDescription,
                AdvertiserName = item2.AdvertiserName
            };
        }

        private List<AgRestriction> LoadAgRestriction(List<Restriction> restrictions,
            List<ProgrammeCategoryHierarchy> programmeCategories,
            IReadOnlyCollection<ProgrammeDictionary> programmeDictionaries,
            List<SalesArea> allSalesAreas,
            DateTime runEndDate,
            List<Clash> allClashes,
            AgRestriction agRestriction)
        {
            var agRestrictions = new List<AgRestriction>();
            var salesAreaNamesIndex = allSalesAreas.ToDictionary(s => s.Name);
            var programmeDictionaryExternalRefsIndex = programmeDictionaries.ToDictionary(c => c.ExternalReference.ToUpper());
            var programmeCategoryIdsIndex = new Dictionary<int, ProgrammeCategoryHierarchy>();
            var programmeCategoryNamesIndex = new Dictionary<string, ProgrammeCategoryHierarchy>();
            var programmeCategoryExternalRefsIndex = new Dictionary<string, ProgrammeCategoryHierarchy>();
            var clashesExternalRefsIndex = new Dictionary<string, Clash>();
            var clashesParentExternalIdentifiersIndex = new Dictionary<string, List<Clash>>();

            foreach (var programmeCategory in programmeCategories)
            {
                programmeCategoryIdsIndex.Add(programmeCategory.Id, programmeCategory);
                programmeCategoryNamesIndex.Add(programmeCategory.Name.ToUpper(), programmeCategory);

                if (!string.IsNullOrWhiteSpace(programmeCategory.ExternalRef))
                {
                    programmeCategoryExternalRefsIndex.Add(programmeCategory.ExternalRef, programmeCategory);
                }
            }

            foreach (var clash in allClashes)
            {
                clashesExternalRefsIndex.Add(clash.Externalref, clash);

                if (string.IsNullOrWhiteSpace(clash.ParentExternalidentifier))
                {
                    continue;
                }

                if (!clashesParentExternalIdentifiersIndex.ContainsKey(clash.ParentExternalidentifier))
                {
                    clashesParentExternalIdentifiersIndex.Add(clash.ParentExternalidentifier, new List<Clash>());
                }

                clashesParentExternalIdentifiersIndex[clash.ParentExternalidentifier].Add(clash);
            }

            foreach (var restriction in restrictions)
            {
                List<SalesArea> salesAreas;

                if (restriction.SalesAreas == null || !restriction.SalesAreas.Any())
                {
                    salesAreas = allSalesAreas;
                }
                else
                {
                    salesAreas = new List<SalesArea>();

                    foreach (var item in restriction.SalesAreas)
                    {
                        if (salesAreaNamesIndex.TryGetValue(item, out SalesArea area))
                        {
                            salesAreas.Add(area);
                        }
                    }
                }

                foreach (var salesArea in salesAreas)
                {
                    if (restriction.RestrictionBasis == RestrictionBasis.Clash)
                    {
                        IEnumerable<Clash> clashes = Enumerable.Empty<Clash>();

                        if (clashesParentExternalIdentifiersIndex.TryGetValue(restriction.ClashCode, out var childClashes))
                        {
                            clashes = clashes.Union(childClashes);
                        }

                        if (clashesExternalRefsIndex.TryGetValue(restriction.ClashCode, out var item))
                        {
                            clashes = clashes.Append(item);
                        }

                        foreach (var clash in clashes)
                        {
                            var agRestrictionClone = agRestriction.Clone();

                            agRestrictionClone.ProductCode = restriction.ProductCode;
                            agRestrictionClone.ClashCode = clash.Externalref;
                            agRestrictionClone.CopyCode = string.IsNullOrWhiteSpace(restriction.ClockNumber)
                                ? "0"
                                : restriction.ClockNumber;
                            agRestrictionClone.ClearanceCode = restriction.ClearanceCode ?? "";
                            agRestrictionClone.SalesAreaNo = salesArea?.CustomId ?? 0;
                            agRestrictionClone.ProgCategoryNo =
                                !string.IsNullOrWhiteSpace(restriction.ProgrammeCategory) && programmeCategoryNamesIndex.TryGetValue(restriction.ProgrammeCategory.ToUpper(), out var category)
                                    ? category.Id
                                    : 0;
                            agRestrictionClone.ProgrammeNo =
                                !string.IsNullOrWhiteSpace(restriction.ExternalProgRef) && programmeDictionaryExternalRefsIndex.TryGetValue(restriction.ExternalProgRef.ToUpper(), out var programme)
                                    ? programme.Id
                                    : 0;
                            agRestrictionClone.StartDate = restriction.StartDate.ToString("yyyyMMdd");
                            agRestrictionClone.EndDate = restriction.EndDate.HasValue
                                ? restriction.EndDate?.ToString("yyyyMMdd")
                                : runEndDate.AddYears(10).ToString("yyyyMMdd");
                            agRestrictionClone.IndexType = restriction.IndexType;
                            agRestrictionClone.IndexThreshold = restriction.IndexThreshold;
                            agRestrictionClone.PublicHolidayIndicator =
                                AgConversions.ToAgIncludeExcludeEither(restriction.PublicHolidayIndicator);
                            agRestrictionClone.SchoolHolidayIndicator =
                                AgConversions.ToAgIncludeExcludeEither(restriction.SchoolHolidayIndicator);
                            agRestrictionClone.RestrictionType = Convert.ToInt32(restriction.RestrictionType);
                            agRestrictionClone.RestrictionDays = AgConversions.ToAgDaysAsInt(restriction.RestrictionDays);
                            agRestrictionClone.StartTime = restriction.StartTime?.ToString("hhmmss") ?? "0";
                            agRestrictionClone.EndTime = restriction.EndTime?.ToString("hhmmss") ?? "995959";
                            agRestrictionClone.TimeToleranceMinsBefore = restriction.TimeToleranceMinsBefore;
                            agRestrictionClone.TimeToleranceMinsAfter = restriction.TimeToleranceMinsAfter;
                            agRestrictionClone.ProgClassCode = restriction.ProgrammeClassification ?? "";
                            agRestrictionClone.ProgClassFlag = restriction.ProgrammeClassificationIndicator.ToString();
                            agRestrictionClone.LiveBroadcastFlag = restriction.LiveProgrammeIndicator.ToString();
                            agRestrictionClone.EpisodeNo = restriction.EpisodeNumber;

                            agRestrictions.Add(agRestrictionClone);

                            if (agRestrictionClone.ProgCategoryNo != 0)
                            {
                                var clashCode = clash.Externalref;
                                AddRestrictionForParentProgrammeCategory(
                                    programmeDictionaryExternalRefsIndex,
                                    runEndDate,
                                    agRestriction,
                                    restriction,
                                    salesArea,
                                    agRestrictions,
                                    clashCode,
                                    programmeCategoryExternalRefsIndex,
                                    programmeCategoryIdsIndex,
                                    agRestrictionClone.ProgCategoryNo
                                );
                            }
                        }
                    }
                    else
                    {
                        var agRestrictionClone = agRestriction.Clone();

                        agRestrictionClone.ProductCode = restriction.ProductCode;
                        agRestrictionClone.ClashCode = restriction.ClashCode ?? "";
                        agRestrictionClone.CopyCode = string.IsNullOrWhiteSpace(restriction.ClockNumber)
                            ? "0"
                            : restriction.ClockNumber;
                        agRestrictionClone.ClearanceCode = restriction.ClearanceCode ?? "";
                        agRestrictionClone.SalesAreaNo = salesArea?.CustomId ?? 0;
                        agRestrictionClone.ProgCategoryNo =
                            !string.IsNullOrWhiteSpace(restriction.ProgrammeCategory) && programmeCategoryNamesIndex.TryGetValue(restriction.ProgrammeCategory, out var category)
                                ? category.Id
                                : 0;
                        agRestrictionClone.ProgrammeNo =
                            !string.IsNullOrWhiteSpace(restriction.ExternalProgRef) && programmeDictionaryExternalRefsIndex.TryGetValue(restriction.ExternalProgRef.ToUpper(), out var programme)
                                ? programme.Id
                                : 0;
                        agRestrictionClone.StartDate = restriction.StartDate.ToString("yyyyMMdd");
                        agRestrictionClone.EndDate = restriction.EndDate.HasValue
                            ? restriction.EndDate?.ToString("yyyyMMdd")
                            : runEndDate.AddYears(10).ToString("yyyyMMdd");
                        agRestrictionClone.IndexType = restriction.IndexType;
                        agRestrictionClone.IndexThreshold = restriction.IndexThreshold;
                        agRestrictionClone.PublicHolidayIndicator =
                            AgConversions.ToAgIncludeExcludeEither(restriction.PublicHolidayIndicator);
                        agRestrictionClone.SchoolHolidayIndicator =
                            AgConversions.ToAgIncludeExcludeEither(restriction.SchoolHolidayIndicator);
                        agRestrictionClone.RestrictionType = Convert.ToInt32(restriction.RestrictionType);
                        agRestrictionClone.RestrictionDays = AgConversions.ToAgDaysAsInt(restriction.RestrictionDays);
                        agRestrictionClone.StartTime = restriction.StartTime?.ToString("hhmmss") ?? "0";
                        agRestrictionClone.EndTime = restriction.EndTime?.ToString("hhmmss") ?? "995959";
                        agRestrictionClone.TimeToleranceMinsBefore = restriction.TimeToleranceMinsBefore;
                        agRestrictionClone.TimeToleranceMinsAfter = restriction.TimeToleranceMinsAfter;
                        agRestrictionClone.ProgClassCode = restriction.ProgrammeClassification ?? "";
                        agRestrictionClone.ProgClassFlag = restriction.ProgrammeClassificationIndicator.ToString();
                        agRestrictionClone.LiveBroadcastFlag = restriction.LiveProgrammeIndicator.ToString();
                        agRestrictionClone.EpisodeNo = restriction.EpisodeNumber;

                        agRestrictions.Add(agRestrictionClone);

                        if (agRestrictionClone.ProgCategoryNo != 0)
                        {
                            var clashCode = restriction.ClashCode ?? "";

                            AddRestrictionForParentProgrammeCategory(
                                    programmeDictionaryExternalRefsIndex,
                                    runEndDate,
                                    agRestriction,
                                    restriction,
                                    salesArea,
                                    agRestrictions,
                                    clashCode,
                                    programmeCategoryExternalRefsIndex,
                                    programmeCategoryIdsIndex,
                                    agRestrictionClone.ProgCategoryNo
                                );
                        }
                    }
                }
            }

            return agRestrictions;
        }

        private static void AddRestrictionForParentProgrammeCategory(
            Dictionary<string, ProgrammeDictionary> programmeDictionaries,
            DateTime runEndDate,
            AgRestriction agRestriction,
            Restriction restriction,
            SalesArea salesArea,
            List<AgRestriction> agRestrictions,
            string clashCode,
            Dictionary<string, ProgrammeCategoryHierarchy> programmeCategoriesExternalRefIndex,
            Dictionary<int, ProgrammeCategoryHierarchy> programmeCategoriesIdIndex,
            int programmeCategoryId)
        {
            if (!programmeCategoriesIdIndex.TryGetValue(programmeCategoryId, out var category) || string.IsNullOrEmpty(category.ParentExternalRef))
            {
                return;
            }

            var parentCategoryNum = programmeCategoriesExternalRefIndex.TryGetValue(category.ParentExternalRef, out var parentCategory) ? parentCategory.Id : 0;
            if (parentCategoryNum == 0)
            {
                return;
            }

            var agRestrictionClone = agRestriction.Clone();

            agRestrictionClone.ProductCode = restriction.ProductCode;
            agRestrictionClone.ClashCode = clashCode;
            agRestrictionClone.CopyCode = string.IsNullOrWhiteSpace(restriction.ClockNumber)
                ? "0"
                : restriction.ClockNumber;
            agRestrictionClone.ClearanceCode = restriction.ClearanceCode ?? "";
            agRestrictionClone.SalesAreaNo = salesArea?.CustomId ?? 0;
            agRestrictionClone.ProgCategoryNo = parentCategoryNum;
            agRestrictionClone.ProgrammeNo =
                !string.IsNullOrWhiteSpace(restriction.ExternalProgRef) && programmeDictionaries.TryGetValue(restriction.ExternalProgRef.ToUpper(), out var programme)
                    ? programme.Id
                    : 0;
            agRestrictionClone.StartDate = restriction.StartDate.ToString("yyyyMMdd");
            agRestrictionClone.EndDate = restriction.EndDate.HasValue
                ? restriction.EndDate?.ToString("yyyyMMdd")
                : runEndDate.AddYears(10).ToString("yyyyMMdd");
            agRestrictionClone.IndexType = restriction.IndexType;
            agRestrictionClone.IndexThreshold = restriction.IndexThreshold;
            agRestrictionClone.PublicHolidayIndicator =
                AgConversions.ToAgIncludeExcludeEither(restriction.PublicHolidayIndicator);
            agRestrictionClone.SchoolHolidayIndicator =
                AgConversions.ToAgIncludeExcludeEither(restriction.SchoolHolidayIndicator);
            agRestrictionClone.RestrictionType = Convert.ToInt32(restriction.RestrictionType);
            agRestrictionClone.RestrictionDays = AgConversions.ToAgDaysAsInt(restriction.RestrictionDays);
            agRestrictionClone.StartTime = restriction.StartTime?.ToString("hhmmss") ?? "0";
            agRestrictionClone.EndTime = restriction.EndTime?.ToString("hhmmss") ?? "995959";
            agRestrictionClone.TimeToleranceMinsBefore = restriction.TimeToleranceMinsBefore;
            agRestrictionClone.TimeToleranceMinsAfter = restriction.TimeToleranceMinsAfter;
            agRestrictionClone.ProgClassCode = restriction.ProgrammeClassification ?? "";
            agRestrictionClone.ProgClassFlag = restriction.ProgrammeClassificationIndicator.ToString();
            agRestrictionClone.LiveBroadcastFlag = restriction.LiveProgrammeIndicator.ToString();

            agRestrictions.Add(agRestrictionClone);
        }
    }
}
