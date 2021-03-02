using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGP10302 : UpdateBase, IUpdate
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateXGGP10302(UpdateConfiguration configuration)
        {
            _tenantConnectionStrings = configuration.TenantConnectionStrings;
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateXGGP10302_UpdateProgrammeDictionaryDocument(_tenantConnectionStrings, _updatesFolder, configuration.Mapper)
            };
        }

        public override Guid Id => new Guid("B79FD7AE-B4E9-47CC-85A5-3E128E8DE477");

        public override string Name => "XGGP-10302";

        public override string DatabaseVersion
        {
            get { return ""; }
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
