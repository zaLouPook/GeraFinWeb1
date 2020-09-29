using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.PurchaseSummary.RoleName)]

    public class PurchaseSummaryLineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}