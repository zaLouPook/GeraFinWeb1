using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.GeraFinUser.RoleName)]
    public class GeraFinUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}