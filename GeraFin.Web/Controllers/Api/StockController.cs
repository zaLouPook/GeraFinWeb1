using GeraFin.DAL.DataAccess;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Stock")]
    public class StockController : Controller
    {
        private readonly ApplicationDbContext _context;
        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        public static int StockQtyOnHand;

        // GET: api/Stock
        [HttpGet]
        public async Task<IActionResult> GetProduct()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId")); ;

            if (BranchId == 0)
            {
                List<Stock> Items = await _context.Stock
                    .Where(x => x.IsAlocated == 1 && x.Quantity != 0).ToListAsync();

                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            else
            {
                List<Stock> Items = await _context.Stock
                    .Where(x => x.BranchId == BranchId && x.IsAlocated == 1 && x.Quantity != 0).ToListAsync();

                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<Stock> payload)
        {
            payload.value.IsAlocated = 1;

            Stock stock = payload.value;
            _context.Stock.Add(stock);
            _context.SaveChanges();
            return Ok(stock);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<Stock> payload)
        {
            payload.value.IsAlocated = 1;

            Stock stock = payload.value;
            _context.Stock.Update(stock);
            _context.SaveChanges();
            return Ok(stock);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<Stock> payload)
        {
            Stock stock = _context.Stock.Where(x => x.StockId == (int)payload.key).FirstOrDefault();
            _context.Stock.Remove(stock);
            _context.SaveChanges();
            return Ok(stock);

        }
    }
}