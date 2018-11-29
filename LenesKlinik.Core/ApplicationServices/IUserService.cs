using LenesKlinik.Core.Entities;
using LenesKlinik.Core.Entities.DTO;

namespace LenesKlinik.Core.ApplicationServices
{
    public interface IUserService
    {
        User CreateUser(UserCreateInput ucInput);
    }
}