using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Holidays
{
    public interface IHolidayDeleted : IEvent
    {
        HolidayType? HolidayType { get; }
        string SalesAreaName { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }
}
