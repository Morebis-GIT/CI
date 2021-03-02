using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates.UpdateXGGT13801
{
    public class UpdateXGGT13801 : UpdateBase, IUpdate
    {
        public UpdateXGGT13801(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT13801_AddTargetZeroRatedBreaksColumn(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("851b1a9c-6b2d-4202-8dc9-7b4fc5186c95");

        public override string Name => "XGGT-13801";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
