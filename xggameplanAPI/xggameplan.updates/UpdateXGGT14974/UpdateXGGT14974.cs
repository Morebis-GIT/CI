using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT14974 : UpdateBase, IUpdate
    {
        public UpdateXGGT14974(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT14974_AddBusinessTypes(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("442ae720-8084-4724-800e-90d0cbae1bea");

        public override string Name => "XGGT-14974";

        public override string DatabaseVersion => "";
    }
}
