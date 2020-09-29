using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.DailyIncome.RoleName)]

    public class DailyIncomeLineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}