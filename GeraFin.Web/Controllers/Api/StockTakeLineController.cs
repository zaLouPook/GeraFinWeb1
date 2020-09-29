using System;
using System.Linq;
using System.Data.SqlClient;
using GeraFin.DAL.DataAccess;
using System.Threading.Tasks;
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
    [Route("api/StockTakeLine")]
    public class StockTakeLineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockTakeLineController(ApplicationDbContext context)
        {
            _context = context;
        }

        public static int StockTakeIdUpdate;

        // GET: api/StockTakeLine
        [HttpGet]
        public async Task<IActionResult> GetStockTakeLine()
        {
            var headers = Request.Headers["StockTakeId"];
            int StockTakeId = Convert.ToInt32(headers);

            StockTakeIdUpdate = StockTakeId;

            StockTake stocktake = _context.StockTake.Where(x => x.StockTakeId == StockTakeId).FirstOrDefault();

            if (stocktake.Rollover == true)
            {
                List<StockTakeLine> Items = await _context.StockTakeLine.OrderByDescending(x => x.StoreRoomCount).Where(x => x.StockTakeId == StockTakeId && x.StoreRoomCount != 0).Distinct().ToListAsync();

                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            else
            {
                //UpdateStockTakeLinesVarrience(stocktake.BranchId);

                List<StockTakeLine> Items = await _context.StockTakeLine.OrderByDescending(x => x.StoreRoomCount).Where(x => x.StockTakeId == StockTakeId).Distinct().ToListAsync();

                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<StockTakeLine> payload)
        {
            StockTakeLine stockTakeLine = payload.value;

            _context.StockTakeLine.Add(stockTakeLine);
            _context.SaveChanges();

            return Ok(stockTakeLine);
        }

        //public void UpdateStockTakeLinesVarrience(int BranchId)
        //{
        //    _context.Database.ExecuteSqlCommand("GeraFin.stp_UpdateStockTakeLine_Varience {0}", new SqlParameter("@BranchId", BranchId));
        //}

        public void UpdateStockTakeLines(int StockTakeId)
        {
            _context.Database.ExecuteSqlCommand("GeraFin.stp_UpdateStockTakeLinesBranch {0}", new SqlParameter("@StockTakeId", StockTakeId));
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<StockTakeLine> payload)
        {
            payload.value.OpeningBalanceCount = payload.value.StoreRoomCount;

            StockTakeLine stockTakeline = payload.value;

            _context.StockTakeLine.Update(stockTakeline);
            _context.SaveChanges();

            UpdateStockTakeLines(stockTakeline.StockTakeId);

            return Ok(stockTakeline);
        }
    }
}