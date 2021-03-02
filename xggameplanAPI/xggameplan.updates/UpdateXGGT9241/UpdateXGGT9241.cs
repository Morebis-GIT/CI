using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates.UpdateXGGT9241
{
    public class UpdateXGGT9241 : UpdateBase, IUpdate
    {
        public UpdateXGGT9241(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT9241_AddSpotBookingRuleFeature(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("68372213-8B8B-4AEB-A384-0802B9DA5C8A");

        public override string Name => "XGGT-9241";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
