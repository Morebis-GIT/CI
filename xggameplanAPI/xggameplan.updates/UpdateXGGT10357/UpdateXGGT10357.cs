using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT10357
{
    public class UpdateXGGT10357 : UpdateBase, IUpdate
    {
        public UpdateXGGT10357(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT10357_AddKPIPrioritiesAndDefaultBRSConfigurationTemplate(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("4EF5C7D0-397D-4A6E-A4A8-1009DC9EFB0D");

        public override string Name => "XGGT-10357";

        public override string DatabaseVersion => "";
    }
}
