using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.EventsCalendar.RoleName)]
    public class EventsCalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddNewEvent()
        {
            return View("Index");
        }
    }
}