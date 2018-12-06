using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices
{
    public interface IUserService
    {
        User CreateUser(User user,string clearPass);
    }
}