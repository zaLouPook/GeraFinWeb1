using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.Stock.RoleName)]

    public class StockController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}