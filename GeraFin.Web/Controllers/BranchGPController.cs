using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.BranchGP.RoleName)]
    public class BranchGPController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}