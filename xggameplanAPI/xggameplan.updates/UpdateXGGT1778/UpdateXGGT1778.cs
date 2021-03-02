using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT1778 : UpdateBase, IUpdate
    {
        private IEnumerable<string> _tenantConnectionStrings;

        public UpdateXGGT1778(UpdateConfiguration configuration)
        {
            _tenantConnectionStrings = configuration.TenantConnectionStrings;
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateStepXGGT1778_AddPassNewGeneralRule(_tenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("EA9EEA6D-F0BB-48A3-8B95-72976CDBEE44");

        public override string Name => "XGGT-1778";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
