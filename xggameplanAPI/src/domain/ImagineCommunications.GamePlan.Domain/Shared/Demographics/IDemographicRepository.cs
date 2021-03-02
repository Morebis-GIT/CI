using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.Demographics
{
    public interface IDemographicRepository
    {
        /// <summary>
        /// Get all demographics.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Demographic> GetAll();

        /// <summary>
        /// Get all demographics set true for gameplan
        /// </summary>
        /// <returns></returns>
        List<string> GetAllGameplanDemographics();

        /// <summary>
        /// Add demo
        /// </summary>
        /// <param name="channels"></param>
        void Add(IEnumerable<Demographic> channels);

        /// <summary>
        /// Get demographic by ExternalRef.
        /// </summary>
        /// <param name="externalRef"></param>
        /// <returns></returns>
        Demographic GetByExternalRef(string externalRef);

        /// <summary>
        /// Get demographics list by list of ExternalRef
        /// </summary>
        /// <param name="externalRef"></param>
        /// <returns></returns>
        IEnumerable<Demographic> GetByExternalRef(List<string> externalRefs);

        /// <summary>
        /// Get demographic by code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Demographic GetById(int id);

        /// <summary>
        /// Delete demographic by id
        /// </summary>
        /// <param name="id"></param>
        void Delete(int id);

        int CountAll { get; }

        /// <summary>
        /// Updates demographic
        /// </summary>
        /// <param name="demographic"></param>
        void Update(Demographic demographic);

        /// <summary>
        /// Update existing demographics
        /// </summary>
        /// <param name="demographics"></param>
        void UpdateRange(IEnumerable<Demographic> demographics);

        /// <summary>
        /// Save changes
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Deletes all demographics
        /// </summary>
        void Truncate();

        /// <summary>
        /// Delete demographics by externalRefs
        /// </summary>
        /// <param name="externalRefs"></param>
        void DeleteRangeByExternalRefs(IEnumerable<string> externalRefs);

        /// <summary>
        /// Insert or replace demographics
        /// </summary>
        /// <param name="items"></param>
        void InsertOrUpdate(IEnumerable<Demographic> items);
    }
}
