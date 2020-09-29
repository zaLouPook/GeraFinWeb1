using System;
using System.Linq;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using GeraFin.Models.ViewModels.Dashboard;
using GeraFin.Models.ViewModels.Syncfusion;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/EventsCalendar")]
    public class EventsCalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsCalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddNewEvent(EventsCalendar Event)
        {
            if (String.IsNullOrEmpty(Event.EventClass))
                Event.EventClass = "external-event bg-light-blue ui-draggable ui-draggable-handle";

            _context.EventsCalendar.Add(Event);
            _context.SaveChanges();

            return View("Index");
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<EventsCalendar> payload)
        {
            EventsCalendar Event = payload.value;
            _context.EventsCalendar.Update(Event);
            _context.SaveChanges();
            return Ok(Event);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<EventsCalendar> payload)
        {
            EventsCalendar Event = _context.EventsCalendar.Where(x => x.BranchId == (int)payload.key).FirstOrDefault();
            _context.EventsCalendar.Remove(Event);
            _context.SaveChanges();
            return Ok(Event);
        }
    }
}