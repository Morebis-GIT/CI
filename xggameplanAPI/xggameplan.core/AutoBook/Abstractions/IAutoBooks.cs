using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.InstanceConfiguration.Objects;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using xggameplan.Model;
using AutoBookDomainObject = ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects.AutoBook;

namespace xggameplan.AutoBooks.Abstractions
{
    /// <summary>
    /// Interface for managing all AutoBook instances collectively.
    /// </summary>
    public interface IAutoBooks
    {
        /// <summary>
        /// Creates new AutoBook instance
        /// </summary>
        /// <returns></returns>
        void Create(AutoBookDomainObject autoBook);

        /// <summary>
        /// Deletes AutoBook instance
        /// </summary>
        /// <param name="autoBook"></param>
        void Delete(AutoBookDomainObject autoBook);

        /// <summary>
        /// Restarts AutoBook instance
        /// </summary>
        /// <param name="autoBook"></param>
        void Restart(AutoBookDomainObject autoBook);

        List<AutoBookDomainObject> RestartNonWorking();

        /// <summary>
        /// Returns AutoBook instance that is currently processing run
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        AutoBookDomainObject GetCurrentAutoBookForRun(Run run, Guid scenarioId);

        /// <summary>
        /// Returns first free AutoBook of the specified type and locks it if required
        /// </summary>
        /// <param name="autoBookType"></param>
        /// <param name="lockAutoBook"></param>
        /// <returns></returns>
        AutoBookDomainObject GetFirstAdequateIdleAutoBook(AutoBookInstanceConfiguration autoBookType, double autoBookRequiredStorageGB, bool lockAutoBooks);

        /// <summary>
        /// Returns working AutoBooks, includes might be currently busy.
        /// </summary>
        List<AutoBookDomainObject> WorkingAutoBooks { get; }

        AutoBookSettings Settings { get; }

        /// <summary>
        /// Tests provisioning
        /// </summary>
        /// <returns></returns>
        bool TestProvisioning();

        /// <summary>
        /// Number of instances
        /// </summary>
        int Instances { get; }

        /// <summary>
        /// Returns interface for managing AutoBook instance.
        /// </summary>
        /// <param name="autoBook"></param>
        /// <returns></returns>
        IAutoBook GetInterface(AutoBookDomainObject autoBook);

        /// <summary>
        /// Executes AutoBook provisioning
        /// </summary>
        void ExecuteProvisionining();

        /// <summary>
        /// Validates before the AutoBook can be deleted
        /// </summary>
        /// <param name="autoBook"></param>
        void ValidateForDelete(AutoBookDomainObject autoBook);

        /// <summary>
        /// Returns PAAutoBook from Provisioning API
        /// </summary>
        /// <param name="autoBook"></param>
        /// <returns></returns>
        PAAutoBook GetPAAutoBook(AutoBookDomainObject autoBook);

        List<AutoBookInstanceConfiguration> CreateDefaultInstanceConfigurations();

        /// <summary>
        /// Returns AutoBook instance configurations that can execute the run, ordered in ascending cost order
        /// </summary>
        /// <param name="run"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        List<AutoBookInstanceConfiguration> GetInstanceConfigurationsAscByCost(Run run, List<AutoBookInstanceConfiguration> allAutoBookInstanceConfigurations,
            int salesArea, int campaigns, int demographics, int breaks);

        /// <summary>
        /// Returns required storage for run
        /// </summary>
        /// <param name="breaksCount"></param>
        /// <returns></returns>
        double GetRequiredStorageForBreakCountInGB(int breaksCount);

        /// <summary>
        /// Wait for AutoBooks to be working
        /// </summary>
        /// <param name="autoBooks"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        List<AutoBookDomainObject> WaitForAutoBooksWorking(List<AutoBookDomainObject> autoBooks, TimeSpan timeout);

        /// <summary>
        /// Wait for provisioning AutoBooks to be working
        /// </summary>
        List<AutoBookDomainObject> WaitForProvisioned();

        string CreateAutoBookRequestRun(AutoBookRequestModel autoBookRequestModel);
    }
}
