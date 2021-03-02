using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenAutopilotRuleRepository : IAutopilotRuleRepository, IDisposable
    {
        private readonly IDocumentSession _session;

        public RavenAutopilotRuleRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(AutopilotRule autopilotRule)
        {
            lock (_session)
            {
                _session.Store(autopilotRule);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var item = Get(id);
                if (item is null)
                {
                    return;
                }

                _session.Delete(item);
            }
        }

        public void Delete(IEnumerable<int> ids)
        {
            lock (_session)
            {
                var rules = _session.GetAll<AutopilotRule>(s => s.Id.In(ids.ToList()));
                foreach (var rule in rules)
                {
                    _session.Delete(rule);
                }
            }
        }

        public AutopilotRule Get(int id) => _session.Load<AutopilotRule>(id);

        public IEnumerable<AutopilotRule> GetAll() => _session.GetAll<AutopilotRule>();

        public IEnumerable<AutopilotRule> GetByFlexibilityLevelId(int id) => _session.GetAll<AutopilotRule>(r => r.FlexibilityLevelId == id);

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Update(AutopilotRule autopilotRule)
        {
            lock (_session)
            {
                _session.Store(autopilotRule);
            }
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
