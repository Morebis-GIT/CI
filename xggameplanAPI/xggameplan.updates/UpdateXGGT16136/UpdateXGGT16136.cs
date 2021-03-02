using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT16136 : UpdateBase, IUpdate
    {
        public UpdateXGGT16136(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT16136_RemoveKPIsFromBRSTemplates(configuration.TenantConnectionStrings, _updatesFolder),
                new UpdateStepXGGT16136_UpdateKPIComparisonConfigurations(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid> { new Guid("89953B63-FE6E-486B-9226-CA636A07BC03") };

        public override Guid Id => new Guid("ae949b76-c846-4c6f-9b03-bdf1be307216");

        public override string Name => "XGGT-16136";

        public override string DatabaseVersion => "";
    }
}
