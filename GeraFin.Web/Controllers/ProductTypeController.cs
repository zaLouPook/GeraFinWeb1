using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.ProductType.RoleName)]

    public class ProductTypeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}