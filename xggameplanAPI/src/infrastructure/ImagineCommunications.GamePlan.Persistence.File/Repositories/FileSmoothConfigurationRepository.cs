using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileSmoothConfigurationRepository
        : FileRepositoryBase, ISmoothConfigurationRepository
    {
        public FileSmoothConfigurationRepository(string folder)
            : base(folder, "smooth_configuration")
        {
        }

        public void Dispose()
        {
        }

        public SmoothConfiguration GetById(int id)
        {
            return GetItemByID<SmoothConfiguration>(_folder, _type, id.ToString());
        }

        public void Add(SmoothConfiguration smoothConfiguration)
        {
            var items = new List<SmoothConfiguration>() { smoothConfiguration };
            InsertItems(_folder, _type, items, items.Select(i => i.Id.ToString()).ToList());
        }

        public void Update(SmoothConfiguration smoothConfiguration)
        {
            UpdateOrInsertItem(_folder, _type, smoothConfiguration, smoothConfiguration.Id.ToString());
        }

        public void SaveChanges()
        {
        }

        public void Truncate() => throw new System.NotImplementedException();
    }
}
