using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.Specials.RoleName)]

    public class SpecialsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}