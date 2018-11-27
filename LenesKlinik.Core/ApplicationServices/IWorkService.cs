using System.Collections.Generic;
using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices
{
    public interface IWorkService
    {
        Work CreateWork(Work work);
        IEnumerable<Work> GetAllWork();
    }
}