using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.DomainServices
{
    public interface IWorkRepository
    {
        Work CreateWork(Work work);
    }
}