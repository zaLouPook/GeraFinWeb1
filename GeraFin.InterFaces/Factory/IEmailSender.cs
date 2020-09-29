using System.Threading.Tasks;

namespace GeraFin.InterFaces.Factory
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendEmailConfirmationAsync(string email, object callbackUrl);
    }
}
