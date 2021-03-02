using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenRuleTypeRepository : IRuleTypeRepository, IDisposable
    {
        private readonly IDocumentSession _session;

        public RavenRuleTypeRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(RuleType ruleType)
        {
            lock (_session)
            {
                _session.Store(ruleType);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var ruleType = Get(id);

                if (ruleType is null)
                {
                    return;
                }

                _session.Delete(ruleType);
            }
        }

        public RuleType Get(int id) => _session.Load<RuleType>(id);

        public IEnumerable<RuleType> GetAll(bool onlyAllowedAutopilot = false)
        {
            return _session
                .GetAll<RuleType>()
                .Where(p => onlyAllowedAutopilot == false || p.AllowedForAutopilot)
                .ToList();
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Update(RuleType ruleType)
        {
            lock (_session)
            {
                _session.Store(ruleType);
            }
        }

        public void Update(IEnumerable<RuleType> ruleTypes)
        {
            if (ruleTypes is null)
            {
                return;
            }

            lock (_session)
            {
                foreach (var ruleType in ruleTypes.Where(rt => rt != null))
                {
                    _session.Store(ruleType);
                }
            }
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
