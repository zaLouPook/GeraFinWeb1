using System;
using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.Logging
{
    public class RequestLog
    {
        [Key]
        public int RequestId { get; set; }
        public int ResponseId { get; set; }
        public string Request { get; set; }
        public string RequestMethod { get; set; }
        public string InternalErrorId { get; set; }
        public string InternalErrorMessage { get; set; }
        public DateTime Date { get; set; }
    }
}
