using System;
using ImagineCommunications.GamePlan.Domain.Shared.System.Preview;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Tenants
{
    public class Tenant
    {
        private Tenant()
        {
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public string DefaultTheme { get; private set; }

        public DatabaseProviderConfiguration TenantDb { get; private set; }

        public PreviewFile Preview { get; set; }

        public static Tenant Create( int id, string name, string defaultTheme,
                                     DatabaseProviderConfiguration tenantDb){

            Validate(name, defaultTheme, tenantDb);

            return new Tenant(){
                Id = id,
                Name = name,
                DefaultTheme = defaultTheme,
                TenantDb = tenantDb
            };
        }

        private static void Validate(string name, string defaultTheme,
                                     DatabaseProviderConfiguration tenantDb)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (String.IsNullOrWhiteSpace(defaultTheme))
            {
                throw new ArgumentNullException(nameof(defaultTheme));
            }
            if (tenantDb == null)
            {
                throw new ArgumentNullException(nameof(tenantDb));
            }
        }

        public void Change(string name, string defaultTheme,
                           DatabaseProviderConfiguration tenantDb){

            Validate(name,defaultTheme,tenantDb);
            Name = name;
            DefaultTheme = defaultTheme;
            TenantDb = tenantDb;
        }
    }
}
