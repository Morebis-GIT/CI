using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT14867 : UpdateBase, IUpdate
    {
        public UpdateXGGT14867(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT14867_AddPaginatedCampaignViewProductFeature(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("55d0860d-6afd-4d80-8db8-b96c36b07010");

        public override string Name => "XGGT-14867";

        public override string DatabaseVersion => "";
    }
}
