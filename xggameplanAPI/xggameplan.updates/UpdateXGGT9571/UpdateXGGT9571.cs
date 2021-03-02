using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT9571 : UpdateBase, IUpdate
    {
        public UpdateXGGT9571(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT9571_UpdateMaxRatingsRulesValues(configuration.TenantConnectionStrings, _updatesFolder)
            };
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();

        public sealed override Guid Id => new Guid("25d1b926-66db-4c8d-b020-383d98d51d39");

        public override string Name => "XGGT-9571";

        public override string DatabaseVersion { get; } = "";
    }
}
