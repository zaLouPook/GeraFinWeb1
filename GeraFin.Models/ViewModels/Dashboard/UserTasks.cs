using System;
using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.Dashboard
{
    public class UserTasks
    {
        [Key]
        public int TaskId { get; set; }
        public int UserId { get; set; }
        public string TaskName { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public bool IsOverDue { get; set; }
        public bool MarkCompleted { get; set; }
    }
}
