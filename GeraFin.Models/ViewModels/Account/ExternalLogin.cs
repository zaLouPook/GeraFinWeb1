using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.Account
{
    public class ExternalLogin
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
