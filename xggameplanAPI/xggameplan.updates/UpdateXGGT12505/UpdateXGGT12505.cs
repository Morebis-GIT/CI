using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates.UpdateXGGT12505
{
    public class UpdateXGGT12505 : UpdateBase, IUpdate
    {
        public UpdateXGGT12505(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT12505_AddNewCampaignColumns(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("00ed2bb7-adde-41a7-a1d8-f82faa901e26");

        public override string Name => "XGGT-12505";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
