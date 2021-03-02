using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT9170 : UpdateBase, IUpdate
    {
        public UpdateXGGT9170(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT9170_SyncScenarioCampaignsWihActualCampaigns(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("40762772-20F2-44D3-8A24-6E3F14BE3D32");

        public override string Name => "XGGT-9170";

        public override string DatabaseVersion { get; } = "";
    }
}
