using GeraFin.Models.Pages;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.GeraFinWeb;

using System.Linq;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.StockTake.RoleName)]

    public class StockTakeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockTakeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {

            StockTake stockTake = _context.StockTake.SingleOrDefault(x => x.StockTakeId.Equals(id));

            if (stockTake == null)
            {
                return View(new StockTake());
            }
            return View(stockTake);
        }
    }
}