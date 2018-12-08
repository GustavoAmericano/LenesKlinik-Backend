using System.Collections.Generic;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices
{
    public interface IWorkService
    {
        /// <summary>
        /// Validates and saves a new work entity to the DB.
        /// </summary>
        /// <param name="work"></param>
        /// <returns>The saved work entity.</returns>
        Work CreateWork(Work work);

        /// <summary>
        /// Fetches all work entities from the DB.
        /// </summary>
        /// <returns>An IEnumerable of all Work found in the DB.</returns>
        IEnumerable<Work> GetAllWork();

        /// <summary>
        /// Removes a Work from the Db.
        /// </summary>
        /// <param name="workId"></param>
        void DeleteWork(int workId);

        /// <summary>
        /// Validates and updates a Work entity in the DB.
        /// </summary>
        /// <param name="workId"></param>
        /// <param name="work"></param>
        /// <returns>The updated entity.</returns>
        Work UpdateWork(int workId, Work work);


        /// <summary>
        /// Gets a single Work entity from DB based on it's ID.
        /// </summary>
        /// <param name="workId"></param>
        /// <returns>The first matching Work</returns>
        Work GetWorkById(int workId);
    }
}