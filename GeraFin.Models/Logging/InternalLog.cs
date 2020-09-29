using System;
using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.Logging
{
    public class InternalLog
    {
        [Key]
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public int ErrorId { get; set; }
        public string Page { get; set; }
        public string Service { get; set; }
        public string Exception { get; set; }
        public DateTime ExDate { get; set; }
    }
}
