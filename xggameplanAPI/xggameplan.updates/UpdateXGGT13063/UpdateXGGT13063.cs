using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT13063 : UpdateBase, IUpdate
    {
        public UpdateXGGT13063(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT13063_RenameTarpsFaultType(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public sealed override Guid Id => new Guid("e35f0784-62c0-4851-a6c7-e67f03d04fc8");

        public override string Name => "XGGT-13063";

        public List<Guid> DependsOnUpdates => new List<Guid>{ new Guid("ca690e78-9fc6-48de-9d0e-c23222ad7dfd") };

        public override string DatabaseVersion { get; } = "";
    }
}
