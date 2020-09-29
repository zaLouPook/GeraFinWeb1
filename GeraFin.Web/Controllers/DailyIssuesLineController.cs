using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.DailyIssues.RoleName)]
    public class DailyIssuesLineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}