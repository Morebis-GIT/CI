using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications
{
    /// <summary>
    /// Programme classification repository
    /// </summary>
    public interface IProgrammeClassificationRepository
    {
        /// <summary>
        /// get all demos
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProgrammeClassification> GetAll();

        /// <summary>
        /// Add demo
        /// </summary>
        /// <param name="programmeClassifications"></param>
        void Add(IEnumerable<ProgrammeClassification> programmeClassifications);

        /// <summary>
        /// Get programme classification by Code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        ProgrammeClassification GetByCode(string code);

        /// <summary>
        /// Get programme classification by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ProgrammeClassification GetById(int id);

        /// <summary>
        /// delete demo by id
        /// </summary>
        /// <param name="id"></param>
        void Delete(int id);

        int CountAll { get; }

        /// <summary>
        /// Updates demo
        /// </summary>
        /// <param name="programmeClassification"></param>
        void Update(ProgrammeClassification programmeClassification);

        /// <summary>
        /// Save changes
        /// </summary>
        void SaveChanges();

        /// <summary>
        /// Deletes all demos
        /// </summary>
        void Truncate();
    }
}
