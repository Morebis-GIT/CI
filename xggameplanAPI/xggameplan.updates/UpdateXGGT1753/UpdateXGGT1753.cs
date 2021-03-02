using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT1753 : UpdateBase, IUpdate
    {
        public UpdateXGGT1753(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT1753_AddingRulesRepositories(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("EA7B2EC9-9620-4D73-ABFE-C413C54272D2");

        public override string Name => "XGGT-1753";

        public override string DatabaseVersion => "";
    }
}
