using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT14638
{
    public class UpdateXGGT14638 : UpdateBase, IUpdate
    {
        public UpdateXGGT14638(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);

            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT14638_AddFiveNewWeightingRules(
                    configuration.TenantConnectionStrings)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("A89EEB39-320B-4DAA-84EA-FCC800D355EC");

        public override string Name => "XGGT-14638";

        public override string DatabaseVersion => "";
    }
}
