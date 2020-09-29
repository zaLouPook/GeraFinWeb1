using System.ComponentModel.DataAnnotations;

namespace GeraFin.Models.ViewModels.Dashboard
{
    public class EventsCalendar
    {
        [Key]
        public int EventId { get; set; }
        public int UserId { get; set; }
        public int BranchId { get; set; }
        public string EventName { get; set; }
        public string EventClass { get; set; }
        public bool MarkCompleted { get; set; }
    }
}
