using System.Collections.Generic;
using xggameplan.Model;

namespace xggameplan.Repository
{
    /// <summary>
    /// System message repository
    /// </summary>
    public interface ISystemMessageRepository
    {
        /// <summary>
        /// Returns single system message by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SystemMessage Get(int id);

        /// <summary>
        /// Returns all system messages
        /// </summary>
        /// <returns></returns>
        List<SystemMessage> GetAll();

        /// <summary>
        /// Returns all system messages for the specified group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        List<SystemMessage> GetByGroup(SystemMessageGroups group);

    }
}
