using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.Account
{
    public class ForgotPassword
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
