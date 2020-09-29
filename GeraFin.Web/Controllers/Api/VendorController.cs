using GeraFin.DAL.DataAccess;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Vendor")]
    public class VendorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VendorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public int iBranchId;

        // GET: api/Vendor
        [HttpGet]
        public async Task<IActionResult> GetVendor()
        {
            List<Vendor> Items = await _context.Vendor.ToListAsync();
            int Count = Items.Count();
            return Ok(new { Items, Count });
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<Vendor> payload)
        {
            Vendor vendor = payload.value;
            _context.Vendor.Add(vendor);
            _context.SaveChanges();
            return Ok(vendor);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<Vendor> payload)
        {
            Vendor vendor = payload.value;
            _context.Vendor.Update(vendor);
            _context.SaveChanges();
            return Ok(vendor);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<Vendor> payload)
        {
            Vendor vendor = _context.Vendor.Where(x => x.VendorId == (int)payload.key).FirstOrDefault();
            _context.Vendor.Remove(vendor);
            _context.SaveChanges();
            return Ok(vendor);

        }
    }
}