using GeraFin.Models.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.TransactionType.RoleName)]

    public class TransactionTypeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}