using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT10421
{
    public class UpdateXGGT10421 : UpdateBase, IUpdate
    {
        public UpdateXGGT10421(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT10421_MigrateProgrammeCategories(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }
        public override Guid Id => new Guid("D5220B68-7DE7-4B4A-AE27-09EAF8294300");

        public override string Name => "XGGT-10421";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
