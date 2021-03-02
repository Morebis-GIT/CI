using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT14246 : UpdateBase, IUpdate
    {
        public UpdateXGGT14246(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT14246_AddLandmarkBookingProductFeature(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("17d238ba-1aa6-4f1a-9cbd-dc841d9d381c");

        public override string Name => "XGGT-14246";

        public override string DatabaseVersion => "";
    }
}
