using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.Weeklysignoff.RoleName)]

    public class WeeklySignOffLineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}