using System;
using System.Linq;
using GeraFin.InterFaces.Factory;
using GeraFin.DAL.DataAccess;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/PurchaseSummary")]
    public class PurchaseSummaryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INumberSequence _numberSequence;

        public PurchaseSummaryController(ApplicationDbContext context, INumberSequence numberSequence)
        {
            _context = context;
            _numberSequence = numberSequence;
        }

        // GET: api/PurchaseSummary
        [HttpGet]
        public async Task<IActionResult> GetPurchaseSummary()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId != 0)
            {
                List<PurchaseSummary> Items = await _context.PurchaseSummary.Where(x => x.BranchId == BranchId).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            else
            {
                List<PurchaseSummary> Items = await _context.PurchaseSummary.ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            PurchaseSummary result = await _context.PurchaseSummary.Where(x => x.InvoiceOrderId.Equals(id)).Include(x => x.InvoiceOrderLines).FirstOrDefaultAsync();

            return Ok(result);
        }

        private void UpdatePurchaseSummary(int invoiceOrderId)
        {
            try
            {
                PurchaseSummary purchaseSummary = new PurchaseSummary();
                purchaseSummary = _context.PurchaseSummary.Where(x => x.InvoiceOrderId.Equals(invoiceOrderId)).FirstOrDefault();

                if (purchaseSummary != null)
                {
                    List<PurchaseSummary> lines = new List<PurchaseSummary>();
                    lines = _context.PurchaseSummary.Where(x => x.InvoiceOrderId.Equals(invoiceOrderId)).ToList();

                    purchaseSummary.Amount = lines.Sum(x => x.Amount);
                    purchaseSummary.SubTotal = lines.Sum(x => x.SubTotal);
                    purchaseSummary.Total = lines.Sum(x => x.Total);

                    _context.Update(purchaseSummary);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<PurchaseSummary> payload)
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId != 0)
            {
                payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));
                payload.value.DeliveryDate = Convert.ToDateTime(payload.value.DeliveryDate.AddDays(1).ToString("yyyy-MM-dd"));

                payload.value.CustomerRefNumber = (from Branch in _context.Branch.Where(x => x.BranchId == BranchId) select Branch.BranchName).FirstOrDefault();

                PurchaseSummary purchaseSummary = payload.value;
                purchaseSummary.InvoiceOrderName = _numberSequence.GetNumberSequence("IN");

                _context.PurchaseSummary.Add(purchaseSummary);
                _context.SaveChanges();

                this.UpdatePurchaseSummary(purchaseSummary.InvoiceOrderId);

                return Ok(purchaseSummary);
            }
            else
            {
                payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));
                payload.value.DeliveryDate = Convert.ToDateTime(payload.value.DeliveryDate.AddDays(1).ToString("yyyy-MM-dd"));

                payload.value.CustomerRefNumber = (from Branch in _context.Branch.Where(x => x.BranchId == BranchId) select Branch.BranchName).FirstOrDefault();

                PurchaseSummary purchaseSummary = payload.value;
                purchaseSummary.InvoiceOrderName = _numberSequence.GetNumberSequence("IN");

                _context.PurchaseSummary.Add(purchaseSummary);
                _context.SaveChanges();

                this.UpdatePurchaseSummary(purchaseSummary.InvoiceOrderId);
                return Ok(purchaseSummary);
            }
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<PurchaseSummary> payload)
        {
            payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));
            payload.value.DeliveryDate = Convert.ToDateTime(payload.value.DeliveryDate.AddDays(1).ToString("yyyy-MM-dd"));

            PurchaseSummary purchaseSummary = payload.value;

            _context.PurchaseSummary.Update(purchaseSummary);
            _context.SaveChanges();

            return Ok(purchaseSummary);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<PurchaseSummary> payload)
        {
            PurchaseSummary purchaseSummary = _context.PurchaseSummary.Where(x => x.InvoiceOrderId == (int)payload.key).FirstOrDefault();

            _context.PurchaseSummary.Remove(purchaseSummary);
            _context.SaveChanges();

            return Ok(purchaseSummary);
        }
    }
}