using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeraFin.Models.ViewModels.Admin
{
    public class AdminVerifyAccountViewModel
    {
        public string Email { get; set; }
        public string VerificationCode { get; set; }
    }
}
