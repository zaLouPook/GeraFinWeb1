using System;
using System.Linq;
using GeraFin.DAL.DataAccess;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/DailyIssuesLine")]

    public class DailyIssuesLineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyIssuesLineController(ApplicationDbContext context)
        {
            _context = context;
        }

        public static int issuesUpdateOrderID;

        // GET: api/DailyIssuesLine
        [HttpGet]
        public async Task<IActionResult> GetDailyIssuesLineLine()
        {
            var headers = Request.Headers["DailyIssuesId"];
            int DailyIssuesId = Convert.ToInt32(headers);

            issuesUpdateOrderID = DailyIssuesId;

            List<DailyIssuesLine> Items = await _context.DailyIssuesLine.Include(x => x.DailyIssues).Where(x => x.DailyIssuesId.Equals(DailyIssuesId)).ToListAsync();

            int Count = Items.Count();
            return Ok(new { Items, Count });
        }

        private DailyIssuesLine Recalculate(DailyIssuesLine dailyIssuesLine)
        {
            try
            {
                dailyIssuesLine.Amount = Convert.ToDecimal(dailyIssuesLine.Quantity) * dailyIssuesLine.Price;
                dailyIssuesLine.SubTotal = dailyIssuesLine.Amount;
                dailyIssuesLine.Total = dailyIssuesLine.SubTotal;

                _context.Update(dailyIssuesLine);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return dailyIssuesLine;
        }

        private void UpdateDailyIssues(int dailyIssueId)
        {
            try
            {
                DailyIssues dailyIssues = new DailyIssues();
                dailyIssues = _context.DailyIssues.Where(x => x.DailyIssuesId.Equals(dailyIssueId)).FirstOrDefault();

                if (dailyIssues != null)
                {
                    List<DailyIssuesLine> lines = new List<DailyIssuesLine>();
                    lines = _context.DailyIssuesLine.Where(x => x.DailyIssuesId.Equals(dailyIssueId)).ToList();

                    dailyIssues.Amount = lines.Sum(x => x.Amount);
                    dailyIssues.SubTotal = lines.Sum(x => x.SubTotal);
                    dailyIssues.Total = lines.Sum(x => x.Total);

                    _context.Update(dailyIssues);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<DailyIssuesLine> payload)
        {
            payload.value.Price = (from products in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select products.DefaultSellingPrice).FirstOrDefault();
            payload.value.Description = (from products in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select products.ProductCode).FirstOrDefault();
            payload.value.DailyIssuesId = issuesUpdateOrderID;

            if (payload.value.Price == 0)
            {
                payload.value.Price = (from products in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select products.SpecialPrice).FirstOrDefault();
            }

            DailyIssuesLine dailyIssuesLine = payload.value;

            _context.DailyIssuesLine.Add(dailyIssuesLine);
            _context.SaveChanges();

            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            Stock stock = _context.Stock.Where(x => x.ProductId == payload.value.ProductId && x.BranchId == BranchId).FirstOrDefault();

            this.Recalculate(dailyIssuesLine);
            this.UpdateDailyIssues(dailyIssuesLine.DailyIssuesId);

            return Ok(dailyIssuesLine);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<DailyIssuesLine> payload)
        {
            DailyIssuesLine dailyIssuesLine = payload.value;
            dailyIssuesLine = this.Recalculate(dailyIssuesLine);

            _context.DailyIssuesLine.Update(dailyIssuesLine);
            _context.SaveChanges();

            this.UpdateDailyIssues(dailyIssuesLine.DailyIssuesId);
            return Ok(dailyIssuesLine);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<DailyIssuesLine> payload)
        {
            DailyIssuesLine dailyIssuesLine = _context.DailyIssuesLine.Where(x => x.DailyIssuesLineId == (int)payload.key).FirstOrDefault();

            _context.DailyIssuesLine.Remove(dailyIssuesLine);
            _context.SaveChanges();

            this.UpdateDailyIssues(dailyIssuesLine.DailyIssuesId);
            return Ok(dailyIssuesLine);
        }

        public void UpdateStock(int ProductId, decimal OrderQTY, int BranchId)
        {
            Stock stock = _context.Stock.Where(x => x.ProductId == ProductId && x.BranchId == BranchId).FirstOrDefault();

            if (!stock.Quantity.Equals(null))
            {
                if (stock.Quantity == 0)
                {
                    return;
                }
                
                else if (stock.Quantity == OrderQTY)
                {
                    stock.Quantity = 0;
                    _context.SaveChanges();
                }
                
                else if (stock.Quantity > OrderQTY)
                {
                    stock.Quantity -= OrderQTY;

                    _context.Update(stock);
                    _context.SaveChanges();
                }
            }
        }
    }
}