using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.Product.RoleName)]
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}