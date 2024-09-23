namespace CCI.Service.Contractors
{
    public interface IEmailService
    {
        void SendEmail(string toEmail, string subject, string body, bool isHtml = false);
    }
}
