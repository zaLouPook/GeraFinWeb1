using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeraFin.Models.ViewModels.Admin
{
    public class AdminChangePasswordViewModel
    {
        [Required(ErrorMessage = "Old Password Required")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "New Password Required")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Re Enter New Password Required")]
        public string ReEnterPassword { get; set; }
    }
}
