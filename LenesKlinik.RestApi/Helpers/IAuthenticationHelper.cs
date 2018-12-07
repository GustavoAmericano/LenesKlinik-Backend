using LenesKlinik.Core.Entities;

namespace LenesKlinik.RestApi.Helpers
{
    public interface IAuthenticationHelper
    {
        string GenerateToken(User user);
    }
}
