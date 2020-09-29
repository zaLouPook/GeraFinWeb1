using System.Linq;
using System.Threading.Tasks;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Branches")]
    public class MyBranchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyBranchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetBranch()
        {
            List<Branch> Items = await _context.Branch.ToListAsync();
            int Count = Items.Count();
            return Ok(new { Items, Count });
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

            branch.RegionalEmail = User.Identity.Name;
            branch.LoginRequired = true;

            _context.Branch.Update(branch);
            _context.SaveChanges();

            return Ok(branch);
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