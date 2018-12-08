using System;
using System.Collections;
using System.Collections.Generic;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.DomainServices
{
    public interface IWorkRepository
    {
        /// <summary>
        /// Saves the Work entity to the DB.
        /// </summary>
        /// <param name="work"></param>
        /// <returns>The saved entity.</returns>
        Work CreateWork(Work work);

        /// <summary>
        /// Gets all work saved in the DB.
        /// </summary>
        /// <returns>An IEnumerable of Work entities</returns>
        IEnumerable<Work> GetAllWork();

        /// <summary>
        /// Removes a Work entity from the DB.
        /// </summary>
        /// <param name="workId"></param>
        void DeleteWork(int workId);

        /// <summary>
        /// Updates the saved Work entity with the changes.
        /// </summary>
        /// <param name="work"></param>
        /// <returns>The updated entity.</returns>
        Work UpdateWork(Work work);

        /// <summary>
        /// Gets the first matching Work from DB.
        /// </summary>
        /// <param name="workId"></param>
        /// <returns>The first matching Work entity.</returns>
        Work GetWorkById(int workId);
    }
}