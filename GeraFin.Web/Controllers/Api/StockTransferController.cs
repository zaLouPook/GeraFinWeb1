using System.Linq;
using System.Threading.Tasks;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using GeraFin.InterFaces.Factory;
using System;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/StockTransfer/[action]")]
    public class StockTransferController : Controller
    {
        public string _ConString = "";
        private readonly ApplicationDbContext _context;
        private readonly INumberSequence _numberSequence;
        public static int StockQtyOnHand;


        public StockTransferController(ApplicationDbContext context, INumberSequence numberSequence)
        {
            _context = context;
            _numberSequence = numberSequence;
        }

        // GET: api/StockTransfer
        [HttpGet]
        public async Task<IActionResult> GetProduct()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId == 0)
            {
                List<Stock> Items = await _context.Stock
                    .Where(x => x.Quantity != 0 && x.IsAlocated == 1).ToListAsync();

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

        [HttpGet]
        public async Task <IActionResult> GetBranch()
        {
            List<Branch> Items = await _context.Branch.Where(x => x.LoginRequired == true).ToListAsync();
            int Count = Items.Count();
            return Ok(new { Items, Count });
        }

        [HttpPost]
        public IActionResult Update([FromBody]Crud<Stock> payload)
        {
            int TransferToBranchId = payload.value.BranchId;
            
            payload.value.IsAlocated = 1;
            payload.value.PurchaseOrderId = (from exStock in _context.Stock.Where(x => x.StockId == payload.value.StockId) select exStock.PurchaseOrderId).FirstOrDefault();

            decimal TotalItemQTYRem = (from QTY in _context.Stock.Where(x => x.StockId == payload.value.StockId && x.IsAlocated == 1) select QTY.Quantity).FirstOrDefault() - payload.value.Quantity;
            decimal QTYTransfered = payload.value.Quantity;

            ValidateTransfer(payload.value.BranchId, payload.value.ProductId, QTYTransfered, payload, Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId")));
            
            payload.value.BranchId = (from BranchData in _context.Stock.Where(x => x.StockId == payload.value.StockId && x.IsAlocated == 1) select BranchData.BranchId).FirstOrDefault();

            int PurchaseOrderIdCR = InsertStockTransferTransactionCR(TransferToBranchId, payload.value.VendorId, (payload.value.DefaultBuyingPrice * QTYTransfered), (payload.value.DefaultBuyingPrice * QTYTransfered), (payload.value.DefaultBuyingPrice * QTYTransfered));
            InsertStockTransferLineTransactionCR(PurchaseOrderIdCR, payload.value.UnitOfMeasureId.ToString(), payload.value.ProductId, "Credit - Internal Stock Transfer", "Credit - Internal Stock Transfer", payload.value.Quantity, payload.value.DefaultBuyingPrice, (payload.value.Quantity * payload.value.DefaultBuyingPrice), (payload.value.Quantity * payload.value.DefaultBuyingPrice), (payload.value.Quantity * payload.value.DefaultBuyingPrice));
            
            int PurchaseOrderIdDR = InsertStockTransferTransactionDR(Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId")), payload.value.VendorId, (payload.value.DefaultBuyingPrice * QTYTransfered) * -1, (payload.value.DefaultBuyingPrice * QTYTransfered) * -1, (payload.value.DefaultBuyingPrice * QTYTransfered) * -1);
            InsertStockTransferLineTransactionDR(PurchaseOrderIdDR, payload.value.UnitOfMeasureId.ToString(), payload.value.ProductId, "Credit - Internal Stock Transfer", "Credit - Internal Stock Transfer", payload.value.Quantity, payload.value.DefaultBuyingPrice, (payload.value.Quantity * payload.value.DefaultBuyingPrice) * -1, (payload.value.Quantity * payload.value.DefaultBuyingPrice) * -1, (payload.value.Quantity * payload.value.DefaultBuyingPrice) * -1);

            //Update Transfering Branch
            Stock stock = payload.value;
            payload.value.Quantity = TotalItemQTYRem;
            
            _context.Stock.Update(stock);
            _context.SaveChanges();
            return Ok(stock);
        }

        private void ValidateTransfer(int TransferingId, int ProductId, decimal QTYTransfered, [FromBody]Crud<Stock> payload, int TransferFromBranchId)
        {
            payload.value.IsAlocated = 1;

            int ProductExist = (from ExistingStock in _context.Stock.Where(x => x.BranchId == TransferingId && x.ProductId == ProductId) select ExistingStock.ProductId).FirstOrDefault();

            if (ProductExist == 0)
            {
                payload.value.Description = payload.value.ProductName;
                InsertNewStock(payload, QTYTransfered, TransferFromBranchId);
            }
            else
            {
                UpdateExistingStock(payload);
            }
        }

        public IActionResult InsertNewStock([FromBody]Crud<Stock> payload, decimal QTYTransfered, int TransferFromBranchId)
        {
            Stock stock = new Stock();

            var description = from Prod in _context.Product.Where(x => x.ProductId == stock.ProductId) select Prod.Description.FirstOrDefault();

            stock.BranchId = payload.value.BranchId;
            stock.BrandName = payload.value.BrandName;
            stock.DefaultBuyingPrice = payload.value.DefaultBuyingPrice;
            stock.Description = payload.value.Description;
            stock.ProductCode = payload.value.ProductCode;
            stock.ProductId = payload.value.ProductId;
            stock.ProductName = payload.value.ProductName;
            stock.Quantity = QTYTransfered;
            stock.UnitOfMeasureId = payload.value.UnitOfMeasureId;
            stock.VendorId = payload.value.VendorId;
            stock.IsAlocated = 1;
            stock.PurchaseOrderId = payload.value.PurchaseOrderId;
            stock.IsTransferedStock = true;
            stock.Notify = 1;
            stock.TransferFromBranchId = TransferFromBranchId;
            stock.TransferToBranchId = payload.value.BranchId;

            _context.Stock.Add(stock);
            _context.SaveChanges();

            return null;
        }

        public int InsertStockTransferTransactionCR(int BranchId, int VendorId, decimal Amount, decimal SubTotal, decimal Total)
        {
            PurchaseOrder purchaseOrder = new PurchaseOrder();
            purchaseOrder.PurchaseOrderName = _numberSequence.GetNumberSequence("CR_TRN");
            purchaseOrder.BranchId = BranchId;
            purchaseOrder.VendorId = VendorId;
            purchaseOrder.OrderDate = DateTime.Now;
            purchaseOrder.DeliveryDate = DateTime.Now;
            purchaseOrder.PurchaseTypeId = 3;
            purchaseOrder.AccountType = "CREDIT - Interal Stock Transfer";
            purchaseOrder.Remarks = "CREDIT - Interal Stock Transfer";
            purchaseOrder.Amount = Amount;
            purchaseOrder.SubTotal = SubTotal;
            purchaseOrder.Total = Total;
            purchaseOrder.SupplierInvTotal = 0;
            purchaseOrder.SupplierVatAmount = 0;
            purchaseOrder.Recieved = true;
            purchaseOrder.Visable = 0;
            purchaseOrder.TransactionTypeId = 2;

            _context.PurchaseOrder.Add(purchaseOrder);
            _context.SaveChanges();

            return purchaseOrder.PurchaseOrderId;
        }

        public void InsertStockTransferLineTransactionCR(int PurchaseOrderId, string UOM, int ProductId, string Description, string QuickNote, decimal Quantity, decimal Price, decimal Amount, decimal SubTotal, decimal Total)
        {
            PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
            purchaseOrderLine.PurchaseOrderId = PurchaseOrderId;
            purchaseOrderLine.UOM = UOM;
            purchaseOrderLine.ProductId = ProductId;
            purchaseOrderLine.Description = Description;
            purchaseOrderLine.QuickNote = QuickNote;
            purchaseOrderLine.Quantity = Quantity;
            purchaseOrderLine.Price = Price;
            purchaseOrderLine.SpecialPrice = 0;
            purchaseOrderLine.Amount = Amount;
            purchaseOrderLine.SubTotal = SubTotal;
            purchaseOrderLine.Total = Total;
            purchaseOrderLine.CreditLine = false;
            purchaseOrderLine.Credited = 0;

            _context.PurchaseOrderLine.Add(purchaseOrderLine);
            _context.SaveChanges();
        }

        public int InsertStockTransferTransactionDR(int BranchId, int VendorId, decimal Amount, decimal SubTotal, decimal Total)
        {
            PurchaseOrder purchaseOrder = new PurchaseOrder();
            purchaseOrder.PurchaseOrderName = _numberSequence.GetNumberSequence("DR_TRN");
            purchaseOrder.BranchId = BranchId;
            purchaseOrder.VendorId = VendorId;
            purchaseOrder.OrderDate = DateTime.Now;
            purchaseOrder.DeliveryDate = DateTime.Now;
            purchaseOrder.PurchaseTypeId = 3;
            purchaseOrder.AccountType = "DEBIT - Interal Stock Transfer";
            purchaseOrder.Remarks = "DEBIT - Interal Stock Transfer";
            purchaseOrder.Amount = Amount;
            purchaseOrder.SubTotal = SubTotal;
            purchaseOrder.Total = Total;
            purchaseOrder.SupplierInvTotal = 0;
            purchaseOrder.SupplierVatAmount = 0;
            purchaseOrder.Recieved = true;
            purchaseOrder.Visable = 0;
            purchaseOrder.TransactionTypeId = 4;

            _context.PurchaseOrder.Add(purchaseOrder);
            _context.SaveChanges();

            return purchaseOrder.PurchaseOrderId;
        }

        public void InsertStockTransferLineTransactionDR(int PurchaseOrderId, string UOM, int ProductId, string Description, string QuickNote, decimal Quantity, decimal Price, decimal Amount, decimal SubTotal, decimal Total)
        {
            PurchaseOrderLine purchaseOrderLine = new PurchaseOrderLine();
            purchaseOrderLine.PurchaseOrderId = PurchaseOrderId;
            purchaseOrderLine.UOM = UOM;
            purchaseOrderLine.ProductId = ProductId;
            purchaseOrderLine.Description = Description;
            purchaseOrderLine.QuickNote = QuickNote;
            purchaseOrderLine.Quantity = Quantity;
            purchaseOrderLine.Price = Price;
            purchaseOrderLine.SpecialPrice = 0;
            purchaseOrderLine.Amount = Amount;
            purchaseOrderLine.SubTotal = SubTotal;
            purchaseOrderLine.Total = Total;
            purchaseOrderLine.CreditLine = false;
            purchaseOrderLine.Credited = 0;

            _context.PurchaseOrderLine.Add(purchaseOrderLine);
            _context.SaveChanges();
        }

        public IActionResult UpdateExistingStock([FromBody]Crud<Stock> payload)
        {
            payload.value.Notify = 1;
            payload.value.TransferToBranchId = payload.value.BranchId; 
            payload.value.TransferFromBranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId")); ;
            payload.value.Description = payload.value.ProductName;

            Stock stock = payload.value;

            _context.Stock.Update(stock);
            _context.SaveChanges();

            return null;
        }
    }
}