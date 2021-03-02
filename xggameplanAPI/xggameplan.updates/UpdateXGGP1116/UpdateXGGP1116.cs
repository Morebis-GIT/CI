using System;
using System.Collections.Generic;
using System.IO;

namespace xggameplan.Updates
{
    public class UpdateXGGP1116 : UpdateBase, IUpdate
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;

        public UpdateXGGP1116(UpdateConfiguration configuration)
        {
            _tenantConnectionStrings = configuration.TenantConnectionStrings;
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateXGGP1116_XGGP1116(_tenantConnectionStrings, _updatesFolder, configuration.Mapper)
            };
        }

        public override Guid Id => new Guid("b29f3421-caef-4fb5-937d-a930ce75852a");

        public override string Name => "XGGP-1116";

        public override string DatabaseVersion
        {
            get { return ""; }    // TODO: Set this
        }

        public List<Guid> DependsOnUpdates => new List<Guid>();
    }
}
