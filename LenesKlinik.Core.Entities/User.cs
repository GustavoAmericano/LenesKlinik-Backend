namespace LenesKlinik.Core.Entities
{
    public class User
    {
        // Login information
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        //Personal information
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public int SecretNumber { get; set; }
    }
}