namespace xggameplan.SystemTests
{
    /// <summary>
    /// Result for a system test
    /// </summary>
    public class SystemTestResult
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
        /// Result type
        /// </summary>
        public ResultTypes ResultType { get; }

        /// <summary>
        /// Category, for grouping results
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Optional link for more information
        /// </summary>
        public string Link { get; }  

        public SystemTestResult(ResultTypes resultType, string category, string message, string link)
        {
            Category = category;
            ResultType = resultType;
            Message = message;
            Link = link;
        }
    }

    /// <summary>
    /// System test categories, allows different sets of tests to be executed
    /// </summary>
    public enum SystemTestCategories : byte
    {
        /// <summary>
        /// Default set of tests (Everything)
        /// </summary>
        Default = 0,

        /// <summary>
        /// Tests for deployment. For these tests then we don't care about things such as flagging up historic runs that have
        /// failed, we only care that this installation has worked.
        /// </summary>
        Deployment = 1
    }
}