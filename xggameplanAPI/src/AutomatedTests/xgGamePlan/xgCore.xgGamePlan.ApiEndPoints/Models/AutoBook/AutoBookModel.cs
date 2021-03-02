using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.AutoBook
{
    public class AutoBookModel
    {
        public string Id { get; set; }

        public string Api { get; set; }

        public AutoBookStatuses Status { get; set; }

        public AutoBookTaskModel Task { get; set; }

        public string Version { get; set; }

        public int InstanceConfigurationId { get; set; }

        public override bool Equals(object obj) =>
            obj is AutoBookModel model &&
            Id == model.Id &&
            Api == model.Api &&
            Status == model.Status &&
            EqualityComparer<AutoBookTaskModel>.Default.Equals(Task, model.Task) &&
            Version == model.Version &&
            InstanceConfigurationId == model.InstanceConfigurationId;

        public override int GetHashCode() => 0;
    }
}
