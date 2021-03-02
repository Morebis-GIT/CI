using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT13203
{
    public class UpdateXGGT13203 : UpdateBase, IUpdate
    {
        public UpdateXGGT13203(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT13203_Delivery_Currency(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("EE0F6359-1E1B-4363-899A-5660D4B01733");

        public override string Name => "XGGT-13203";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
