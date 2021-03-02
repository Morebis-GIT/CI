using System;

namespace ImagineCommunications.GamePlan.Domain.Generic.DbSequence
{
    public interface IIdentifierSequence
    {
        T GetNextValue<T>(string sequenceName) where T : IConvertible;
    }
}
