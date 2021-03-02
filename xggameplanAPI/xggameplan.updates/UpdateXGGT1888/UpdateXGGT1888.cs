using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT1888 : UpdateBase, IUpdate
    {
        public UpdateXGGT1888(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT1888_AddingDefaultAutopilotSettings(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid> {new Guid("EA7B2EC9-9620-4D73-ABFE-C413C54272D2")};

        public override Guid Id => new Guid("1C9E7E74-222C-4C66-902A-4ECB697D3965");

        public override string Name => "XGGT-1888";

        public override string DatabaseVersion => "";
    }
}
