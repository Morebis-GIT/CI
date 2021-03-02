using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;
using AutoBookDefaultParametersEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters.AutoBookDefaultParameters;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Mapping
{
    public class AutoBookDefaultParametersProfile : Profile
    {
        public AutoBookDefaultParametersProfile()
        {
            CreateMap<AutoBookDefaultParameters, AutoBookDefaultParametersEntity>()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgCampaign_AgCampaignSalesAreas, opt => opt.Ignore())
                .AfterMap((src, dest, context) =>
                {
                    var salesAreas = dest.AgCampaign_AgCampaignSalesAreas?.Where(e =>
                        e.Type == Entities.Tenant.AutoBookApi.DefaultParameters.NestedType.CollectionItem)
                      .ToList();

                    if (salesAreas == null || !salesAreas.Any())
                    {
                        salesAreas = context.Mapper.Map<List<Entities.Tenant.AutoBookApi.DefaultParameters
                            .AgCampaignSalesArea>>(src.AgCampaign.AgCampaignSalesAreas?.ToList())
                            ?? new List<Entities.Tenant.AutoBookApi.DefaultParameters.AgCampaignSalesArea>(1);
                    }
                    else
                    {
                        context.Mapper.Map(src.AgCampaign.AgCampaignSalesAreas.ToList(), salesAreas);
                    }

                    salesAreas.ForEach(e => e.Type = Entities.Tenant.AutoBookApi.DefaultParameters.NestedType.CollectionItem);

                    var salesArea = dest.AgCampaign_AgCampaignSalesAreas?.FirstOrDefault(e =>
                        e.Type == Entities.Tenant.AutoBookApi.DefaultParameters.NestedType.TypeMember);

                    if (salesArea != null)
                    {
                        context.Mapper.Map(src.AgCampaignSalesArea, salesArea);
                    }
                    else
                    {
                        salesArea = context.Mapper.Map<Entities.Tenant.AutoBookApi.DefaultParameters.AgCampaignSalesArea>(src.AgCampaignSalesArea);
                        salesArea.Type = Entities.Tenant.AutoBookApi.DefaultParameters.NestedType.TypeMember;
                    }

                    salesAreas.Add(salesArea);

                    dest.AgCampaign_AgCampaignSalesAreas = salesAreas;

                    if (dest.Id == Guid.Empty)
                    {
                        dest.Id = src.Id;
                    }
                });

            CreateMap<AutoBookDefaultParametersEntity, AutoBookDefaultParameters>()
                .ForMember(x => x.AgBreak, opt => opt.MapFrom(e => e))
                .ForMember(x => x.AgCampaign, opt => opt.MapFrom(e => e))
                .ForMember(x => x.AgExposure, opt => opt.MapFrom(e => e))
                .ForMember(x => x.AgISRTimeBand, opt => opt.MapFrom(e => e))
                .ForMember(x => x.AgPeakStartEndTime, opt => opt.MapFrom(e => e))
                .ForMember(x => x.AgProgRestriction, opt => opt.MapFrom(e => e))
                .ForMember(x => x.AgProgTxDetail, opt => opt.MapFrom(e => e))
                .ForMember(x => x.AgRestriction, opt => opt.MapFrom(e => e))
                .ForMember(x => x.AgSpot, opt => opt.MapFrom(e => e))
                .AfterMap((src, dest, context) =>
                {
                    var salesArea = src.AgCampaign_AgCampaignSalesAreas.FirstOrDefault(t =>
                        t.Type == Entities.Tenant.AutoBookApi.DefaultParameters.NestedType.TypeMember);

                    dest.AgCampaignSalesArea = context.Mapper.Map<AgCampaignSalesArea>(salesArea);

                    var salesAreas = src.AgCampaign_AgCampaignSalesAreas.Where(t =>
                        t.Type == Entities.Tenant.AutoBookApi.DefaultParameters.NestedType.CollectionItem).ToList();

                    dest.AgCampaign.AgCampaignSalesAreas = context.Mapper.Map<AgCampaignSalesAreas>(salesAreas);
                });

            #region ABDefaultParameters entity to AgBreak domain model mapping
            CreateMap<AutoBookDefaultParametersEntity, AgBreak>()
                .ForMember(x => x.AgSalesAreaPtrRef, opt => opt.MapFrom(e => e))
                .ForMember(x => x.AgAvals, opt => opt.MapFrom(e => e.AgBreak_AgAvals))
                .ForMember(x => x.AgPredictions, opt => opt.MapFrom(e => e.AgBreak_AgPredictions))
                .ForMember(x => x.AgProgCategories, opt => opt.MapFrom(e => e.AgBreak_AgProgCategories))
                .ForMember(x => x.AgRegionalBreaks, opt => opt.MapFrom(e => e.AgBreak_AgRegionalBreaks))
                .ForMember(x => x.BreakNo, opt => opt.MapFrom(e => e.AgBreak_BreakNo))
                .ForMember(x => x.BreakTypeCode, opt => opt.MapFrom(e => e.AgBreak_BreakTypeCode))
                .ForMember(x => x.DayNumber, opt => opt.MapFrom(e => e.AgBreak_DayNumber))
                .ForMember(x => x.Duration, opt => opt.MapFrom(e => e.AgBreak_Duration))
                .ForMember(x => x.EpisNo, opt => opt.MapFrom(e => e.AgBreak_EpisNo))
                .ForMember(x => x.ExternalNo, opt => opt.MapFrom(e => e.AgBreak_ExternalNo))
                .ForMember(x => x.LongForm, opt => opt.MapFrom(e => e.AgBreak_LongForm))
                .ForMember(x => x.MaxPrgcs, opt => opt.MapFrom(e => e.AgBreak_MaxPrgcs))
                .ForMember(x => x.NbrAvals, opt => opt.MapFrom(e => e.AgBreak_NbrAvals))
                .ForMember(x => x.NbrBkrgs, opt => opt.MapFrom(e => e.AgBreak_NbrBkrgs))
                .ForMember(x => x.NbrPreds, opt => opt.MapFrom(e => e.AgBreak_NbrPreds))
                .ForMember(x => x.NbrPrgcs, opt => opt.MapFrom(e => e.AgBreak_NbrPrgcs))
                .ForMember(x => x.NbrZeroBkrgs, opt => opt.MapFrom(e => e.AgBreak_NbrZeroBkrgs))
                .ForMember(x => x.NominalTime, opt => opt.MapFrom(e => e.AgBreak_NominalTime))
                .ForMember(x => x.PositionInProg, opt => opt.MapFrom(e => e.AgBreak_PositionInProg))
                .ForMember(x => x.ProgEventNo, opt => opt.MapFrom(e => e.AgBreak_ProgEventNo))
                .ForMember(x => x.ProgNo, opt => opt.MapFrom(e => e.AgBreak_ProgNo))
                .ForMember(x => x.SalesAreaId, opt => opt.MapFrom(e => e.AgBreak_SalesAreaId))
                .ForMember(x => x.SalesAreaNo, opt => opt.MapFrom(e => e.AgBreak_SalesAreaNo))
                .ForMember(x => x.ScheduledDate, opt => opt.MapFrom(e => e.AgBreak_ScheduledDate))
                .ForMember(x => x.Solus, opt => opt.MapFrom(e => e.AgBreak_Solus))
                .ForMember(x => x.Uid, opt => opt.MapFrom(e => e.AgBreak_Uid));
            #endregion

            #region ABDefaultParameters entity to AgCampaign domain model mapping
            CreateMap<AutoBookDefaultParametersEntity, AgCampaign>()
                .ForMember(x => x.AgCampaignRequirement, opt => opt.MapFrom(e => e))
                .ForMember(x => x.AgCampaignSalesAreas, opt => opt.MapFrom(e => e.AgCampaign_AgCampaignSalesAreas))
                .ForMember(x => x.AgProgrammeList, opt => opt.MapFrom(e => e.AgCampaign_AgProgrammeList))
                .ForMember(x => x.AdvertiserIdentifier, opt => opt.MapFrom(e => e.AgCampaign_AdvertiserIdentifier))
                .ForMember(x => x.BusinesssAreaNo, opt => opt.MapFrom(e => e.AgCampaign_BusinesssAreaNo))
                .ForMember(x => x.CampaignNo, opt => opt.MapFrom(e => e.AgCampaign_CampaignNo))
                .ForMember(x => x.CampaignSpotMaxRatings, opt => opt.MapFrom(e => e.AgCampaign_CampaignSpotMaxRatings))
                .ForMember(x => x.ClashCode, opt => opt.MapFrom(e => e.AgCampaign_ClashCode))
                .ForMember(x => x.ClashNo, opt => opt.MapFrom(e => e.AgCampaign_ClashNo))
                .ForMember(x => x.ClearanceCode, opt => opt.MapFrom(e => e.AgCampaign_ClearanceCode))
                .ForMember(x => x.DealNo, opt => opt.MapFrom(e => e.AgCampaign_DealNo))
                .ForMember(x => x.DeliveryCurrency, opt => opt.MapFrom(e => e.AgCampaign_DeliveryCurrency))
                .ForMember(x => x.DemographicNo, opt => opt.MapFrom(e => e.AgCampaign_DemographicNo))
                .ForMember(x => x.EndDate, opt => opt.MapFrom(e => e.AgCampaign_EndDate))
                .ForMember(x => x.ExternalNo, opt => opt.MapFrom(e => e.AgCampaign_ExternalNo))
                .ForMember(x => x.IncludeFunctions, opt => opt.MapFrom(e => e.AgCampaign_IncludeFunctions))
                .ForMember(x => x.MaxAgCampagignSalesArea, opt => opt.MapFrom(e => e.AgCampaign_MaxAgCampagignSalesArea))
                .ForMember(x => x.MultiPartFlag, opt => opt.MapFrom(e => e.AgCampaign_MultiPartFlag))
                .ForMember(x => x.NbrAgCampagignSalesArea, opt => opt.MapFrom(e => e.AgCampaign_NbrAgCampagignSalesArea))
                .ForMember(x => x.NbrAgCampaignProgrammeList, opt => opt.MapFrom(e => e.AgCampaign_NbrAgCampaignProgrammeList))
                .ForMember(x => x.ProductCode, opt => opt.MapFrom(e => e.AgCampaign_ProductCode))
                .ForMember(x => x.RevenueBudget, opt => opt.MapFrom(e => e.AgCampaign_RevenueBudget))
                .ForMember(x => x.RootClashCode, opt => opt.MapFrom(e => e.AgCampaign_RootClashCode))
                .ForMember(x => x.StartDate, opt => opt.MapFrom(e => e.AgCampaign_StartDate))
                .ForMember(x => x.ExternalNo, opt => opt.MapFrom(e => e.AgCampaign_ExternalNo))
                .ForMember(x => x.AgCampaignSalesAreas, opt => opt.Ignore());
            #endregion

            #region ABDefaultParameters entity to AgExposure domain model mapping
            CreateMap<AutoBookDefaultParametersEntity, AgExposure>()
                .ForMember(x => x.BreakSalesAreaNo, opt => opt.MapFrom(e => e.AgExposure_BreakSalesAreaNo))
                .ForMember(x => x.ClashCode, opt => opt.MapFrom(e => e.AgExposure_ClashCode))
                .ForMember(x => x.EndDate, opt => opt.MapFrom(e => e.AgExposure_EndDate))
                .ForMember(x => x.EndDay, opt => opt.MapFrom(e => e.AgExposure_EndDay))
                .ForMember(x => x.EndTime, opt => opt.MapFrom(e => e.AgExposure_EndTime))
                .ForMember(x => x.MasterClashCode, opt => opt.MapFrom(e => e.AgExposure_MasterClashCode))
                .ForMember(x => x.NoOfExposures, opt => opt.MapFrom(e => e.AgExposure_NoOfExposures))
                .ForMember(x => x.StartDate, opt => opt.MapFrom(e => e.AgExposure_StartDate))
                .ForMember(x => x.StartDay, opt => opt.MapFrom(e => e.AgExposure_StartDay))
                .ForMember(x => x.StartTime, opt => opt.MapFrom(e => e.AgExposure_StartTime));
            #endregion

            CreateMap<AgHfssDemo, Entities.Tenant.AutoBookApi.DefaultParameters.AgHfssDemo>().ReverseMap();

            #region ABDefaultParameters entity to AgISRTimeBand domain model mapping
            CreateMap<AutoBookDefaultParametersEntity, AgISRTimeBand>()
                .ForMember(x => x.Days, opt => opt.MapFrom(e => e.AgISRTimeBand_Days))
                .ForMember(x => x.EndTime, opt => opt.MapFrom(e => e.AgISRTimeBand_EndTime))
                .ForMember(x => x.Exclude, opt => opt.MapFrom(e => e.AgISRTimeBand_Exclude))
                .ForMember(x => x.StartTime, opt => opt.MapFrom(e => e.AgISRTimeBand_StartTime));
            #endregion

            #region ABDefaultParameters entity to AgPeakStartEndTime domain model mapping
            CreateMap<AutoBookDefaultParametersEntity, AgPeakStartEndTime>()
                .ForMember(x => x.DayPartNumber, opt => opt.MapFrom(e => e.AgPeakStartEndTime_DayPartNumber))
                .ForMember(x => x.EndDayOfDayPart, opt => opt.MapFrom(e => e.AgPeakStartEndTime_EndDayOfDayPart))
                .ForMember(x => x.EndTimeOfDayPart, opt => opt.MapFrom(e => e.AgPeakStartEndTime_EndTimeOfDayPart))
                .ForMember(x => x.MidPoint, opt => opt.MapFrom(e => e.AgPeakStartEndTime_MidPoint))
                .ForMember(x => x.Name, opt => opt.MapFrom(e => e.AgPeakStartEndTime_Name))
                .ForMember(x => x.SalesArea, opt => opt.MapFrom(e => e.AgPeakStartEndTime_SalesArea))
                .ForMember(x => x.ScenarioNumber, opt => opt.MapFrom(e => e.AgPeakStartEndTime_ScenarioNumber))
                .ForMember(x => x.StartDayOfDayPart, opt => opt.MapFrom(e => e.AgPeakStartEndTime_StartDayOfDayPart))
                .ForMember(x => x.StartTimeOfDayPart, opt => opt.MapFrom(e => e.AgPeakStartEndTime_StartTimeOfDayPart));
            #endregion

            #region ABDefaultParameters entity to AgProgRestriction domain model mapping
            CreateMap<AutoBookDefaultParametersEntity, AgProgRestriction>()
                .ForMember(x => x.CampaignNo, opt => opt.MapFrom(e => e.AgProgRestriction_CampaignNo))
                .ForMember(x => x.EpisNo, opt => opt.MapFrom(e => e.AgProgRestriction_EpisNo))
                .ForMember(x => x.IncludeExcludeFlag, opt => opt.MapFrom(e => e.AgProgRestriction_IncludeExcludeFlag))
                .ForMember(x => x.PrgcNo, opt => opt.MapFrom(e => e.AgProgRestriction_PrgcNo))
                .ForMember(x => x.ProgNo, opt => opt.MapFrom(e => e.AgProgRestriction_ProgNo))
                .ForMember(x => x.SalesAreaNo, opt => opt.MapFrom(e => e.AgProgRestriction_SalesAreaNo));
            #endregion

            #region ABDefaultParameters entity to AgProgTxDetail domain model mapping
            CreateMap<AutoBookDefaultParametersEntity, AgProgTxDetail>()
                .ForMember(x => x.ClassCode, opt => opt.MapFrom(e => e.AgProgTxDetail_ClassCode))
                .ForMember(x => x.EpisodeNo, opt => opt.MapFrom(e => e.AgProgTxDetail_EpisodeNo))
                .ForMember(x => x.LiveBroadcast, opt => opt.MapFrom(e => e.AgProgTxDetail_LiveBroadcast))
                .ForMember(x => x.ProgCategoryNo, opt => opt.MapFrom(e => e.AgProgTxDetail_ProgCategoryNo))
                .ForMember(x => x.ProgrammeNo, opt => opt.MapFrom(e => e.AgProgTxDetail_ProgrammeNo))
                .ForMember(x => x.SalesAreaNo, opt => opt.MapFrom(e => e.AgProgTxDetail_SalesAreaNo))
                .ForMember(x => x.ScheduledEndTime, opt => opt.MapFrom(e => e.AgProgTxDetail_ScheduledEndTime))
                .ForMember(x => x.ScheduledStartTime, opt => opt.MapFrom(e => e.AgProgTxDetail_ScheduledStartTime))
                .ForMember(x => x.TregNo, opt => opt.MapFrom(e => e.AgProgTxDetail_TregNo))
                .ForMember(x => x.TxDate, opt => opt.MapFrom(e => e.AgProgTxDetail_TxDate));
            #endregion

            #region ABDefaultParameters entity to AgRestriction domain model mapping
            CreateMap<AutoBookDefaultParametersEntity, AgRestriction>()
                .ForMember(x => x.ClashCode, opt => opt.MapFrom(e => e.AgRestriction_ClashCode))
                .ForMember(x => x.ClearanceCode, opt => opt.MapFrom(e => e.AgRestriction_ClearanceCode))
                .ForMember(x => x.CopyCode, opt => opt.MapFrom(e => e.AgRestriction_CopyCode))
                .ForMember(x => x.EndDate, opt => opt.MapFrom(e => e.AgRestriction_EndDate))
                .ForMember(x => x.EndTime, opt => opt.MapFrom(e => e.AgRestriction_EndTime))
                .ForMember(x => x.EpisodeNo, opt => opt.MapFrom(e => e.AgRestriction_EpisodeNo))
                .ForMember(x => x.IndexThreshold, opt => opt.MapFrom(e => e.AgRestriction_IndexThreshold))
                .ForMember(x => x.IndexType, opt => opt.MapFrom(e => e.AgRestriction_IndexType))
                .ForMember(x => x.LiveBroadcastFlag, opt => opt.MapFrom(e => e.AgRestriction_LiveBroadcastFlag))
                .ForMember(x => x.ProductCode, opt => opt.MapFrom(e => e.AgRestriction_ProductCode))
                .ForMember(x => x.ProgCategoryNo, opt => opt.MapFrom(e => e.AgRestriction_ProgCategoryNo))
                .ForMember(x => x.ProgClassCode, opt => opt.MapFrom(e => e.AgRestriction_ProgClassCode))
                .ForMember(x => x.ProgClassFlag, opt => opt.MapFrom(e => e.AgRestriction_ProgClassFlag))
                .ForMember(x => x.ProgrammeNo, opt => opt.MapFrom(e => e.AgRestriction_ProgrammeNo))
                .ForMember(x => x.PublicHolidayIndicator, opt => opt.MapFrom(e => e.AgRestriction_PublicHolidayIndicator))
                .ForMember(x => x.RestrictionDays, opt => opt.MapFrom(e => e.AgRestriction_RestrictionDays))
                .ForMember(x => x.RestrictionType, opt => opt.MapFrom(e => e.AgRestriction_RestrictionType))
                .ForMember(x => x.SalesAreaNo, opt => opt.MapFrom(e => e.AgRestriction_SalesAreaNo))
                .ForMember(x => x.SchoolHolidayIndicator, opt => opt.MapFrom(e => e.AgRestriction_SchoolHolidayIndicator))
                .ForMember(x => x.StartDate, opt => opt.MapFrom(e => e.AgRestriction_StartDate))
                .ForMember(x => x.StartTime, opt => opt.MapFrom(e => e.AgRestriction_StartTime))
                .ForMember(x => x.TimeToleranceMinsAfter, opt => opt.MapFrom(e => e.AgRestriction_TimeToleranceMinsAfter))
                .ForMember(x => x.TimeToleranceMinsBefore, opt => opt.MapFrom(e => e.AgRestriction_TimeToleranceMinsBefore));
            #endregion

            #region ABDefaultParameters entity to AgSpot domain model mapping
            CreateMap<AutoBookDefaultParametersEntity, AgSpot>()
                .ForMember(x => x.AdvertiserIdentifier, opt => opt.MapFrom(e => e.AgSpot_AdvertiserIdentifier))
                .ForMember(x => x.BonusSpot, opt => opt.MapFrom(e => e.AgSpot_BonusSpot))
                .ForMember(x => x.BookingPosition, opt => opt.MapFrom(e => e.AgSpot_BookingPosition))
                .ForMember(x => x.BreakDate, opt => opt.MapFrom(e => e.AgSpot_BreakDate))
                .ForMember(x => x.BreakNo, opt => opt.MapFrom(e => e.AgSpot_BreakNo))
                .ForMember(x => x.BreakSalesAreaNo, opt => opt.MapFrom(e => e.AgSpot_BreakSalesAreaNo))
                .ForMember(x => x.BreakTime, opt => opt.MapFrom(e => e.AgSpot_BreakTime))
                .ForMember(x => x.CampaignNo, opt => opt.MapFrom(e => e.AgSpot_CampaignNo))
                .ForMember(x => x.ClashCode, opt => opt.MapFrom(e => e.AgSpot_ClashCode))
                .ForMember(x => x.ClientPicked, opt => opt.MapFrom(e => e.AgSpot_ClientPicked))
                .ForMember(x => x.ISRLocked, opt => opt.MapFrom(e => e.AgSpot_ISRLocked))
                .ForMember(x => x.MultipartIndicator, opt => opt.MapFrom(e => e.AgSpot_MultipartIndicator))
                .ForMember(x => x.PreempteeStatus, opt => opt.MapFrom(e => e.AgSpot_PreempteeStatus))
                .ForMember(x => x.PreemptorStatus, opt => opt.MapFrom(e => e.AgSpot_PreemptorStatus))
                .ForMember(x => x.PriceFactor, opt => opt.MapFrom(e => e.AgSpot_PriceFactor))
                .ForMember(x => x.ProductCode, opt => opt.MapFrom(e => e.AgSpot_ProductCode))
                .ForMember(x => x.RootClashCode, opt => opt.MapFrom(e => e.AgSpot_RootClashCode))
                .ForMember(x => x.SpotLength, opt => opt.MapFrom(e => e.AgSpot_SpotLength))
                .ForMember(x => x.SpotNo, opt => opt.MapFrom(e => e.AgSpot_SpotNo))
                .ForMember(x => x.SpotSalesAreaNo, opt => opt.MapFrom(e => e.AgSpot_SpotSalesAreaNo))
                .ForMember(x => x.Status, opt => opt.MapFrom(e => e.AgSpot_Status));
            #endregion

            #region mappings to AgRequirement
            CreateMap<AutoBookDefaultParametersEntity, AgRequirement>()
                .ForMember(x => x.Required, opt => opt.MapFrom(e => e.AgCampaign_AgCampaignRequirement_Required))
                .ForMember(x => x.SareRequired, opt => opt.MapFrom(e => e.AgCampaign_AgCampaignRequirement_SareRequired))
                .ForMember(x => x.Supplied, opt => opt.MapFrom(e => e.AgCampaign_AgCampaignRequirement_Supplied))
                .ForMember(x => x.TgtRequired, opt => opt.MapFrom(e => e.AgCampaign_AgCampaignRequirement_TgtRequired));

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgCampaignSalesArea, AgRequirement>()
                .ForMember(x => x.Required, opt => opt.MapFrom(e => e.AgSalesAreaCampaignRequirement_Required))
                .ForMember(x => x.SareRequired, opt => opt.MapFrom(e => e.AgSalesAreaCampaignRequirement_SareRequired))
                .ForMember(x => x.Supplied, opt => opt.MapFrom(e => e.AgSalesAreaCampaignRequirement_Supplied))
                .ForMember(x => x.TgtRequired, opt => opt.MapFrom(e => e.AgSalesAreaCampaignRequirement_TgtRequired))
                .ReverseMap();

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgCampaignProgramme, AgRequirement>()
                .ForMember(x => x.Required, opt => opt.MapFrom(e => e.AgCampaignProgrammeRequirement_Required))
                .ForMember(x => x.SareRequired, opt => opt.MapFrom(e => e.AgCampaignProgrammeRequirement_SareRequired))
                .ForMember(x => x.Supplied, opt => opt.MapFrom(e => e.AgCampaignProgrammeRequirement_Supplied))
                .ForMember(x => x.TgtRequired, opt => opt.MapFrom(e => e.AgCampaignProgrammeRequirement_TgtRequired));

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgDayPart, AgRequirement>()
                .ForMember(x => x.Required, opt => opt.MapFrom(e => e.AgDayPartRequirement_Required))
                .ForMember(x => x.SareRequired, opt => opt.MapFrom(e => e.AgDayPartRequirement_SareRequired))
                .ForMember(x => x.Supplied, opt => opt.MapFrom(e => e.AgDayPartRequirement_Supplied))
                .ForMember(x => x.TgtRequired, opt => opt.MapFrom(e => e.AgDayPartRequirement_TgtRequired))
                .ReverseMap();

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgLength, AgRequirement>()
                .ForMember(x => x.Required, opt => opt.MapFrom(e => e.AgLengthRequirement_Required))
                .ForMember(x => x.SareRequired, opt => opt.MapFrom(e => e.AgLengthRequirement_SareRequired))
                .ForMember(x => x.Supplied, opt => opt.MapFrom(e => e.AgLengthRequirement_Supplied))
                .ForMember(x => x.TgtRequired, opt => opt.MapFrom(e => e.AgLengthRequirement_TgtRequired))
                .ReverseMap();

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgDayPartLength, AgRequirement>()
                .ForMember(x => x.Required, opt => opt.MapFrom(e => e.AgPartLengthRequirement_Required))
                .ForMember(x => x.SareRequired, opt => opt.MapFrom(e => e.AgPartLengthRequirement_SareRequired))
                .ForMember(x => x.Supplied, opt => opt.MapFrom(e => e.AgPartLengthRequirement_Supplied))
                .ForMember(x => x.TgtRequired, opt => opt.MapFrom(e => e.AgPartLengthRequirement_TgtRequired))
                .ReverseMap();

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgPart, AgRequirement>()
                .ForMember(x => x.Required, opt => opt.MapFrom(e => e.AgPartRequirement_Required))
                .ForMember(x => x.SareRequired, opt => opt.MapFrom(e => e.AgPartRequirement_SareRequired))
                .ForMember(x => x.Supplied, opt => opt.MapFrom(e => e.AgPartRequirement_Supplied))
                .ForMember(x => x.TgtRequired, opt => opt.MapFrom(e => e.AgPartRequirement_TgtRequired))
                .ReverseMap();

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgPartLength, AgRequirement>()
                .ForMember(x => x.Required, opt => opt.MapFrom(e => e.AgPartLengthRequirement_Required))
                .ForMember(x => x.SareRequired, opt => opt.MapFrom(e => e.AgPartLengthRequirement_SareRequired))
                .ForMember(x => x.Supplied, opt => opt.MapFrom(e => e.AgPartLengthRequirement_Supplied))
                .ForMember(x => x.TgtRequired, opt => opt.MapFrom(e => e.AgPartLengthRequirement_TgtRequired))
                .ReverseMap();

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgStrikeWeight, AgRequirement>()
                .ForMember(x => x.Required, opt => opt.MapFrom(e => e.AgStikeWeightRequirement_Required))
                .ForMember(x => x.SareRequired, opt => opt.MapFrom(e => e.AgStikeWeightRequirement_SareRequired))
                .ForMember(x => x.Supplied, opt => opt.MapFrom(e => e.AgStikeWeightRequirement_Supplied))
                .ForMember(x => x.TgtRequired, opt => opt.MapFrom(e => e.AgStikeWeightRequirement_TgtRequired))
                .ReverseMap();

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgStrikeWeightLength, AgRequirement>()
                .ForMember(x => x.Required, opt => opt.MapFrom(e => e.AgStrikeWeightLengthRequirement_Required))
                .ForMember(x => x.SareRequired, opt => opt.MapFrom(e => e.AgStrikeWeightLengthRequirement_SareRequired))
                .ForMember(x => x.Supplied, opt => opt.MapFrom(e => e.AgStrikeWeightLengthRequirement_Supplied))
                .ForMember(x => x.TgtRequired, opt => opt.MapFrom(e => e.AgStrikeWeightLengthRequirement_TgtRequired))
                .ReverseMap();
            #endregion

            CreateMap<AutoBookDefaultParametersEntity, AgSalesAreaPtrRef>()
                .ForMember(x => x.ClassId, opt => opt.MapFrom(e => e.AgBreak_AgSalesAreaPtrRef_ClassId))
                .ForMember(x => x.SalesAreaNo, opt => opt.MapFrom(e => e.AgBreak_AgSalesAreaPtrRef_SalesAreaNo));

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgCampaignSalesArea, AgCampaignSalesArea>()
                .ForMember(x => x.AgCampaignSalesAreaPtrRef, opt => opt.MapFrom(e => e))
                .ForMember(x => x.AgSalesAreaCampaignRequirement, opt => opt.MapFrom(e => e))
                .ReverseMap()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AutoBookDefaultParametersId, opt => opt.Ignore());

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgDayPartLength, AgDayPartLength>()
                .ForMember(x => x.AgPartLengthRequirement, opt => opt.MapFrom(e => e))
                .ReverseMap()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgDayPartId, opt => opt.Ignore());

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgDayPart, AgDayPart>()
                .ForMember(x => x.AgDayPartRequirement, opt => opt.MapFrom(e => e))
                .ReverseMap()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgCampaignSalesAreaId, opt => opt.Ignore());

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgLength, AgLength>()
                .ForMember(x => x.AgLengthRequirement, opt => opt.MapFrom(e => e))
                .ReverseMap()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgCampaignSalesAreaId, opt => opt.Ignore());

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgPart, AgPart>()
                .ForMember(x => x.AgPartRequirement, opt => opt.MapFrom(e => e))
                .ReverseMap()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgCampaignSalesAreaId, opt => opt.Ignore());

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgPartLength, AgPartLength>()
                .ForMember(x => x.AgPartLengthRequirement, opt => opt.MapFrom(e => e))
                .ReverseMap()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgCampaignSalesAreaId, opt => opt.Ignore());

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgStrikeWeight, AgStrikeWeight>()
                .ForMember(x => x.AgStikeWeightRequirement, opt => opt.MapFrom(e => e))
                .ReverseMap()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgCampaignSalesAreaId, opt => opt.Ignore());

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgStrikeWeightLength, AgStrikeWeightLength>()
                .ForMember(x => x.AgStrikeWeightLengthRequirement, opt => opt.MapFrom(e => e))
                .ReverseMap()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgStrikeWeightId, opt => opt.Ignore());

            CreateMap<Entities.Tenant.AutoBookApi.DefaultParameters.AgCampaignSalesArea, AgCampaignSalesAreaPtrRef>()
                .ForMember(x => x.ClassId, opt => opt.MapFrom(e => e.AgCampaignSalesAreaPtrRef_ClassId))
                .ForMember(x => x.SalesAreaNo, opt => opt.MapFrom(e => e.AgCampaignSalesAreaPtrRef_SalesAreaNo))
                .ReverseMap();

            CreateMap<AgMultiPart, Entities.Tenant.AutoBookApi.DefaultParameters.AgMultiPart>()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgLengthId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<AgTimeSlice, Entities.Tenant.AutoBookApi.DefaultParameters.AgTimeSlice>()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgDayPartId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<AgRegionalBreak, Entities.Tenant.AutoBookApi.DefaultParameters.AgRegionalBreak>()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AutoBookDefaultParametersId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<AgPrediction, Entities.Tenant.AutoBookApi.DefaultParameters.AgPrediction>()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AutoBookDefaultParametersId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<AgAval, Entities.Tenant.AutoBookApi.DefaultParameters.AgAval>()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AutoBookDefaultParametersId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<AgCampaignProgrammeProgrammeCategory, Entities.Tenant.AutoBookApi.DefaultParameters.AgCampaignProgrammeProgrammeCategory>()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgCampaignProgrammeId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<AgTimeBand, Entities.Tenant.AutoBookApi.DefaultParameters.AgTimeBand>()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AgCampaignProgrammeId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<AgCampaignProgramme, Entities.Tenant.AutoBookApi.DefaultParameters.AgCampaignProgramme>()
                .ForMember(e => e.Id, opt => opt.Ignore())
                .ForMember(e => e.AutoBookDefaultParametersId, opt => opt.Ignore())
                .ForMember(e => e.SalesAreas, opt => opt.MapFrom(x => x.SalesAreas.Select(t => t.SalesAreaNumber).ToList()))
                .ReverseMap()
                .ForMember(x => x.AgCampaignProgrammeRequirement, opt => opt.MapFrom(e => e))
                .ForMember(x => x.SalesAreas, opt => opt.MapFrom(e =>
                    e.SalesAreas.Select(t =>
                       new AgSalesArea { SalesAreaNumber = t })
                    .ToList()
                 ));
        }
    }
}
