namespace xggameplan.AutoBooks.Abstractions
{
    /// <summary>
    /// Interface for communication with Environments API (Provisioning API)
    /// </summary>
    public interface IEnvironmentAPI<TEnvDetails>
    {
        TEnvDetails Get();

        /// <summary>
        /// Creates the new environment
        /// </summary>
        void Create();

        /// <summary>
        /// Deletes the environment
        /// </summary>
        void Delete();
    }
}
