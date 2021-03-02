using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT3727 : UpdateBase, IUpdate
    {
        public UpdateXGGT3727(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStep3727_ScenarioCampaignDemographicNamePatch(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("d62ccf8a-4775-4e79-8206-7a23f9aa49b5");

        public override string Name => "XGGT-3727";

        public override string DatabaseVersion { get; } = "";
    }
}
