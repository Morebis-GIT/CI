namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    /// <summary>
    /// <para>Reason why the restriction checked exited.</para>
    /// <para>Only for debug use.</para>
    /// </summary>
    public enum DebugExitReason_Restriction
    {
        GuruMeditation_ValueHasNotBeenSet = 0,
        CheckingIsDisabled,
        NoRestrictionsToCheck,
        IsNotForBreakSalesArea,
        DoesNotApplyToLiveBroadcasts,
        AppliesToBreaksWithinSchoolHolidays,
        AppliesToBreaksOutsideSchoolHolidays,
        AppliesToBreaksWithinPublicHolidays,
        AppliesToBreaksOutsidePublicHolidays,
        StartEndDateTimeDoesNotContainTheBreak,
        DoesNotCoverTheBreakDay,
        BasisIsByClashButNoClashFound,
        EndOfMethod,
    }
}
