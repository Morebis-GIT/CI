using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT13949 : UpdateBase, IUpdate
    {
        public UpdateXGGT13949(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT14246_AddLandmarkBookingProductFeature(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public override Guid Id => new Guid("0b66e85d-c36c-46c8-874d-4b3b7260bf9a");

        public override string Name => "XGGT-13949";

        public override string DatabaseVersion => "";
    }
}
