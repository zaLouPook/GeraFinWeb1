using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.PurchaseOrder.RoleName)]

    public class PurchaseOrderLineController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}