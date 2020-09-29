using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.UnitOfMeasure.RoleName)]

    public class UnitOfMeasureController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}