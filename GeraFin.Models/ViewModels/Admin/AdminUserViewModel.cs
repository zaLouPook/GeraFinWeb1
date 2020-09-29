using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.Admin
{
    public class AdminUserViewModel
    {
        [Required(ErrorMessage = "User Name Required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "User Email Required")]
        [EmailAddress(ErrorMessage = "Email Address Invalid")]
        public string Email { get; set; }
        public string Password { get; set; }
        [Required(ErrorMessage = "User Phone Number Required")]
        public string Phone { get; set; }
        public string VerificationCode { get; set; }
        public int Verify { get; set; }
        public string AccountCreatingTime { get; set; }
        public int State { get; set; }
    }
}
