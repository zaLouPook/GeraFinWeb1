using System;
using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.Logging
{
    public class ResponseLog
    {
        [Key]
        public int ResponseId { get; set; }
        public int RequestId { get; set; }
        public string Respone { get; set; }
        public string ResponetMethod { get; set; }
        public string ResponeErrorId { get; set; }
        public string ResponeErrorMessage { get; set; }
        public DateTime Date { get; set; }
    }
}
