using System.Linq;
using GeraFin.DAL.DataAccess;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GeraFin.InterFaces.Factory;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using System;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/CreditedLines/[action]")]

    public class CreditedLinesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INumberSequence _numberSequence;
        public CreditedLinesController(ApplicationDbContext context, INumberSequence numberSequence)
        {
            _context = context;
            _numberSequence = numberSequence;
        }

        // GET: api/CreditNotes
        [HttpGet]
        public async Task<IActionResult> GetCreditedLines()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId == 0)
            {
                List<CreditedLines> Items = await _context.CreditedLines.ToListAsync();
                int Count = Items.Count();

                return Ok(new { Items, Count });
            }
            else
            {
                List<CreditedLines> Items = await _context.CreditedLines.Where(x => x.BranchId == BranchId).ToListAsync();
                int Count = Items.Count();
                
                return Ok(new { Items, Count });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProdByPOID(int id)
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            List<PurchaseOrderLine> GetPurchaseOrder = await _context.PurchaseOrderLine.Where(x => x.PurchaseOrderId == id).ToListAsync();
            var ProductIds = GetPurchaseOrder.Select(x => x.ProductId).ToList();

            List<Product> Items = await _context.Product.Where(e => ProductIds.Contains(e.ProductId)).ToListAsync();
            
            return Json(Items.Select(x => new
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName
            }).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> GetPriceByProdId(int id)
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            List<PurchaseOrderLine> Items = await _context.PurchaseOrderLine.Where(x => x.ProductId == id).ToListAsync();
            
            return Json(Items.Select(x => new
            {
                Price = x.Price,
                Quantity = x.Quantity
            }).ToList());
        }

        [HttpGet]
        public async Task<IActionResult> GetPOByTransType()
        {
            CreditedLinesController creditedLinesController = this;
            int BranchId = Convert.ToInt32((object)creditedLinesController.HttpContext.Session.GetInt32("_BranchId"));
            
            List<PurchaseOrder> items = await _context.PurchaseOrder.Where(x => x.BranchId == BranchId && x.TransactionTypeId == 1 && x.Recieved == true && x.Remarks != null).ToListAsync();
            
            return Json(items.Select(x => new
            {
                PurchaseOrderId = x.PurchaseOrderId,
                Remarks = x.Remarks
            }).ToList());
        }

        [HttpPost]
        public IActionResult Insert([FromBody]Crud<CreditedLines> payload)
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            payload.value.Price = (from data in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select data.DefaultBuyingPrice).FirstOrDefault();
            payload.value.TransactionTypeId = 3;
            int VendorId = (from data in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select data.VendorId).FirstOrDefault();
            payload.value.Amount = (payload.value.Price * payload.value.Quantity) * -1;
            payload.value.BranchId = BranchId;
            int UOM = (from data in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select data.UnitOfMeasureId).FirstOrDefault();
            string Messure = (from data in _context.UnitOfMeasure.Where(x => x.UnitOfMeasureId == UOM) select data.UnitOfMeasureName).FirstOrDefault();

            CreditedLines creditNote = payload.value;
            creditNote.Description = _numberSequence.GetNumberSequence("CN");

            int PurchaseOrderId = InsertPurchaseOrder(creditNote.Description, BranchId, VendorId, DateTime.Now, DateTime.Now, creditNote.Amount, creditNote.SubTotal, creditNote.Total);
            InsertPurchaseOrderLine(PurchaseOrderId, Messure, creditNote.ProductId, creditNote.Quantity, payload.value.Price, payload.value.Amount, payload.value.Amount, payload.value.Amount);
            
            _context.CreditedLines.Add(creditNote);
            _context.SaveChanges();

            UpdateStockOnHand(creditNote.ProductId, creditNote.PurchaseOrderId, creditNote.Quantity, creditNote.BranchId);
            
            UpdatePurchaseOrder(PurchaseOrderId);

            return Ok(creditNote);
        }

        public void UpdateStockOnHand(int ProductId, int PurchaseOrderId, decimal CreditedQuantity, int BranchId)
        {
            Stock stock = _context.Stock.Where(x => x.BranchId == BranchId && x.ProductId == ProductId).FirstOrDefault();

            decimal val = stock.Quantity - CreditedQuantity;

            stock.Quantity = val;

            _context.Stock.Update(stock);
            _context.SaveChanges();

        }

        public int InsertPurchaseOrder(string PurchaseOrderName, int BranchId, int VendorId, DateTime OrderDate, DateTime DelDate, decimal Amount, decimal SubTotal, decimal Total)
        {
            PurchaseOrder purchaseOrder = new PurchaseOrder();
            purchaseOrder.PurchaseOrderName = PurchaseOrderName;
            purchaseOrder.BranchId = BranchId;
            purchaseOrder.VendorId = VendorId;
            purchaseOrder.OrderDate = OrderDate;
            purchaseOrder.DeliveryDate = DelDate;
            purchaseOrder.PurchaseTypeId = 2;
            purchaseOrder.AccountType = "Credit Note Issued";
            purchaseOrder.Remarks = "Credit Note Issued";
            purchaseOrder.Amount = Amount;
            purchaseOrder.SubTotal = SubTotal;
            purchaseOrder.Total = Total;
            purchaseOrder.SupplierInvTotal = 0;
            purchaseOrder.SupplierVatAmount = 0;
            purchaseOrder.Recieved = true;
            purchaseOrder.Visable = 0;
            purchaseOrder.TransactionTypeId = 3;

            _context.PurchaseOrder.Add(purchaseOrder); 
            _context.SaveChanges();

            return purchaseOrder.PurchaseOrderId;
        }

        public void InsertPurchaseOrderLine(int PurchaseOrderId, string UOM, int ProductId, decimal Quantity, decimal Price, decimal Amount, decimal SubTotal, decimal Total)
        {
            PurchaseOrderLine PurchaseOrderCreditedLine = new PurchaseOrderLine();
            PurchaseOrderCreditedLine.PurchaseOrderId = PurchaseOrderId;
            PurchaseOrderCreditedLine.UOM = UOM;
            PurchaseOrderCreditedLine.ProductId = ProductId;
            PurchaseOrderCreditedLine.Description = "Credit Note Passed";
            PurchaseOrderCreditedLine.QuickNote = "Credit Note Passed";
            PurchaseOrderCreditedLine.Quantity = Quantity * -1;
            PurchaseOrderCreditedLine.Price = Price;
            PurchaseOrderCreditedLine.SpecialPrice = 0;
            PurchaseOrderCreditedLine.Amount = Amount;
            PurchaseOrderCreditedLine.SubTotal = SubTotal;
            PurchaseOrderCreditedLine.Total = Total;
            PurchaseOrderCreditedLine.CreditLine = true;
            PurchaseOrderCreditedLine.Credited = 1;

            _context.PurchaseOrderLine.Add(PurchaseOrderCreditedLine);
            _context.SaveChanges();
        }

        private void UpdatePurchaseOrder(int purchaseOrderId)
        {
            PurchaseOrder purchaseOrder = new PurchaseOrder();
            purchaseOrder = _context.PurchaseOrder.Where(x => x.PurchaseOrderId.Equals(purchaseOrderId)).FirstOrDefault();

            if (purchaseOrder != null)
            {
                List<PurchaseOrderLine> lines = new List<PurchaseOrderLine>();
                lines = _context.PurchaseOrderLine.Where(x => x.PurchaseOrderId.Equals(purchaseOrderId)).ToList();

                //update master data by its lines
                purchaseOrder.Amount = lines.Sum(x => x.Amount);
                purchaseOrder.SubTotal = lines.Sum(x => x.SubTotal);
                purchaseOrder.Total = lines.Sum(x => x.Total);

                _context.Update(purchaseOrder);
                _context.SaveChanges();
            }
        }

        [HttpPost]
        public IActionResult Update([FromBody]Crud<CreditedLines> payload)
        {
            payload.value.TransactionTypeId = 3;
            CreditedLines creditNote = payload.value;

            _context.CreditedLines.Update(creditNote);
            _context.SaveChanges();

            return Ok(creditNote);
        }

        [HttpPost]
        public IActionResult Remove([FromBody]Crud<CreditedLines> payload)
        {
            CreditedLines creditNote = _context.CreditedLines.Where(x => x.LineId == (int)payload.key).FirstOrDefault();

            _context.CreditedLines.Remove(creditNote);
            _context.SaveChanges();
            
            return Ok(creditNote);
        }
    }
}