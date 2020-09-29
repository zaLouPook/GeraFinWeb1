using System;
using System.Linq;
using System.Threading.Tasks;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/DailyIncomeLine")]
    public class DailyIncomeLineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DailyIncomeLineController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DailyIncomeLine
        [HttpGet]
        public async Task<IActionResult> GetDailyIncomeLine()
        {
            var headers = Request.Headers["SalesOrderId"];
            int salesOrderId = Convert.ToInt32(headers);

            List<DailyIncomeLine> Items = await _context.DailyIncomeLine.Where(x => x.SalesOrderId.Equals(salesOrderId)).ToListAsync();

            int Count = Items.Count();
            return Ok(new { Items, Count });
        }

        private DailyIncomeLine Recalculate(DailyIncomeLine dailyIncomeLine)
        {
            try
            {
                dailyIncomeLine.Amount = Convert.ToDecimal(dailyIncomeLine.Quantity) * dailyIncomeLine.Price;
                dailyIncomeLine.SubTotal = dailyIncomeLine.Amount;
                dailyIncomeLine.Total = dailyIncomeLine.SubTotal;
            }
            catch (Exception)
            {
                throw;
            }
            return dailyIncomeLine;
        }

        private void UpdateSalesOrder(int salesOrderId)
        {
            try
            {
                DailyIncome dailyIncome = new DailyIncome();
                dailyIncome = _context.DailyIncome.Where(x => x.SalesOrderId.Equals(salesOrderId)).FirstOrDefault();

                if (dailyIncome != null)
                {
                    List<DailyIncomeLine> lines = new List<DailyIncomeLine>();
                    lines = _context.DailyIncomeLine.Where(x => x.SalesOrderId.Equals(salesOrderId)).ToList();

                    dailyIncome.Amount = lines.Sum(x => x.Amount);
                    dailyIncome.SubTotal = lines.Sum(x => x.SubTotal);
                    dailyIncome.Total = lines.Sum(x => x.Total);

                    _context.Update(dailyIncome);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<DailyIncomeLine> payload)
        {
            payload.value.Price = (from products in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select products.DefaultSellingPrice).FirstOrDefault();
            payload.value.Description = (from products in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select products.ProductCode).FirstOrDefault();

            DailyIncomeLine dailyIncomeLine = payload.value;
            dailyIncomeLine = this.Recalculate(dailyIncomeLine);

            _context.DailyIncomeLine.Add(dailyIncomeLine);
            _context.SaveChanges();

            this.UpdateSalesOrder(dailyIncomeLine.SalesOrderId);
            return Ok(dailyIncomeLine);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<DailyIncomeLine> payload)
        {
            DailyIncomeLine dailyIncomeLine = payload.value;
            dailyIncomeLine = this.Recalculate(dailyIncomeLine);

            _context.DailyIncomeLine.Update(dailyIncomeLine);
            _context.SaveChanges();

            this.UpdateSalesOrder(dailyIncomeLine.SalesOrderId);
            return Ok(dailyIncomeLine);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<DailyIncomeLine> payload)
        {
            DailyIncomeLine dailyIncomeLine = _context.DailyIncomeLine.Where(x => x.SalesOrderLineId == (int)payload.key).FirstOrDefault();

            _context.DailyIncomeLine.Remove(dailyIncomeLine);
            _context.SaveChanges();

            this.UpdateSalesOrder(dailyIncomeLine.SalesOrderId);
            return Ok(dailyIncomeLine);
        }
    }
}