using System;
using System.Linq;
using GeraFin.DAL.Helpers;
using System.Data.SqlClient;
using System.Threading.Tasks;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using GeraFin.InterFaces.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/PurchaseOrder")]

    public class PurchaseOrderController : Controller
    {
        public string _ConString = "";

        private readonly ApplicationDbContext _context;
        private readonly INumberSequence _numberSequence;

        public PurchaseOrderController(ApplicationDbContext context, INumberSequence numberSequence, IConfiguration configuration)
        {
            _context = context;
            _numberSequence = numberSequence;
            _ConString = configuration.GetConnectionString("DevEnvR");
        }

        // GET: api/PurchaseOrder
        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrder()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId != 0)
            {
                List<PurchaseOrder> Items = await _context.PurchaseOrder.Where(x => x.BranchId == BranchId && x.Visable != 0).OrderByDescending(x => x.PurchaseOrderId).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            else
            {
                List<PurchaseOrder> Items = await _context.PurchaseOrder.Where(x => x.Visable != 0).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            PurchaseOrder result = await _context.PurchaseOrder.Where(x => x.PurchaseOrderId.Equals(id)).Include(x => x.PurchaseOrderLines).FirstOrDefaultAsync();

            return Ok(result);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetProdById(int id)
        {
            PurchaseOrder result = await _context.PurchaseOrder.Where(x => x.PurchaseOrderId.Equals(id)).Include(x => x.PurchaseOrderLines).FirstOrDefaultAsync();

            result.PurchaseOrderLines.Select(x => x.ProductId).ToList();

            return Ok(result);
        }

        private void UpdatePurchaseOrder(int purchaseOrderId)
        {
            try
            {
                PurchaseOrder purchaseOrder = new PurchaseOrder();
                purchaseOrder = _context.PurchaseOrder.Where(x => x.PurchaseOrderId.Equals(purchaseOrderId)).FirstOrDefault();

                if (purchaseOrder != null)
                {
                    List<PurchaseOrderLine> lines = new List<PurchaseOrderLine>();

                    lines = _context.PurchaseOrderLine.Where(x => x.PurchaseOrderId.Equals(purchaseOrderId)).ToList();

                    purchaseOrder.PurchaseTypeId = 1;
                    purchaseOrder.TransactionTypeId = 1;
                    purchaseOrder.Amount = lines.Sum(x => x.Amount);
                    purchaseOrder.SubTotal = lines.Sum(x => x.SubTotal);
                    purchaseOrder.Total = lines.Sum(x => x.Total);

                    _context.Update(purchaseOrder);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<PurchaseOrder> payload)
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId != 0)
            {
                //payload.value.AccountType = "30 days";
                payload.value.PurchaseTypeId = 1;
                payload.value.TransactionTypeId = 1;
                payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));
                payload.value.DeliveryDate = Convert.ToDateTime(payload.value.DeliveryDate.AddDays(1).ToString("yyyy-MM-dd"));

                payload.value.PurchaseTypeId = 1;

                PurchaseOrder purchaseOrder = payload.value;
                purchaseOrder.PurchaseOrderName = _numberSequence.GetNumberSequence("PO");

                _context.PurchaseOrder.Add(purchaseOrder);
                _context.SaveChanges();

                this.UpdatePurchaseOrder(purchaseOrder.PurchaseOrderId);
                return Ok(purchaseOrder);
            }
            else
            {
                //payload.value.AccountType = "30 days";

                payload.value.PurchaseTypeId = 1;
                payload.value.TransactionTypeId = 1;
                payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));
                payload.value.DeliveryDate = Convert.ToDateTime(payload.value.DeliveryDate.AddDays(1).ToString("yyyy-MM-dd"));

                PurchaseOrder purchaseOrder = payload.value;
                purchaseOrder.PurchaseOrderName = _numberSequence.GetNumberSequence("PO");

                _context.PurchaseOrder.Add(purchaseOrder);
                _context.SaveChanges();

                this.UpdatePurchaseOrder(purchaseOrder.PurchaseOrderId);
                return Ok(purchaseOrder);
            }
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<PurchaseOrder> payload)
        {
            payload.value.TransactionTypeId = 1;
            payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));
            payload.value.DeliveryDate = Convert.ToDateTime(payload.value.DeliveryDate.AddDays(1).ToString("yyyy-MM-dd"));

            PurchaseOrder purchaseOrder = payload.value;
            _context.PurchaseOrder.Update(purchaseOrder);

            if (purchaseOrder.Recieved == true)
            {
                AllocatePurchaseOrder(purchaseOrder.PurchaseOrderId);
            }

            _context.SaveChanges();
            this.UpdatePurchaseOrder(purchaseOrder.PurchaseOrderId);
            return Ok(purchaseOrder);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<PurchaseOrder> payload)
        {
            PurchaseOrder purchaseOrder = _context.PurchaseOrder.Where(x => x.PurchaseOrderId == (int)payload.key).FirstOrDefault();
            _context.PurchaseOrder.Remove(purchaseOrder);
            _context.SaveChanges();
            this.UpdatePurchaseOrder(purchaseOrder.PurchaseOrderId);
            return Ok(purchaseOrder);
        }

        private void AllocatePurchaseOrder(int purchaseOrderId)
        {
            SqlHelper helper = new SqlHelper(_ConString);
            SqlParameter[] Parameters = new SqlParameter[1];
            Parameters.SetValue(helper.CreateParameter("@PurchaseOrderId", purchaseOrderId, 0), 0);

            helper.Update("GeraFin.stp_AllocatePurchaseOrder", System.Data.CommandType.StoredProcedure, Parameters);
        }
    }
}