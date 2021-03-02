using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace xggameplan.Updates
{
    /// <summary>
    /// Update for XGGP-885
    /// </summary>
    public class UpdateXGGP885 : UpdateBase, IUpdate
    {
        private List<string> _tenantConnectionStrings;

        public UpdateXGGP885(UpdateConfiguration configuration)
        {
            _tenantConnectionStrings = configuration.TenantConnectionStrings.ToList();
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateXGGP885_XGGP885(_tenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id
        {
            get { return new Guid("78e2b3e5-e968-472b-9c04-b210a2937685"); }
        }

        public override string Name
        {
            get { return "XGGP-885"; }
        }

        public override string DatabaseVersion
        {
            get { return ""; }    // TODO: Set this
        }

        public List<Guid> DependsOnUpdates
        {
            get { return new List<Guid>(); }
        }
    }
}
