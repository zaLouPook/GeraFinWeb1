using System;
using System.Linq;
using GeraFin.DAL.DataAccess;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Branch")]
    public class BranchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BranchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBranch()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId == 0)
            {
                List<Branch> Items = await _context.Branch.Where(x => x.LoginRequired == true).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            else
            {
                List<Branch> Items = await _context.Branch.Where(x => x.LoginRequired == true && x.BranchId == BranchId).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<Branch> payload)
        {
            Branch branch = payload.value;
            _context.Branch.Add(branch);
            _context.SaveChanges();
            return Ok(branch);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<Branch> payload)
        {
            Branch branch = payload.value;

            if(branch.ViewBranch == true)
            {
                branch.RegionalEmail = User.Identity.Name;
                branch.LoginRequired = true;
                branch.ViewBranch = true;

                _context.Branch.Update(branch);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                branch.RegionalEmail = null;
                branch.LoginRequired = true;
                branch.ViewBranch = false;

                _context.Branch.Update(branch);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<Branch> payload)
        {
            Branch branch = _context.Branch.Where(x => x.BranchId == (int)payload.key).FirstOrDefault();

            _context.Branch.Remove(branch);
            _context.SaveChanges();
            return Ok(branch);
        }
    }
}