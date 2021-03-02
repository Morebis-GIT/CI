using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT15514
{
    public class UpdateXGGT15514 : UpdateBase, IUpdate
    {
        public UpdateXGGT15514(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);

            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT15514_FixFitToSpotLengthRuleTypo(
                    configuration.TenantConnectionStrings)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("56E46356-CED3-4570-B9B5-1281A17211F9");

        public override string Name => "XGGT-15514";

        public override string DatabaseVersion => "";
    }
}
