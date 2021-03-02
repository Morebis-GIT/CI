using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT10786 : UpdateBase, IUpdate
    {
        public UpdateXGGT10786(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT10786_FunctionalAreaTarpsFaultTypeInsert(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("ca690e78-9fc6-48de-9d0e-c23222ad7dfd");

        public override string Name => "XGGT-10786";

        public override string DatabaseVersion { get; } = "";
    }
}
