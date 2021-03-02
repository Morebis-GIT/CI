using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT1519 : UpdateBase, IUpdate
    {
        public UpdateXGGT1519(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT1519_AddDefaultPeakExposureCountAndDefaultOffPeakExposureCount(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("3f5d7aa7-9a9d-407a-89b1-8d772fd91ebb");

        public override string Name => "XGGT-1519";

        public override string DatabaseVersion => "";
    }
}
