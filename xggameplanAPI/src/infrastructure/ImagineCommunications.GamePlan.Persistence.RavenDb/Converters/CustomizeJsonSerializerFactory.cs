using System;
using System.Collections.Generic;
using Raven.Imports.Newtonsoft.Json;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Converters
{
    /// <summary>
    /// Stores actions to customize json serializer.
    /// </summary>
    public class CustomizeJsonSerializerFactory
    {
        private readonly List<Action<JsonSerializer>> _actions = new List<Action<JsonSerializer>>();

        public void Add(Action<JsonSerializer> action)
        {
            _actions.Add(action);
        }

        public void Execute(JsonSerializer serializer)
        {
            _actions.ForEach(x => x(serializer));
        }
    }
}
