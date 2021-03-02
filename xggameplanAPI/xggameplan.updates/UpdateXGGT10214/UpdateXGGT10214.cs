using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT10214 : UpdateBase, IUpdate
    {
        public UpdateXGGT10214(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT10214_UpdatePassTarpsEmptyCollection(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("ab655b68-d126-403e-88bb-2c69e8353f0c");

        public override string Name => "XGGT-10214";

        public override string DatabaseVersion => "";
    }
}
