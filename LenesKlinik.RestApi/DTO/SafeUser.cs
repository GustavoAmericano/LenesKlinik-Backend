using LenesKlinik.Core.Entities;

namespace LenesKlinik.RestApi.DTO
{
    public class SafeUser
    {
        // Login information
        public int Id { get; set; }
        public string Email { get; set; }
        public Customer Customer { get; set; }
    }
}