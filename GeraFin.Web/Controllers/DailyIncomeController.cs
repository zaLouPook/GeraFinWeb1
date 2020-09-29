using System.Linq;
using GeraFin.Models.Pages;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.GeraFinWeb;
using System;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.DailyIncome.RoleName)]

    public class DailyIncomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyIncomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            DailyIncome dailyIncome = _context.DailyIncome.SingleOrDefault(x => x.SalesOrderId.Equals(id));

            if (dailyIncome == null)
            {
                return View(new DailyIncome());
            }
            return View(dailyIncome);
        }
    }
}