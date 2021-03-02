using System.Collections.Generic;
using Autofac;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using Microsoft.Extensions.Configuration;
using xggameplan.AuditEvents;
using xggameplan.common.Email;

namespace xggameplan.core.DependencyInjection
{
    public class DefaultAuditEventModule : Module
    {
        private readonly string _csvLogsFolder;

        public DefaultAuditEventModule(string csvLogsFolder)
        {
            _csvLogsFolder = csvLogsFolder;
        }

        protected override void Load(ContainerBuilder builder)
        {
            _ = builder.RegisterType<AuditEventTypeRepository>().As<IAuditEventTypeRepository>().SingleInstance();
            _ = builder.RegisterType<AuditEventValueTypeRepository>().As<IAuditEventValueTypeRepository>().SingleInstance();

            _ = builder.Register(context =>
                  {
                      return new CSVConfiguration(
                          context.Resolve<IAuditEventTypeRepository>(),
                          context.Resolve<IAuditEventValueTypeRepository>(),
                          null,
                          _csvLogsFolder
                      ).GetAuditEventRepository();
                  })
                .Named<IAuditEventRepository>("csv")
                .SingleInstance();

            _ = builder.Register(context =>
                  {
                      var applicationConfiguration = context.Resolve<IConfiguration>();
                      return new EmailConfiguration(
                          context.Resolve<IEmailAuditEventSettingsRepository>(),
                          applicationConfiguration["Environment:Id"],
                          applicationConfiguration["Frontend:Url"],
                          context.Resolve<IRepositoryFactory>(),
                          context.Resolve<IEmailConnection>()).GetAuditEventRepository();
                  })
                .Named<IAuditEventRepository>("email")
                .InstancePerLifetimeScope();

            _ = builder.Register(context =>
                  {
                      var applicationConfiguration = context.Resolve<IConfiguration>();
                      return new MSTeamsConfiguration(
                          context.Resolve<IAuditEventTypeRepository>(),
                          context.Resolve<IMSTeamsAuditEventSettingsRepository>(),
                          applicationConfiguration["Frontend:Url"],
                          context.Resolve<IRepositoryFactory>()
                      ).GetAuditEventRepository();
                  })
                .Named<IAuditEventRepository>("msteams")
                .InstancePerLifetimeScope();

            _ = builder.Register(context => new MasterAuditEventRepository(new List<IAuditEventRepository>
                {
                    context.ResolveNamed<IAuditEventRepository>("csv"),
                    context.ResolveNamed<IAuditEventRepository>("email"),
                    context.ResolveNamed<IAuditEventRepository>("msteams")
                }))
                .As<IAuditEventRepository>()
                .InstancePerLifetimeScope();
        }
    }
}
