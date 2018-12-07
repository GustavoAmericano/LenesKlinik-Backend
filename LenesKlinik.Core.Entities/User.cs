using System.Collections.Generic;

namespace LenesKlinik.Core.Entities
{
    public class User
    {
        // Login information
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public bool IsAdmin { get; set; }
        public Customer Customer { get; set; }
    }
}