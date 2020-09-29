using Microsoft.AspNetCore.Mvc;

namespace GeraFin.Controllers
{
    public class NumberSequenceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}