using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.MyBranches.RoleName)]
    public class MyBranchesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}