using System.Linq;
using GeraFin.Models.Pages;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.GeraFinWeb;
using System;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers
{
    [Authorize(Roles = MainMenu.PurchaseOrder.RoleName)]

    public class PurchaseOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            PurchaseOrder purchaseOrder = _context.PurchaseOrder.SingleOrDefault(x => x.PurchaseOrderId.Equals(id));

            //PopulateInvoiceDetails(Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId")), purchaseOrder.VendorId);

            if (purchaseOrder == null)
            {
                return NotFound();
            }
            return View(purchaseOrder);
        }

       //private void PopulateInvoiceDetails(int pOBranchId, int pOVendorId)
       // {
       //     ViewBag.BranchName = (from data in _context.Branch.Where(x => x.BranchId == pOBranchId) select data.BranchName).FirstOrDefault();
       //     ViewBag.DeliveryAddressLine0 = (from data in _context.Branch.Where(x => x.BranchId == pOBranchId) select data.Address).FirstOrDefault();
       //     ViewBag.DeliveryAddressLine1 = (from data in _context.Branch.Where(x => x.BranchId == pOBranchId) select data.City).FirstOrDefault();
       //     ViewBag.DeliveryAddressLine2 = (from data in _context.Branch.Where(x => x.BranchId == pOBranchId) select data.ZipCode).FirstOrDefault();
       //     ViewBag.ContactNo = (from data in _context.Branch.Where(x => x.BranchId == pOBranchId) select data.Phone).FirstOrDefault();
       //     ViewBag.ContactEmail = (from data in _context.Branch.Where(x => x.BranchId == pOBranchId) select data.Email).FirstOrDefault();

       //     ViewBag.Supplier = (from venderdata in _context.Vendor.Where(x => x.VendorId == pOVendorId) select venderdata.VendorName).FirstOrDefault();
       //     ViewBag.SupplierDeliveryAddressLine0 = (from venderdata in _context.Vendor.Where(x => x.VendorId == pOVendorId) select venderdata.Address).FirstOrDefault();
       //     ViewBag.SupplierDeliveryAddressLine1 = (from venderdata in _context.Vendor.Where(x => x.VendorId == pOVendorId) select venderdata.City).FirstOrDefault();
       //     ViewBag.SupplierDeliveryAddressLine2 = (from venderdata in _context.Vendor.Where(x => x.VendorId == pOVendorId) select venderdata.ZipCode).FirstOrDefault();
       //     ViewBag.SupplierPhoneNo = (from venderdata in _context.Vendor.Where(x => x.VendorId == pOVendorId) select venderdata.Phone).FirstOrDefault();
       //     ViewBag.SupplierEmail = (from venderdata in _context.Vendor.Where(x => x.VendorId == pOVendorId) select venderdata.Email).FirstOrDefault();
       // }
    }
}