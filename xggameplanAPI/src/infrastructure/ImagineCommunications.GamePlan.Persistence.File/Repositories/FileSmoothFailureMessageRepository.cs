using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileSmoothFailureMessageRepository
        : FileRepositoryBase, ISmoothFailureMessageRepository
    {
        public FileSmoothFailureMessageRepository(string folder)
            : base(folder, "smooth_failure_message")
        { }

        public void Dispose()
        { }

        public IEnumerable<SmoothFailureMessage> GetAll() =>
            GetAllByType<SmoothFailureMessage>(_folder, _type);

        public void Add(IEnumerable<SmoothFailureMessage> items)
        {
            InsertItems(
                _folder,
                _type,
                items.ToList(),
                items.Select(i => i.Id.ToString()).ToList()
                );
        }

        public void Truncate() => throw new NotImplementedException();

        public void SaveChanges() => throw new NotImplementedException();
    }
}
