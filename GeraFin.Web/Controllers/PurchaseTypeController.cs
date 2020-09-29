using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.PurchaseType.RoleName)]

    public class PurchaseTypeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}