using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates.UpdateXGGT9876
{
    public class UpdateXGGT9876 : UpdateBase, IUpdate
    {
        public UpdateXGGT9876(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateXGGT9876_AddDefaultFeatures(configuration.MasterConnectionString, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("5F13E864-217D-46BA-BA15-44E9B38E3A60");

        public override string Name => "XGGT-9876";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
