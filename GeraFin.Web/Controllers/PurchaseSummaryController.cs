using System.Linq;
using GeraFin.Models.Pages;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.GeraFinWeb;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.PurchaseSummary.RoleName)]

    public class PurchaseSummaryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseSummaryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            PurchaseSummary purchaseSummary = _context.PurchaseSummary.SingleOrDefault(x => x.InvoiceOrderId.Equals(id));

            //ViewBag.BranchName = (from data in _context.Branch.Where(x => x.BranchId == purchaseSummary.BranchId) select data.BranchName).FirstOrDefault();

            if (purchaseSummary == null)
            {
                return View(new PurchaseSummary());
            }
            return View(purchaseSummary);
        }
    }
}