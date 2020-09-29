using System.Linq;
using GeraFin.Models.Pages;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.GeraFinWeb;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.DailyIssues.RoleName)]
    public class DailyIssuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyIssuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            DailyIssues dailyIssues = _context.DailyIssues.SingleOrDefault(x => x.DailyIssuesId.Equals(id));

            if (dailyIssues == null)
            {
                return View(new DailyIssues());
            }
            return View(dailyIssues);
        }
    }
}