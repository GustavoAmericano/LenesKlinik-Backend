using LenesKlinik.Core.Entities;

namespace LenesKlinik.Core.ApplicationServices
{
    public interface IUserService
    {
        User CreateUser(User user,string clearPass);
        User ValidateUser(string email, string password);
    }
}