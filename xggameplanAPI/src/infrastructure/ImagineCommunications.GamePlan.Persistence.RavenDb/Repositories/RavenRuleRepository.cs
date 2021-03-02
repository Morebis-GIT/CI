using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Rules;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenRuleRepository : IRuleRepository, IDisposable
    {
        private readonly IDocumentSession _session;

        public RavenRuleRepository(IDocumentSession session)
        {
            _session = session;
        }

        public void Add(Rule rule)
        {
            lock (_session)
            {
                _session.Store(rule);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var rule = Get(id);

                if (rule is null)
                {
                    return;
                }

                _session.Delete(rule);
            }
        }

        public IEnumerable<Rule> FindByRuleTypeId(int ruleTypeId)
        {
            return _session.GetAll<Rule>(s => s.RuleTypeId == ruleTypeId);
        }

        public IEnumerable<Rule> FindByRuleTypeIds(IEnumerable<int> ruleTypeIds)
        {
            return _session.GetAll<Rule>(s => s.RuleTypeId.In(ruleTypeIds.ToList()));
        }

        public Rule Get(int id) => _session.Load<Rule>(id);

        public IEnumerable<Rule> GetAll() => _session.GetAll<Rule>();

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
                WaitForIndexes();
            }
        }

        public void Update(Rule rule)
        {
            lock (_session)
            {
                _session.Store(rule);
            }
        }

        public void Update(IEnumerable<Rule> rules)
        {
            if (rules is null)
            {
                return;
            }

            lock (_session)
            {
                foreach (var rule in rules.Where(r => r != null))
                {
                    _session.Store(rule);
                }
            }
        }

        private void WaitForIndexes()
        {
            _session.WaitForIndexes<Rule>(
                indexName: Rules_Default.DefaultIndexName,
                isMapReduce: false,
                testExpression: p => p.Id == 0);
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
