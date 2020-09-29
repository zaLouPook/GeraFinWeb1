using System.Linq;
using GeraFin.Models.Pages;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.GeraFinWeb;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.Weeklysignoff.RoleName)]

    public class WeeklySignOffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeeklySignOffController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            WeeklySignOff weeklySignOff = _context.WeeklySignOff.SingleOrDefault(x => x.WeeklySignOffId.Equals(id));

            if (weeklySignOff == null)
            {
                return View(new WeeklySignOff());
            }
            return View(weeklySignOff);
        }
    }
}