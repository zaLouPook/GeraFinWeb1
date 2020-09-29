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
using Syncfusion.EJ.Export;
using Syncfusion.JavaScript.Models;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;
using Syncfusion.Pdf;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/PurchaseOrderLine")]
    public class PurchaseOrderLineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderLineController(ApplicationDbContext context)
        {
            _context = context;
        }

        public static int PurchaseId;
        
        // GET: api/PurchaseOrderLine
        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrderLine()
        {
            var headers = Request.Headers["PurchaseOrderId"];
            int purchaseOrderId = Convert.ToInt32(headers);
            PurchaseId = purchaseOrderId;

            try
            {
                List<PurchaseOrderLine> Items = await _context.PurchaseOrderLine.Where(x => x.PurchaseOrderId.Equals(purchaseOrderId)).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            catch(Exception ex)
            {

            }
            return null;
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            PurchaseOrder result = await _context.PurchaseOrder.Where(x => x.PurchaseOrderId.Equals(id)).Include(x => x.PurchaseOrderLines).FirstOrDefaultAsync();

            result.PurchaseOrderLines.Select(x => x.ProductId).ToList();

            return Ok(result);
        }

        private PurchaseOrderLine Recalculate(PurchaseOrderLine purchaseOrderLine)
        {
            try
            {
                if (purchaseOrderLine.Price == 0)
                {
                    purchaseOrderLine.Amount = Convert.ToDecimal(purchaseOrderLine.Quantity) * purchaseOrderLine.SpecialPrice;
                    purchaseOrderLine.SubTotal = purchaseOrderLine.Amount;
                    purchaseOrderLine.Total = purchaseOrderLine.SubTotal;
                }
                else
                {
                    purchaseOrderLine.Amount = Convert.ToDecimal(purchaseOrderLine.Quantity) * purchaseOrderLine.Price;
                    purchaseOrderLine.SubTotal = purchaseOrderLine.Amount;
                    purchaseOrderLine.Total = purchaseOrderLine.SubTotal;
                }

            }
            catch (Exception)
            {
                throw;
            }
            return purchaseOrderLine;
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

                    //update master data by its lines
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
        public IActionResult EmailPDF(string GridModel)
        {
            JsonConvert.SerializeObject(GridModel);

            PdfExport exp = new PdfExport();
            GridPdfExport gridPdfExport = new GridPdfExport();

            var DataSource = _context.PurchaseOrderLine.Where(x => x.PurchaseOrderId == PurchaseId).ToListAsync();
            
            GridProperties obj = ConvertGridObject(GridModel);
            
            JsonConvert.SerializeObject(obj);
            
            exp.Export(obj, DataSource, false, gridPdfExport);

            return null;
        }

        private GridProperties ConvertGridObject(string gridProperty)
        {
            IEnumerable div = (IEnumerable)JsonConvert.DeserializeObject(gridProperty.ToString(), typeof(IEnumerable));
            GridProperties gridProp = new GridProperties();
            foreach (KeyValuePair<string, object> ds in div)
            {
                var property = gridProp.GetType().GetProperty(ds.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    Type type = property.PropertyType;
                    string serialize = JsonConvert.SerializeObject(ds.Value);
                    object value = JsonConvert.DeserializeObject(serialize, type);
                    property.SetValue(gridProp, value, null);
                }
            }
            return gridProp;
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<PurchaseOrderLine> payload)
        {
            if (ModelState.IsValid)
            {
                string UomString = (from products in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select products.UnitOfMeasureId.ToString()).FirstOrDefault();
                Int32.TryParse(UomString, out int UOMx);
                var UomUom = (from Uom in _context.UnitOfMeasure.Where(x => x.UnitOfMeasureId == UOMx) select Uom.UnitOfMeasureName).FirstOrDefault();
                payload.value.UOM = UomUom;
                payload.value.Price = Convert.ToDecimal((from products in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select products.DefaultSellingPrice).FirstOrDefault());
                payload.value.Description = (from products in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select products.ProductCode).FirstOrDefault();
                payload.value.SpecialPrice = Convert.ToDecimal((from products in _context.Product.Where(x => x.ProductId == payload.value.ProductId) select products.SpecialPrice).FirstOrDefault());

                PurchaseOrderLine purchaseOrderLine = payload.value;
                purchaseOrderLine = this.Recalculate(purchaseOrderLine);

                _context.PurchaseOrderLine.Add(purchaseOrderLine);
                _context.SaveChanges();

                this.UpdatePurchaseOrder(purchaseOrderLine.PurchaseOrderId);
                this.Recalculate(purchaseOrderLine);

                int BranchId = _context.PurchaseOrder.Where(x => x.PurchaseOrderId == purchaseOrderLine.PurchaseOrderId).Select(x => x.BranchId).FirstOrDefault();

                this.InsertStock(payload.value.ProductId, payload.value.Quantity, purchaseOrderLine.PurchaseOrder.PurchaseOrderId, BranchId);

                return Ok(purchaseOrderLine);
            }
            return null;
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<PurchaseOrderLine> payload)
        {
            string QN = payload.value.QuickNote;

            PurchaseOrderLine purchaseOrderLine = payload.value;
            purchaseOrderLine = this.Recalculate(purchaseOrderLine);

            _context.PurchaseOrderLine.Update(purchaseOrderLine);
            _context.SaveChanges();

            this.UpdatePurchaseOrder(purchaseOrderLine.PurchaseOrderId);

            return Ok(purchaseOrderLine);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<PurchaseOrderLine> payload)
        {
            PurchaseOrderLine purchaseOrderLine = _context.PurchaseOrderLine.Where(x => x.PurchaseOrderLineId == (int)payload.key).FirstOrDefault();

            _context.PurchaseOrderLine.Remove(purchaseOrderLine);
            _context.SaveChanges();

            this.UpdatePurchaseOrder(purchaseOrderLine.PurchaseOrderId);

            return Ok(purchaseOrderLine);
        }

        public void InsertStock(int ProductId, decimal Quantity, int PurchaseOrderId, int BranchId)
        {
            var Prod = _context.Product.Where(x => x.ProductId == ProductId).FirstOrDefault();
            var ProdIdExist = _context.Stock.Where(x => x.ProductId == ProductId && x.BranchId == BranchId).FirstOrDefault();
            int prodId = ProdIdExist.ProductId;

            if(prodId == 0) //New Stock
            {
                Stock stock = new Stock
                {
                    ProductId = Prod.ProductId,
                    BranchId = BranchId,
                    VendorId = Prod.VendorId,
                    Quantity = Quantity,
                    DefaultBuyingPrice = Prod.DefaultBuyingPrice,
                    Description = Prod.Description,
                    ProductCode = Prod.ProductCode,
                    ProductName = Prod.ProductName,
                    BrandName = Prod.BrandName,
                    UnitOfMeasureId = Prod.UnitOfMeasureId,
                    IsAlocated = 0,
                    PurchaseOrderId = PurchaseOrderId,
                    Notify = 0,
                    TransferFromBranchId = 0,
                    TransferToBranchId = 0,
                    TotalStock = Quantity * Prod.DefaultBuyingPrice
                };
                _context.Stock.Add(stock);
                _context.SaveChanges();
            }
            else
            {
                Stock stock = _context.Stock.Where(x => x.ProductId == ProductId && x.BranchId == BranchId).FirstOrDefault();
                {
                    stock.Quantity = Quantity + ProdIdExist.Quantity;
                };
                _context.Stock.Update(stock);
                _context.SaveChanges();
            }

        }
        public void RemoveFromStock(int UpdateLine, int ProdId)
        {
            Stock stock = _context.Stock.Where(x => x.ProductId == UpdateLine && x.PurchaseOrderId == ProdId).FirstOrDefault();

            if (stock.StockId != 0)
            {
                _context.Stock.Remove(stock);
                _context.SaveChanges();
            }
        }

        //[HttpPost("[action]")]
        //public IActionResult ExportToPdf([FromBody] PurchaseOrderLine model)
        //{
        //    try
        //    {
        //        return (IActionResult)this.File(GridExportController.CreatePDFFile(model), "application/pdf");
        //    }
        //    catch (Exception ex)
        //    {
        //        return (IActionResult)this.Json((object)new
        //        {
        //            isError = true,
        //            errorMessage = ex.Message
        //        });
        //    }
        //}
    }
}