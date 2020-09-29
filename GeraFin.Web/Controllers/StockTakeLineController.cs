using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.StockTake.RoleName)]

    public class StockTakeLineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}