namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    /// <summary>
    /// <para>Reason why the restriction checked exited.</para>
    /// <para>Only for debug use.</para>
    /// </summary>
    public enum DebugRestrictionCheckerExitReason
    {
        ValueHasNotBeenSet = 0,
        RestrictionCheckingIsDisabled,
        NoRestrictionsToCheck,
        RestrictionIsNotForBreakSalesArea,
        RestrictionOnlyAppliesToSchoolHolidays,
        BreakIsOutsideTheRestrictionStartEndDateTime,
        RestrictionDoesNotCoverTheBreakDay,
    }
}
