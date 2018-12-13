namespace LenesKlinik.Core.ApplicationServices
{
    public interface IEmailService
    {
        void SendMail(string emailTo, string title, string body);
    }
}