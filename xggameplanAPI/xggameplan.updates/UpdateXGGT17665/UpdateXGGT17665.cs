using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT17665
{
    public class UpdateXGGT17665 : UpdateBase, IUpdate
    {
        public UpdateXGGT17665(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT17665_ProgrammeOptimization(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("5C9BBCF3-EB74-4033-A7E2-D3D0086BF667");

        public override string Name => "XGGT-17665";

        public override string DatabaseVersion => "";
    }
}
