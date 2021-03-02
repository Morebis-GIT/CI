using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.LengthFactor
{
    public interface ILengthFactorCreated : IEvent
    {
        string SalesArea { get; }
        TimeSpan Duration { get; }
        double Factor { get; }
    }
}
