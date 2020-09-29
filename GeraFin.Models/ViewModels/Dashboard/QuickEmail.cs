using System;
using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.Dashboard
{
    public class QuickEmail
    {
        [Key]
        public int QuickEmailId { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public DateTime DateSent { get; set; }
        public string From { get; set; }
        public string EmailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Sent { get; set; }
    }
}
