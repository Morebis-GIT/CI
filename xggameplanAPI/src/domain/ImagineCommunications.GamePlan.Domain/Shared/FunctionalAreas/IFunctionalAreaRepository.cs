using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;

namespace ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas
{
    public interface IFunctionalAreaRepository
    {
        void Add(FunctionalArea functionalArea);

        IEnumerable<FunctionalArea> GetAll();

        IEnumerable<int> GetSelectedFailureTypeIds();

        FunctionalArea Find(Guid id);

        FaultType FindFaultType(int faultTypeId);

        IEnumerable<FaultType> FindFaultTypes(List<int> faultTypeIds);

        void UpdateFaultTypesSelections(FunctionalArea functionalArea);
    }
}
