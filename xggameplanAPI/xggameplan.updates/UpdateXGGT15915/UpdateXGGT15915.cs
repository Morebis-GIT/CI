using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT15915
{
    public class UpdateXGGT15915 : UpdateBase, IUpdate
    {
        public UpdateXGGT15915(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT15915_RemoveMapBreakWithProgrammesByExternalRefFeatureFlag(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("78AE1CA3-6BF4-4113-BF06-B7B4CB935672");

        public override string Name => "XGGT-15915";

        public override string DatabaseVersion => "";
    }
}
