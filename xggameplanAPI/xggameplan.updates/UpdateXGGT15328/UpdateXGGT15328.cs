using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT15328 : UpdateBase, IUpdate
    {
        public UpdateXGGT15328(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT15328_AddSAPPTargetAreaNameFeatureFlag(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("28fd768d-0912-4d4e-aa0f-f6d1266ef582");

        public override string Name => "XGGT-15328";

        public override string DatabaseVersion => "";
    }
}
