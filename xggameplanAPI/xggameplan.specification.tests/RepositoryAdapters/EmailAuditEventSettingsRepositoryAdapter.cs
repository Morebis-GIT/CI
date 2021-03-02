using System;
using System.Collections.Generic;
using xggameplan.AuditEvents;
using xggameplan.specification.tests.Infrastructure.TestAdapters;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.RepositoryAdapters
{
    public class
        EmailAuditEventSettingsRepositoryAdapter : RepositoryTestAdapter<EmailAuditEventSettings,
            IEmailAuditEventSettingsRepository, int>
    {
        public EmailAuditEventSettingsRepositoryAdapter(IScenarioDbContext dbContext,
            IEmailAuditEventSettingsRepository repository) : base(dbContext, repository)
        {
        }

        protected override EmailAuditEventSettings Add(EmailAuditEventSettings model) =>
            throw new NotImplementedException();

        protected override IEnumerable<EmailAuditEventSettings> AddRange(params EmailAuditEventSettings[] models)
        {
            Repository.AddRange(models);
            return models;
        }

        protected override EmailAuditEventSettings Update(EmailAuditEventSettings model)
        {
            Repository.Update(model);
            return model;
        }

        protected override EmailAuditEventSettings GetById(int id) => throw new NotImplementedException();

        protected override IEnumerable<EmailAuditEventSettings> GetAll() => Repository.GetAll();

        protected override void Delete(int id) => Repository.Delete(id);

        protected override void Truncate() => Repository.Truncate();

        protected override int Count() => throw new NotImplementedException();
    }
}
