using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.LengthFactors
{
    public interface ILengthFactorRepository
    {
        void AddRange(IEnumerable<LengthFactor> items);

        void Delete(int id);

        IEnumerable<LengthFactor> GetAll();

        LengthFactor Get(int id);

        void SaveChanges();

        void Truncate();

        void Update(LengthFactor item);
    }
}
