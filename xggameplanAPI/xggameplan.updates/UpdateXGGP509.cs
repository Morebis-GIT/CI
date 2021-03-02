using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace xggameplan.Updates
{
    /// <summary>
    /// Update for XGGP-509
    /// </summary>
    public class UpdateXGGP509 : UpdateBase, IUpdate
    {
        private List<string> _tenantConnectionStrings;

        public UpdateXGGP509(UpdateConfiguration configuration)
        {
            _tenantConnectionStrings = configuration.TenantConnectionStrings.ToList();
            _updatesFolder = Path.Combine(configuration.UpdatesFolderRoot, this.Id.ToString());
            _ = Directory.CreateDirectory(_updatesFolder);
            _updateSteps = new List<IUpdateStep>()
            {
                new UpdateXGGP509_XGGP791(_tenantConnectionStrings, _updatesFolder)
            };
        }

        public override Guid Id
        {
            get { return new Guid("ee2460ac-1650-41e5-808e-af9ddb2212aa"); }
        }

        public override string Name
        {
            get { return "XGGP-509"; }
        }

        public override string DatabaseVersion
        {
            get { return ""; }    // TODO: Set this
        }

        public List<Guid> DependsOnUpdates
        {
            get { return new List<Guid>() { new Guid("98ef8be0-495e-4397-97e3-6372d28dd4e0") }; }       // UpdateXGGP337 (Create separate Scenario & Pass repositories)            
        }
    }
}
