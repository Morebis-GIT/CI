using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT7600 : UpdateBase, IUpdate
    {
        public UpdateXGGT7600(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT7600_MapCampaignPassPriority(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("16fa3aba-e71e-4142-b6a6-d6c7bc7ea37c");

        public override string Name => "XGGT-7600";

        public override string DatabaseVersion { get; } = "";
    }
}
