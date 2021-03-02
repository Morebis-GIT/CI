using System;

namespace ImagineCommunications.GamePlan.Domain.Scenarios.Objects
{
    /// <summary>
    /// Object fields need to be camelCase otherwise they do not map within the web app
    /// </summary>
    public class ScenarioNotificationModel
    {
        public Guid id { get; set; }

        public string status { get; set; }

        public int totalSteps { get; set; }
        public int currentStep { get; set; }
    }
}