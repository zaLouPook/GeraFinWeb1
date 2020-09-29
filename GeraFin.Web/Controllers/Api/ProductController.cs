using System;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Syncfusion.EJ.Export;
using System.Collections;
using Newtonsoft.Json;
using Syncfusion.JavaScript.Models;
using System.Reflection;
using Syncfusion.XlsIO;
using Syncfusion.OfficeChart.Implementation;
using Newtonsoft.Json.Schema;
using System.IO;
using ClosedXML.Excel;
using System.Data;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Product/[action]")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetProduct()
        {
            int iBranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (iBranchId != 0)
            {
                List<Product> Items = await _context.Product.Where(x => x.BranchId == iBranchId && x.Active == true && x.ProductTypeId == 1)
                    .OrderBy(x => x.ProductId)
                    .ThenBy(x => x.ProductName)
                    .ThenBy(x => x.BrandName)
                    .ThenBy(x => x.VendorId).ToListAsync();

                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            else
            {
                List<Product> Items = await _context.Product.Where(x => x.Active == true)
                    .OrderBy(x => x.ProductId)
                    .ThenBy(x => x.ProductName)
                    .ThenBy(x => x.BrandName)
                    .ThenBy(x => x.VendorId).ToListAsync();

                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProductById(int id)
        {
            int iBranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            List<Product> Items = await _context.Product.Where(x => x.Active == true && x.ProductId == id && x.BranchId == iBranchId)
                    .OrderBy(x => x.ProductId)
                    .ThenBy(x => x.ProductName)
                    .ThenBy(x => x.BrandName)
                    .ThenBy(x => x.VendorId).ToListAsync();

            int Count = Items.Count();
            return Ok(new { Items, Count });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductByTypeId(int id)
        {
            int iBranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            List<Product> Items = await _context.Product.Where(x => x.Active == true && x.ProductTypeId == id && x.BranchId == iBranchId)
                    .OrderBy(x => x.ProductId)
                    .ThenBy(x => x.ProductName)
                    .ThenBy(x => x.BrandName)
                    .ThenBy(x => x.VendorId).ToListAsync();

            int Count = Items.Count();
            return Ok(new { Items, Count });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductByVendorId(int id)
        {
            int iBranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            List<Product> Items = await _context.Product.Where(x => x.Active == true && x.VendorId == id && x.BranchId == iBranchId)
                    .OrderBy(x => x.ProductId)
                    .ThenBy(x => x.ProductName)
                    .ThenBy(x => x.BrandName)
                    .ThenBy(x => x.VendorId).ToListAsync();

            int Count = Items.Count();
            return Ok(new { Items, Count });
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody] Crud<Product> payload)
        {
            payload.value.RowCol = 0;
            payload.value.DefaultSellingPrice = payload.value.DefaultBuyingPrice;
            payload.value.Description = payload.value.ProductName;

            Product product = payload.value;
            _context.Product.Add(product);
            _context.SaveChanges();
            return Ok(product);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody] Crud<Product> payload)
        {
            payload.value.RowCol = 0;
            Product product = payload.value;
            _context.Product.Update(product);
            _context.SaveChanges();

            if (payload.value.SpecialPrice != 0)
            {
                CreateNewSpecial(payload.value.ProductId);
            }
            return Ok(product);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody] Crud<Product> payload)
        {
            Product product = _context.Product.Where(x => x.ProductId == (int)payload.key).FirstOrDefault();
            _context.Product.Remove(product);
            _context.SaveChanges();
            return Ok(product);
        }

        public void CreateNewSpecial(int ProductId)
        {
            _context.Database.ExecuteSqlCommand("GeraFin.stp_CreateNewSpecial {0}", new SqlParameter("@ProductId", ProductId));
        }

        [HttpPost("[action]")]
        public IActionResult ExportToExcel()
        {
            List<Product> Items = _context.Product.Where(x => x.ProductTypeId == 1).ToList();
            List<Branch> Brnc = _context.Branch.ToList();
            List<Vendor> Vend = _context.Vendor.ToList();
            List<UnitOfMeasure> UOMe = _context.UnitOfMeasure.ToList();

            IList<string> ProdList = new List<string>();

            var innerJoinQuery = from product in Items
                                 join Brn in Brnc on product.BranchId equals Brn.BranchId
                                 join Ven in Vend on product.VendorId equals Ven.VendorId
                                 join Uom in UOMe on product.UnitOfMeasureId equals Uom.UnitOfMeasureId
                                 select new
                                 {
                                     product.ProductName,
                                     Ven.VendorName,
                                     Price = product.DefaultSellingPrice.ToString(),
                                     product.Description,
                                     product.ProductCode,
                                     BranndName = product.BrandName,
                                     UnitOfMesure = Uom.UnitOfMeasureName,
                                 };

            innerJoinQuery.ToList();

            DataTable dataTable = ConvertToDataTable(innerJoinQuery.ToList());

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable, "Full");

                MemoryStream fs = new MemoryStream();
                wb.SaveAs(fs);
                fs.Position = 0;

                return new FileStreamResult(fs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "FullProductList.xlsx"
                };
            }
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}




//var Array = Items.ToArray();

//for (int i = 0; i < Array.Length; i++)
//{
//    if (Array[i].BrandName == Array[i + 1].BrandName)
//    {
//        int ProdId0 = Array[i].ProductId;
//        int ProdId1 = Array[i+1].ProductId;
//        decimal Price = Math.Min(Array[i].DefaultSellingPrice, Array[i + 1].DefaultSellingPrice);

//        int UpdateId = _context.Product.Where(x => x.ProductId == ProdId0 && x.DefaultSellingPrice == Price).Select(x => x.ProductId).FirstOrDefault();

//        if(UpdateId == 0)
//        {
//            UpdateId = _context.Product.Where(x => x.ProductId == ProdId1 && x.DefaultSellingPrice == Price).Select(x => x.ProductId).FirstOrDefault();
//        }

//        Update(UpdateId);
//    }
//}
//Items.ToList();
//int Count = Items.Count();
//            return Ok(new { Items, Count
//});




//        private void Update(int ProdId)
//        {
//            Product product = _context.Product.Where(x => x.ProductId == ProdId).FirstOrDefault();
//            product.RowCol = 1;

//            _context.Update(product);
//            _context.SaveChanges();
//        }


