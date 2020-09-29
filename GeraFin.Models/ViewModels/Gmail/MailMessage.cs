using System;
using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.Gmail
{
    public class MailMessage
    {
        [Key]
        public int EmailId { get; set; }
        public string UserId { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string EmailSubject { get; set; }
        public DateTime DateSent { get; set; }
        public string EmailBody { get; set; }
        public Attachment attachment { get; set; }
    }
}
