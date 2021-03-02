using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace ImagineCommunications.GamePlan.Domain.Generic.DbSequence
{
    /// <summary>
    /// Interface for generating identity values
    /// </summary>
    public interface IIdentityGenerator
    {        
        List<T> GetIdentities<T>(int number) where T : class, IIntIdentifier, new();
    }
}
