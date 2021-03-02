using Newtonsoft.Json;

namespace xggameplan.model.External
{
    public class ErrorModel
    {
        public string ErrorCode { get; set; }
        [JsonIgnore]
        public string ErrorField { get; set; }
        public string ErrorMessage { get; set; }
    }
}
