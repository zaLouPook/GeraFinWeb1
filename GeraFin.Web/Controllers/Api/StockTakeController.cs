using System;
using System.Linq;
using GeraFin.InterFaces.Factory;
using GeraFin.DAL.Helpers;
using System.Data.SqlClient;
using System.Threading.Tasks;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/StockTake")]
    public class StockTakeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INumberSequence _numberSequence;

        public string _ConString = "";

        public StockTakeController(ApplicationDbContext context, INumberSequence numberSequence, IConfiguration configuration)
        {
            _context = context;
            _numberSequence = numberSequence;
            _ConString = configuration.GetConnectionString("DevEnvR");
        }

        public static int StockTakeIdReq;

        // GET: api/StockTake
        [HttpGet]
        public async Task<IActionResult> GetStockTake()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId != 0)
            {
                List<StockTake> Items = await _context.StockTake.Where(x => x.BranchId == BranchId).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            else
            {
                List<StockTake> Items = await _context.StockTake.ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            StockTake result = await _context.StockTake.Where(x => x.StockTakeId.Equals(id)).Include(x => x.StockTakeLines).FirstOrDefaultAsync();

            return Ok(result);
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<StockTake> payload)
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));
            payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));
            payload.value.DeliveryDate = Convert.ToDateTime(payload.value.DeliveryDate.AddDays(1).ToString("yyyy-MM-dd"));

            if (BranchId != 0)
            {
                payload.value.CustomerRefNumber = (from Branch in _context.Branch.Where(x => x.BranchId == BranchId) select Branch.BranchName).FirstOrDefault();

                StockTake stocktake = payload.value;
                stocktake.StockTakeName = _numberSequence.GetNumberSequence("IN");
                _context.StockTake.Add(stocktake);
                _context.SaveChanges();

                InsertStockTakeLines(BranchId, stocktake.StockTakeId);

                return Ok(stocktake);
            }
            else
            {
                payload.value.CustomerRefNumber = (from Branch in _context.Branch.Where(x => x.BranchId ==BranchId) select Branch.BranchName).FirstOrDefault();

                StockTake stockTake = payload.value;
                stockTake.StockTakeName = _numberSequence.GetNumberSequence("IN");
                _context.StockTake.Add(stockTake);
                _context.SaveChanges();

                InsertStockTakeLines(stockTake.BranchId, stockTake.StockTakeId);

                return Ok(stockTake);
            }
        }

        public void InsertStockTakeLines(int BranchId, int StockTakeId)
        {
            SqlHelper helper = new SqlHelper(_ConString);
            SqlParameter[] Parameters = new SqlParameter[2];
            Parameters.SetValue(helper.CreateParameter("@BranchId", BranchId, 0), 0);
            Parameters.SetValue(helper.CreateParameter("@StockTakeId", StockTakeId, 0), 1);

            helper.Insert("GeraFin.stp_InsertStockTakeLinesBranch", System.Data.CommandType.StoredProcedure, Parameters);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<StockTake> payload)
        {
            UpdateStockPostStockTake(Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId")));

            payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));
            payload.value.DeliveryDate = Convert.ToDateTime(payload.value.DeliveryDate.AddDays(1).ToString("yyyy-MM-dd"));

            StockTake stockTake = payload.value;
            _context.StockTake.Update(stockTake);
            _context.SaveChanges();
            return Ok(stockTake);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<StockTake> payload)
        {
            List<StockTakeLine> lines = _context.StockTakeLine.Where(x => x.StockTakeId == (int)payload.key).ToList();

            if (lines.Count == 0)
            {
                StockTake stockTake = _context.StockTake.Where(x => x.StockTakeId == (int)payload.key).FirstOrDefault();
                _context.StockTake.Remove(stockTake);
                _context.SaveChanges();
                return Ok(stockTake);
            }
            else
            {
                foreach (var item in lines)
                {
                    _context.Remove(item);
                    _context.SaveChanges();
                }
            }
            return null;
        }
        public void UpdateStockPostStockTake(int BranchId)
        {
            _context.Database.ExecuteSqlCommand((RawSqlString)"GeraFin.stp_UpdateStockPostStockTake {0}", (object)new SqlParameter("@BranchId", (object)BranchId));
        }
    }
}