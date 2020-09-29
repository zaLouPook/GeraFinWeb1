using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.StockTransfer.RoleName)]

    public class StockTransferController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}