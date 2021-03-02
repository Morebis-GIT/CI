using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.updates.UpdateXGGT13656;

namespace xggameplan.Updates.UpdateXGGT13656
{
    public class UpdateXGGT13656 : UpdateBase, IUpdate
    {
        public UpdateXGGT13656(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT13656_AddNewCampaignKPIColumns(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("28cf7ec4-1382-46b6-afb3-a2264f38acfa");

        public override string Name => "XGGT-13656";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
