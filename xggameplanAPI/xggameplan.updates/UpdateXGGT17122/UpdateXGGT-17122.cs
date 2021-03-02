using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT17122
{
    internal class UpdateXGGT_17122 : UpdateBase, IUpdate
    {
        public UpdateXGGT_17122(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT17122_NullPassStartEndTime(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("61c9f64f-5fc4-4b40-8c38-74a3251b3858");

        public override string Name => "XGGT-17122";

        public override string DatabaseVersion => "";
    }
}
