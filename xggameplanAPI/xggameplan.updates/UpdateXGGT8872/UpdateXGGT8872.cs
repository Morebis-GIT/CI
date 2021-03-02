using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates.UpdateXGGT8872
{
    public class UpdateXGGT8872 : UpdateBase, IUpdate
    {
        public UpdateXGGT8872(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT8872_AddSpotBasedDeliveryRules(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("5B81437C-C1BD-425E-AFAB-DD6DDD862B07");

        public override string Name => "XGGT-8872";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
