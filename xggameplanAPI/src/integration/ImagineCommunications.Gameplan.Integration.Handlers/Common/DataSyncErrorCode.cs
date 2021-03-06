﻿namespace ImagineCommunications.Gameplan.Integration.Handlers.Common
{
    public enum DataSyncErrorCode
    {
        UniqueConstraintViolation,
        DuplicateExternalReference,
        ExternalReferenceNotFound,
        SalesAreaNotFound,
        ProgrammeCategoryNotFound,
        DemographicNotFound,
        BookingPositionNotFound,
        BookingPositionGroupNotFound,
        ClearanceCodeNotFound,
        BreakNotFound,
        ClashCodeNotFound,
        ProductNotFound,
        ProductAdvertiserNotFound,
        SpotNotFound,
        LockTypeNotFound,
        InvalidBreakType,
        InvalidRightSizer,
        InvalidCampaignPassPriority,
        Clash_ParentDoesNotExist,
        Clash_ExposureCountGreaterThenParents,
        Universe_StartDateLessThanPredecessors,
        Universe_DateRangeOverlapsPredecessors,
        Universe_GapMoreThan1Day,
        Campaign_ActiveCampaignNotFound,
        Campaign_FailedToGenerateCustomId,
        Spot_SalesAreaMismatchWithBreak,
        Spot_StartDateMismatchWithBreak,
        ClashException_StructureRulesViolation,
        ClashException_TimeRangesOverlap,
        DuplicateParentAndExternalReference,
        TotalRatingsNotFound,
        DayPartNotFound
    }
}
