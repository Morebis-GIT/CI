using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT14140 : UpdateBase, IUpdate
    {
        public UpdateXGGT14140(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT14140_UpdateAutoBookInstanceConfigurations(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("10a4c6d9-0530-4299-aae6-0df91071631e");

        public override string Name => "XGGT-14140";

        public override string DatabaseVersion { get; } = "";
    }
}
