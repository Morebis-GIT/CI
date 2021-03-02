using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT12303 : UpdateBase, IUpdate
    {
        public UpdateXGGT12303(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT12303_AddRunCampaignListOnCreationProductFeature(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("d0267e53-cd47-4ad4-b6f3-b11fece3ebbf");

        public override string Name => "XGGT-12303";

        public override string DatabaseVersion => "";
    }
}
