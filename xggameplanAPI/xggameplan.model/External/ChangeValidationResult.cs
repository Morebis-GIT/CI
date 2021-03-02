namespace xggameplan.Model
{
    /// <summary>
    /// Result for change validation
    /// </summary>
    public class ChangeValidationResult
    {
        public enum ResultTypes : byte
        {
            Warning = 0,
            Error = 1
        }

        /// <summary>
        /// Result type
        /// </summary>
        public ResultTypes ResultType { get; set; }

        /// <summary>
        /// Related item
        /// </summary>
        public object Item { get; set; }

        /// <summary>
        /// Free format message
        /// </summary>
        public string Message { get; set; }

        public ChangeValidationResult(ResultTypes resultType, object item, string message)
        {
            ResultType = resultType;
            Item = item;
            Message = message;
        }
    }
}
