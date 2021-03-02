namespace xggameplan.SystemTasks
{
    /// <summary>
    /// Result for a system task
    /// </summary>
    public class SystemTaskResult
    {
        /// <summary>
        /// Result types, indicate whether there is an issue
        /// </summary>
        public enum ResultTypes : byte
        {
            Information = 0,
            Warning = 1,
            Error = 2
        }

        /// <summary>
        /// Task
        /// </summary>

        public string TaskId { get; }

        /// <summary>
        /// Result type
        /// </summary>
        public ResultTypes ResultType { get; }
        
        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; }        

        public SystemTaskResult(ResultTypes resultType, string taskId, string message)
        {
            TaskId = taskId;
            ResultType = resultType;
            Message = message;            
        }
    }
}