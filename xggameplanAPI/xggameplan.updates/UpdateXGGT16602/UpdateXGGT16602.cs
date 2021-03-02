using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates.UpdateXGGT16602
{
    public class UpdateXGGT16602 : UpdateBase, IUpdate
    {
        public UpdateXGGT16602(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT16602_AddSpotBasedAutopilotRules(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("30DB8DD8-BCCE-4435-8126-A8913BE1D3F2");

        public override string Name => "XGGT-16602";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
