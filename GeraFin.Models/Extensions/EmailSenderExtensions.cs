using System.Threading.Tasks;
using System.Text.Encodings.Web;
using GeraFin.InterFaces.Factory;

namespace GeraFin.Models.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(
                email, 
                "Please Confirm Your Email",
                $"By Following This Link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
