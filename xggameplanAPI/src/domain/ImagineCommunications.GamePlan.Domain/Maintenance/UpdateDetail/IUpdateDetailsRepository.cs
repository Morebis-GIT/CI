using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Maintenance.UpdateDetail
{
    public interface IUpdateDetailsRepository
    {
        List<UpdateDetails> GetAll();

        UpdateDetails Find(Guid id);

        void Add(UpdateDetails update);

        void Update(UpdateDetails update);

        void Remove(Guid id);

        void SaveChanges();
    }
}
