using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT3810 : UpdateBase, IUpdate
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateXGGT3810(UpdateConfiguration configuration)
        {
            _tenantConnectionStrings = configuration.TenantConnectionStrings;
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateStepXGGT3810_AddPassNewTolerance(_tenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("144A5E16-0685-44D4-87B3-E7918A9E3311");

        public override string Name => "XGGT-3810";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
