using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT18176 : UpdateBase, IUpdate
    {
        public UpdateXGGT18176(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT18176_AddCampaignLevelScenarioCampaignResultsFacility(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("c8b63934-f212-40e9-9b33-dd276bb0cc0e");

        public override string Name => "XGGT-18176";

        public override string DatabaseVersion => "";
    }
}
