namespace LenesKlinik.Core.Entities.DTO
{
    public class UserCreateInput
    {
        // Login information
        public string Email { get; set; }
        public string clearPassword { get; set; }

        //Personal information
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public int SecretNumber { get; set; }
    }
}