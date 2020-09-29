using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeraFin.Models.ViewModels.Admin
{
    public class AdminEmployeeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public string ProfilePicture { get; set; }
        public string Password { get; set; }
        public string ActionBy { get; set; }
        public string ActionDone { get; set; }
        public string ActionTime { get; set; }
        public int State { get; set; }
    }
}
