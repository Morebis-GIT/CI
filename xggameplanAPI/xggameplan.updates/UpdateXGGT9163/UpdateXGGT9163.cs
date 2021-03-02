using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT9163 : UpdateBase, IUpdate
    {
        public UpdateXGGT9163(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT9163_NullCampaignPassPrioritiesPatch(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("e84cc113-095c-4d2f-9eec-9e72536855fd");

        public override string Name => "XGGT-9163";

        public override string DatabaseVersion { get; } = "";
    }
}
