using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.Vendor.RoleName)]

    public class VendorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}