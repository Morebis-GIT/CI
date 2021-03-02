using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGT4255 : UpdateBase, IUpdate
    {
        private IEnumerable<string> _tenantConnectionStrings;

        public UpdateXGGT4255(UpdateConfiguration configuration)
        {
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());
            _tenantConnectionStrings = configuration.TenantConnectionStrings;

            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>
            {
                new UpdateStepXGGT4255_AddPassNewGeneralRules(_tenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id => new Guid("A25F3716-8BE5-4178-9412-8125A75EE40C");

        public override string Name => "XGGT-4255";

        public override string DatabaseVersion => "";

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
