using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessTypes.Objects;

namespace ImagineCommunications.GamePlan.Domain.BusinessTypes
{
    public interface IBusinessTypeRepository
    {
        IEnumerable<BusinessType> GetAll();

        BusinessType GetByCode(string code);

        bool Exists(string code);
    }
}
