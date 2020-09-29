using System;
using System.Data;
using System.Linq;
using GeraFin.DAL.Helpers;
using System.Data.SqlClient;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/WeeklySignOffLine")]
    public class WeeklySignOffLineController : Controller
    {
        public string ConString = "";

        private readonly ApplicationDbContext _context;

        public WeeklySignOffLineController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            ConString = configuration.GetConnectionString("DevEnvR");
        }

        // GET: api/GetWeeklySignOffLine
        [HttpGet]
        public IActionResult GetWeeklySignOffLine()
        {
            var headers = Request.Headers["WeeklySignOffId"];
            int weeklySignOffId = Convert.ToInt32(headers);

            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));
            DateTime StartDate = (from InvOrd in _context.WeeklySignOff.Where(x => x.WeeklySignOffId == weeklySignOffId) select InvOrd.OrderDate).FirstOrDefault();
            DateTime EndDate = (from InvOrd in _context.WeeklySignOff.Where(x => x.WeeklySignOffId == weeklySignOffId) select InvOrd.WeekEnd).FirstOrDefault();

            SqlHelper helper = new SqlHelper(ConString);
            SqlParameter[] Parameters = new SqlParameter[3];
            Parameters.SetValue(helper.CreateParameter("@DT", StartDate, 0), 0);
            Parameters.SetValue(helper.CreateParameter("@ENDDATE", EndDate, 0), 1);
            Parameters.SetValue(helper.CreateParameter("@BranchId", BranchId, 0), 2);

            IDataReader reader = helper.GetDataReader("GeraFin.stp_GetWeeklyProcessSummary", CommandType.StoredProcedure, Parameters, out SqlConnection connection);

            List<WeeklySignOffLine> Items = new List<WeeklySignOffLine>();

            while (reader.Read())
            {
                Items.Add(new WeeklySignOffLine()
                {
                    ProductName = Convert.ToString(reader[0]),
                    Price = Convert.ToDecimal(reader[1]),
                    Sun = Convert.ToDecimal(reader[2]),
                    Mon = Convert.ToDecimal(reader[3]),
                    Tue = Convert.ToDecimal(reader[4]),
                    Wed = Convert.ToDecimal(reader[5]),
                    Thu = Convert.ToDecimal(reader[6]),
                    Fri = Convert.ToDecimal(reader[7]),
                    Sat = Convert.ToDecimal(reader[8]),
                    QTY = Convert.ToDecimal(reader[9]),
                    ZAR = Convert.ToDecimal(reader[10]),
                });
            }
            connection.Dispose();

            UpdateWeeklySignOff(weeklySignOffId, Items.Sum(x => x.QTY), (Items.Sum(x => x.ZAR)));

            int Count = Items.Count();
            return Ok(new { Items, Count });
        }

        public void UpdateWeeklySignOff(int weeklysignoffid, decimal weeklycount, decimal weeklyamt)
        {
            WeeklySignOff weeklysignoff = _context.WeeklySignOff.Where(x => x.WeeklySignOffId == weeklysignoffid).FirstOrDefault();
            weeklysignoff.WeeklyCount = weeklycount;
            weeklysignoff.WeeklyAmout = weeklyamt;
            _context.WeeklySignOff.Update(weeklysignoff);
            _context.SaveChanges();
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<WeeklySignOffLine> payload)
        {
            WeeklySignOffLine weeklySignOffLine = payload.value;
            _context.WeeklySignOffLine.Add(weeklySignOffLine);
            _context.SaveChanges();
            return Ok(weeklySignOffLine);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<WeeklySignOffLine> payload)
        {
            WeeklySignOffLine weeklySignOffLine = payload.value;
            _context.WeeklySignOffLine.Update(weeklySignOffLine);
            _context.SaveChanges();
            return Ok(weeklySignOffLine);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<WeeklySignOffLine> payload)
        {
            WeeklySignOffLine weeklySignOffLine = _context.WeeklySignOffLine.Where(x => x.WeeklySignOffId == (int)payload.key).FirstOrDefault();
            _context.WeeklySignOffLine.Remove(weeklySignOffLine);
            _context.SaveChanges();
            return Ok(weeklySignOffLine);
        }
    }
}