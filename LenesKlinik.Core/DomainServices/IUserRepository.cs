using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.DomainServices
{
    public interface IUserRepository
    {
        User CreateUser(User user);
        User ValidateUser(string email);
    }
}