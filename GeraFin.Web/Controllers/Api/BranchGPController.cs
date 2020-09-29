using System;
using System.Linq;
using GeraFin.DAL.Helpers;
using System.Data.SqlClient;
using System.Threading.Tasks;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/BranchGP")]
    public class BranchGPController : Controller
    {
        public string ConString = "";

        private readonly ApplicationDbContext _context;

        public BranchGPController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            ConString = configuration.GetConnectionString("DevEnvR");
        }

        [HttpGet]
        public async Task<IActionResult> GetBranchGP()
        {
            DateTime dtToday = DateTime.Now.Date;
            int day = dtToday.Day;
            int month = dtToday.Month;
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId == 0)
            {
                List<BranchGP> Items = await _context.BranchGP.Where(x => x.Month <= month && x.Day <= day).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            else
            {
                SqlHelper helper = new SqlHelper(ConString);
                SqlParameter[] Parameters = new SqlParameter[1];
                Parameters.SetValue(helper.CreateParameter("@BranchId", BranchId, 0), 0);
                helper.Update("GeraFin.stp_UpdateBranchGP_Daily", System.Data.CommandType.StoredProcedure, Parameters);

                List<BranchGP> Items = await _context.BranchGP.Where(x => x.BranchId == BranchId && x.Month <= month && x.Day <= day).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<BranchGP> payload)
        {
            BranchGP branchGP = payload.value;
            _context.BranchGP.Add(branchGP);
            _context.SaveChanges();
            return Ok(branchGP);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<BranchGP> payload)
        {
            BranchGP branchGP = payload.value;
            _context.BranchGP.Update(branchGP);
            _context.SaveChanges();
            return Ok(branchGP);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<BranchGP> payload)
        {
            BranchGP branchGP = _context.BranchGP.Where(x => x.GPId == (int)payload.key).FirstOrDefault();

            _context.BranchGP.Remove(branchGP);
            _context.SaveChanges();
            return Ok(branchGP);
        }
    }
}