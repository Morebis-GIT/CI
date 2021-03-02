using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace xggameplan.Updates
{
    /// <summary>
    /// Update for XGGP-752
    /// </summary>
    public class UpdateXGGP752 : UpdateBase, IUpdate
    {
        private List<string> _tenantConnectionStrings;

        public UpdateXGGP752(UpdateConfiguration configuration)
        {
            _tenantConnectionStrings = configuration.TenantConnectionStrings.ToList();
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateStepXGGP752_XGGP752(_tenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id
        {
            get { return new Guid("84efaccf-ddad-440d-b1b9-9f4ca29f7f4f"); }
        }

        public override string Name
        {
            get { return "XGGP-752"; }
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
