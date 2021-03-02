using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT10224 : UpdateBase, IUpdate
    {
        public UpdateXGGT10224(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT10244_AddMidnightStartAndEndTimeToTenantSettings(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("c7641a05-ac67-4fe2-a4b5-3886fc10822f");

        public override string Name => "XGGT-10224";

        public override string DatabaseVersion => "";
    }
}
