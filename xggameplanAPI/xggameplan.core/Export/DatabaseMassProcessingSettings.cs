using System;
using System.Collections.Generic;

namespace xggameplan.Database
{
    /// <summary>
    /// Settings for exporting a database
    /// </summary>
    public abstract class DatabaseMassProcessingSettings
    {
        private readonly Dictionary<Type, Action<object>> transformDataDocumentFunctions = new Dictionary<Type, Action<object>>();
        private Dictionary<Type, Func<object, bool>> documentTypeFilterFunctions = new Dictionary<Type, Func<object, bool>>();
        private List<Type> documentTypesToProcess = new List<Type>();

        public string DataFolder { get; protected set; }

        /// <summary>
        /// Document types to export
        /// </summary>
        public List<Type> GetDocumentTypesToProcess() => documentTypesToProcess;

        /// <summary>
        /// Document types to export
        /// </summary>
        protected void SetDocumentTypesToProcess(List<Type> value) => documentTypesToProcess = value;

        /// <summary>
        /// Function to transform data documents (E.g. Clear properties, set defaults)
        /// </summary>
        public Dictionary<Type, Action<object>> GetTransformDataDocumentFunctions() => transformDataDocumentFunctions;

        public Action<object> GetTransformDataDocumentFunction(Type type) =>
            transformDataDocumentFunctions.ContainsKey(type) ? transformDataDocumentFunctions[type] : null;
        
        /// <summary>
        /// Filters for documents
        /// </summary>
        public Dictionary<Type, Func<object, bool>> GetDocumentTypeFilterFunctions() => documentTypeFilterFunctions;

        public Func<object, bool> GetDocumentTypeFilterFunction(Type type) =>
            documentTypeFilterFunctions.ContainsKey(type)? documentTypeFilterFunctions[type]: null;

        /// <summary>
        /// Filters for documents
        /// </summary>
        protected void SetDocumentTypeFilterFunctions(Dictionary<Type, Func<object, bool>> value) => documentTypeFilterFunctions = value;
    }
}
