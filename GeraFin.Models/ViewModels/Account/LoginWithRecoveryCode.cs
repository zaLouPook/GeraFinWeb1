using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.Account
{
    public class LoginWithRecoveryCode
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Recovery Code")]
        public string RecoveryCode { get; set; }
    }
}
