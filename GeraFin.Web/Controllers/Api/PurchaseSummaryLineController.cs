using System;
using System.Linq;
using System.Data;
using GeraFin.DAL.Helpers;
using System.Data.SqlClient;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/PurchaseSummaryLine")]

    public class PurchaseSummaryLineController : Controller
    {
        public string _ConString = "";
        public int InvoiceOrderId;
        private readonly ApplicationDbContext _context;

        public PurchaseSummaryLineController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _ConString = configuration.GetConnectionString("DevEnvR");
        }

        // GET: api/PurchaseSummaryLine
        [HttpGet]
        public IActionResult GetPurchaseSummaryLine()
        {
            var headers = Request.Headers["InvoiceOrderId"];
            InvoiceOrderId = Convert.ToInt32(headers);

            int data = _context.PurchaseSummaryLine.Where(x => x.InvoiceOrderId.Equals(InvoiceOrderId)).Count();

            if (data != 0)
            {
                List<PurchaseSummaryLine> Items = _context.PurchaseSummaryLine.Distinct().Where(x => x.InvoiceOrderId.Equals(InvoiceOrderId)).ToList();

                UpdatePurchaseSummary(InvoiceOrderId);

                return Ok(new { Items, data });
            }
            else
            {
                int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));
                DateTime StartDate = (from InvOrd in _context.PurchaseSummary.Where(x => x.InvoiceOrderId == InvoiceOrderId) select InvOrd.OrderDate).FirstOrDefault();
                DateTime EndDate = (from InvOrd in _context.PurchaseSummary.Where(x => x.InvoiceOrderId == InvoiceOrderId) select InvOrd.DeliveryDate).FirstOrDefault();

                SqlHelper helper = new SqlHelper(_ConString);
                SqlParameter[] Parameters = new SqlParameter[3];
                Parameters.SetValue(helper.CreateParameter("@BranchId", BranchId, 0), 0);
                Parameters.SetValue(helper.CreateParameter("@StartDate", StartDate, 0), 1);
                Parameters.SetValue(helper.CreateParameter("@EndDate", EndDate.ToString(), 0), 2);

                IDataReader reader = helper.GetDataReader("GeraFin.stp_GetWeeklyInvoice", CommandType.StoredProcedure, Parameters, out SqlConnection connection);

                List<PurchaseSummaryLine> Items = new List<PurchaseSummaryLine>();

                while (reader.Read())
                {
                    Items.Add(new PurchaseSummaryLine()
                    {
                        Date = Convert.ToString(reader[0]),
                        InvoiceNumber = Convert.ToString(reader[1]),
                        SupplierName = Convert.ToString(reader[2]),
                        TotalExclVAT = Convert.ToDecimal(reader[3]),
                        VAT = Convert.ToDecimal(reader[4]),
                        TotalInclVAT = Convert.ToDecimal(reader[5]),
                        SupplierInvoiceTotal = Convert.ToDecimal(reader[6]),
                        SupplierInvoiceVAT = Convert.ToDecimal(reader[7]),
                        SupplierTotalinclVAT = Convert.ToDecimal(reader[8]),
                        SupplierInvoiceNo = Convert.ToString(reader[9]),
                    });
                }
                connection.Dispose();

                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        private void UpdatePurchaseSummary(int invoiceOrderId)
        {
            try
            {
                PurchaseSummary purchaseSummary = new PurchaseSummary();
                purchaseSummary = _context.PurchaseSummary.Where(x => x.InvoiceOrderId.Equals(invoiceOrderId)).FirstOrDefault();

                if (purchaseSummary != null)
                {
                    List<PurchaseSummary> Sumarylines = new List<PurchaseSummary>();
                    Sumarylines = _context.PurchaseSummary.Where(x => x.InvoiceOrderId.Equals(invoiceOrderId)).ToList();

                    List<PurchaseSummaryLine> lines = new List<PurchaseSummaryLine>();
                    lines = _context.PurchaseSummaryLine.Where(x => x.InvoiceOrderId.Equals(invoiceOrderId)).ToList();

                    //TotalInclVAT = lines.Select(x => x.SupplierTotalinclVAT);
                    var TotalInclVAT = lines.Select(x => x.SupplierInvoiceTotal + x.SupplierInvoiceVAT);

                    purchaseSummary.Amount = lines.Sum(x => x.TotalExclVAT);
                    purchaseSummary.SubTotal = lines.Sum(x => x.VAT);
                    purchaseSummary.Total = purchaseSummary.Amount + purchaseSummary.SubTotal;

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
        public IActionResult ExportToExcel()
        {
            return View();
        }
    }
}