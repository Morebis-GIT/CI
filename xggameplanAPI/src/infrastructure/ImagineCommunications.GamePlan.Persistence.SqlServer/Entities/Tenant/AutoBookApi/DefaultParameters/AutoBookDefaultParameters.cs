using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi.DefaultParameters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "<Pending>")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "<Pending>")]
    public class AutoBookDefaultParameters : IUniqueIdentifierPrimaryKey
    {
        public Guid Id { get; set; }

        #region AgBreak
        public int AgBreak_SalesAreaNo { get; set; }
        public int AgBreak_SalesAreaId { get; set; }
        public string AgBreak_ScheduledDate { get; set; }
        public int AgBreak_BreakNo { get; set; }
        public string AgBreak_ExternalNo { get; set; }
        public string AgBreak_NominalTime { get; set; }
        public int AgBreak_Uid { get; set; }
        public int AgBreak_ProgEventNo { get; set; }
        public int AgBreak_Duration { get; set; }
        public string AgBreak_Solus { get; set; }
        public int AgBreak_DayNumber { get; set; }
        public string AgBreak_PositionInProg { get; set; }
        public int AgBreak_ProgNo { get; set; }
        public int AgBreak_EpisNo { get; set; }
        public string AgBreak_BreakTypeCode { get; set; }
        public int AgBreak_NbrBkrgs { get; set; }
        public List<AgRegionalBreak> AgBreak_AgRegionalBreaks { get; set; }
        public int AgBreak_NbrZeroBkrgs { get; set; }
        public int AgBreak_NbrPreds { get; set; }
        public List<AgPrediction> AgBreak_AgPredictions { get; set; }
        public int AgBreak_NbrAvals { get; set; }
        public List<AgAval> AgBreak_AgAvals { get; set; }
        public int AgBreak_NbrPrgcs { get; set; }
        public List<int> AgBreak_AgProgCategories { get; set; }
        public int AgBreak_MaxPrgcs { get; set; }
        public int AgBreak_AgSalesAreaPtrRef_SalesAreaNo { get; set; }
        public int AgBreak_AgSalesAreaPtrRef_ClassId { get; set; }
        public string AgBreak_LongForm { get; set; }
        #endregion

        #region AgCampaign
        public int AgCampaign_CampaignNo { get; set; }
        public string AgCampaign_ExternalNo { get; set; }
        public int AgCampaign_DemographicNo { get; set; }
        public int AgCampaign_DealNo { get; set; }
        public int AgCampaign_ProductCode { get; set; }
        public string AgCampaign_ClearanceCode { get; set; }
        public int AgCampaign_BusinesssAreaNo { get; set; }
        public int AgCampaign_RevenueBudget { get; set; }
        public int AgCampaign_DeliveryCurrency { get; set; }
        public string AgCampaign_MultiPartFlag { get; set; }
        public string AgCampaign_StartDate { get; set; }
        public string AgCampaign_EndDate { get; set; }
        public string AgCampaign_RootClashCode { get; set; }
        public string AgCampaign_ClashCode { get; set; }
        public string AgCampaign_AdvertiserIdentifier { get; set; }
        public int AgCampaign_ClashNo { get; set; }
        public double AgCampaign_AgCampaignRequirement_Required { get; set; }
        public double AgCampaign_AgCampaignRequirement_TgtRequired { get; set; }
        public double AgCampaign_AgCampaignRequirement_SareRequired { get; set; }
        public double AgCampaign_AgCampaignRequirement_Supplied { get; set; }
        public int AgCampaign_IncludeFunctions { get; set; }
        public int AgCampaign_NbrAgCampagignSalesArea { get; set; }
        public int AgCampaign_MaxAgCampagignSalesArea { get; set; }
        public int AgCampaign_CampaignSpotMaxRatings { get; set; }
        public List<AgCampaignSalesArea> AgCampaign_AgCampaignSalesAreas { get; set; }
        public int AgCampaign_NbrAgCampaignProgrammeList { get; set; }
        public List<AgCampaignProgramme> AgCampaign_AgProgrammeList { get; set; }
        #endregion        

        #region AgExposure
        public int AgExposure_BreakSalesAreaNo { get; set; }
        public string AgExposure_ClashCode { get; set; }
        public string AgExposure_MasterClashCode { get; set; }
        public string AgExposure_StartDate { get; set; }
        public string AgExposure_EndDate { get; set; }
        public string AgExposure_StartTime { get; set; }
        public string AgExposure_EndTime { get; set; }
        public int AgExposure_StartDay { get; set; }
        public int AgExposure_EndDay { get; set; }
        public int AgExposure_NoOfExposures { get; set; }
        #endregion

        #region AgHfssDemo
        public List<AgHfssDemo> AgHfssDemos { get; set; }
        #endregion

        #region AgISRTimeBand
        public string AgISRTimeBand_StartTime { get; set; }
        public string AgISRTimeBand_EndTime { get; set; }
        public int AgISRTimeBand_Days { get; set; }
        public string AgISRTimeBand_Exclude { get; set; }
        #endregion

        #region AgPeakStartEndTime
        public int AgPeakStartEndTime_SalesArea { get; set; }
        public int AgPeakStartEndTime_ScenarioNumber { get; set; }
        public int AgPeakStartEndTime_DayPartNumber { get; set; }
        public int AgPeakStartEndTime_StartDayOfDayPart { get; set; }
        public int AgPeakStartEndTime_EndDayOfDayPart { get; set; }
        public string AgPeakStartEndTime_StartTimeOfDayPart { get; set; }
        public string AgPeakStartEndTime_EndTimeOfDayPart { get; set; }
        public int AgPeakStartEndTime_MidPoint { get; set; }
        public string AgPeakStartEndTime_Name { get; set; }
        #endregion

        #region AgProgRestriction
        public int AgProgRestriction_CampaignNo { get; set; }
        public int AgProgRestriction_SalesAreaNo { get; set; }
        public int AgProgRestriction_ProgNo { get; set; }
        public int AgProgRestriction_PrgcNo { get; set; }
        public string AgProgRestriction_IncludeExcludeFlag { get; set; }
        public int AgProgRestriction_EpisNo { get; set; }
        #endregion

        #region AgProgTxDetail
        public int AgProgTxDetail_ProgrammeNo { get; set; }
        public int AgProgTxDetail_EpisodeNo { get; set; }
        public int AgProgTxDetail_SalesAreaNo { get; set; }
        public int AgProgTxDetail_TregNo { get; set; }
        public string AgProgTxDetail_TxDate { get; set; }
        public string AgProgTxDetail_ScheduledStartTime { get; set; }
        public string AgProgTxDetail_ScheduledEndTime { get; set; }
        public int AgProgTxDetail_ProgCategoryNo { get; set; }
        public string AgProgTxDetail_ClassCode { get; set; }
        public string AgProgTxDetail_LiveBroadcast { get; set; }
        #endregion

        #region AgRestriction
        public int AgRestriction_ProductCode { get; set; }
        public string AgRestriction_ClashCode { get; set; }
        public string AgRestriction_CopyCode { get; set; }
        public string AgRestriction_ClearanceCode { get; set; }
        public int AgRestriction_SalesAreaNo { get; set; }
        public int AgRestriction_ProgCategoryNo { get; set; }
        public int AgRestriction_ProgrammeNo { get; set; }
        public int AgRestriction_EpisodeNo { get; set; }
        public string AgRestriction_StartDate { get; set; }
        public string AgRestriction_EndDate { get; set; }
        public int AgRestriction_IndexType { get; set; }
        public int AgRestriction_IndexThreshold { get; set; }
        public string AgRestriction_PublicHolidayIndicator { get; set; }
        public string AgRestriction_SchoolHolidayIndicator { get; set; }
        public int AgRestriction_RestrictionType { get; set; }
        public int AgRestriction_RestrictionDays { get; set; }
        public string AgRestriction_StartTime { get; set; }
        public string AgRestriction_EndTime { get; set; }
        public int AgRestriction_TimeToleranceMinsBefore { get; set; }
        public int AgRestriction_TimeToleranceMinsAfter { get; set; }
        public string AgRestriction_ProgClassCode { get; set; }
        public string AgRestriction_ProgClassFlag { get; set; }
        public string AgRestriction_LiveBroadcastFlag { get; set; }
        #endregion

        #region AgSpot
        public int AgSpot_BreakSalesAreaNo { get; set; }
        public string AgSpot_BreakDate { get; set; }
        public string AgSpot_BreakTime { get; set; }
        public int AgSpot_BreakNo { get; set; }
        public int AgSpot_CampaignNo { get; set; }
        public int AgSpot_SpotNo { get; set; }
        public string AgSpot_Status { get; set; }
        public string AgSpot_MultipartIndicator { get; set; }
        public int AgSpot_PreempteeStatus { get; set; }
        public int AgSpot_PreemptorStatus { get; set; }
        public int AgSpot_BookingPosition { get; set; }
        public int AgSpot_SpotLength { get; set; }
        public int AgSpot_SpotSalesAreaNo { get; set; }
        public double AgSpot_PriceFactor { get; set; }
        public string AgSpot_BonusSpot { get; set; }
        public string AgSpot_ClientPicked { get; set; }
        public int AgSpot_ISRLocked { get; set; }
        public int AgSpot_ProductCode { get; set; }
        public string AgSpot_ClashCode { get; set; }
        public string AgSpot_AdvertiserIdentifier { get; set; }
        public string AgSpot_RootClashCode { get; set; }
        #endregion
    }
}
