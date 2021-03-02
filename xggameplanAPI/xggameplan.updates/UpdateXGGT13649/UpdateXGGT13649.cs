using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT13649 : UpdateBase, IUpdate
    {
        public UpdateXGGT13649(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT13649_ProductChanges(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("cfcec756-8940-4f23-8adb-e063a32bda68");

        public override string Name => "XGGT-13649";

        public override string DatabaseVersion => "";
    }
}
