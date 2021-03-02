using System;
using System.Collections.Generic;
using System.IO;
using xggameplan.Updates;

namespace xggameplan.updates
{
    public class UpdateXGGT13062 : UpdateBase, IUpdate
    {
        public UpdateXGGT13062(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT13062_RenamePassTarpsCollectionToRatingPoints(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("63E790FA-90CB-480F-8991-BD3E5BBBBB37");

        public override string Name => "XGGT-13062";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid> { new Guid("ab655b68-d126-403e-88bb-2c69e8353f0c") };
    }
}
