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
    [Route("api/DailyIncome")]
    public class DailyIncomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INumberSequence _numberSequence;

        public string _ConString = "";

        public DailyIncomeController(ApplicationDbContext context, INumberSequence numberSequence, IConfiguration configuration)
        {
            _context = context;
            _numberSequence = numberSequence;
            _ConString = configuration.GetConnectionString("DevEnvR");
        }

        // GET: api/DailyIncome
        [HttpGet]
        public async Task<IActionResult> GetDailyIncome()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId != 0)
            {
                List<DailyIncome> Items = await _context.DailyIncome.Where(x => x.BranchId == BranchId).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            else
            {
                List<DailyIncome> Items = await _context.DailyIncome.ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            DailyIncome result = await _context.DailyIncome.Where(x => x.SalesOrderId.Equals(id)).Include(x => x.SalesOrderLines).FirstOrDefaultAsync();

            return Ok(result);
        }

        private void UpdateSalesOrder(int salesOrderId)
        {
            try
            {
                DailyIncome dailyIncome = new DailyIncome();
                dailyIncome = _context.DailyIncome.Where(x => x.SalesOrderId.Equals(salesOrderId)).FirstOrDefault();

                if (dailyIncome != null)
                {
                    List<DailyIncomeLine> lines = new List<DailyIncomeLine>();
                    lines = _context.DailyIncomeLine.Where(x => x.SalesOrderId.Equals(salesOrderId)).ToList();

                    dailyIncome.SalesTypeId = 1;
                    dailyIncome.Amount = lines.Sum(x => x.Amount);
                    dailyIncome.SubTotal = lines.Sum(x => x.SubTotal);
                    dailyIncome.Total = lines.Sum(x => x.Total);

                    _context.Update(dailyIncome);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<DailyIncome> payload)
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (ModelState.IsValid)
            {
                payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));

                if (BranchId != 0)
                {
                    payload.value.BranchId = BranchId;
                    payload.value.SalesTypeId = 1;
                    payload.value.CustomerRefNumber = (from Branch in _context.Branch.Where(x => x.Email == User.Identity.Name) select Branch.BranchName).FirstOrDefault();

                    DailyIncome dailyIncome = payload.value;
                    dailyIncome.SalesOrderName = _numberSequence.GetNumberSequence("SO");
                    _context.DailyIncome.Add(dailyIncome);
                    _context.SaveChanges();

                    InsertIncomelines(dailyIncome.SalesOrderId, dailyIncome.BranchId);

                    this.UpdateSalesOrder(dailyIncome.SalesOrderId);
                    return Ok(dailyIncome);
                }
                else
                {
                    payload.value.CustomerRefNumber = (from Branch in _context.Branch.Where(x => x.BranchId == BranchId) select Branch.BranchName).FirstOrDefault();

                    DailyIncome dailyIncome = payload.value;
                    dailyIncome.SalesOrderName = _numberSequence.GetNumberSequence("SO");
                    _context.DailyIncome.Add(dailyIncome);
                    _context.SaveChanges();

                    InsertIncomelines(dailyIncome.SalesOrderId, dailyIncome.BranchId);

                    this.UpdateSalesOrder(dailyIncome.SalesOrderId);
                    return Ok(dailyIncome);
                }
            }
            return null;
        }

        public void InsertIncomelines(int SalesOrderId, int BranchId)
        {
            SqlHelper helper = new SqlHelper(_ConString);
            SqlParameter[] Parameters = new SqlParameter[2];
            Parameters.SetValue(helper.CreateParameter("@SalesOrderId", SalesOrderId, 0), 0);
            Parameters.SetValue(helper.CreateParameter("@BranchId", BranchId, 0), 1);

            helper.Insert("GeraFin.stp_InsertIncomeLines", System.Data.CommandType.StoredProcedure, Parameters);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<DailyIncome> payload)
        {
            DailyIncome dailyIncome = payload.value;
            dailyIncome.SalesTypeId = 1;
            dailyIncome.DeliveryDate = dailyIncome.OrderDate;

            _context.DailyIncome.Update(dailyIncome);
            _context.SaveChanges();

            return Ok(dailyIncome);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<DailyIncome> payload)
        {
            List<DailyIncomeLine> lines = _context.DailyIncomeLine.Where(x => x.SalesOrderId == (int)payload.key).ToList();

            if (lines.Count == 0)
            {
                DailyIncome dailyIncome = _context.DailyIncome.Where(x => x.SalesOrderId == (int)payload.key).FirstOrDefault();

                _context.DailyIncome.Remove(dailyIncome);
                _context.SaveChanges();
                return Ok(dailyIncome);
            }
            else
            {
                foreach (var line in lines)
                {
                    _context.DailyIncomeLine.Remove(line);
                    _context.SaveChanges();
                }
                DailyIncome dailyIncome = _context.DailyIncome.Where(x => x.SalesOrderId == (int)payload.key).FirstOrDefault();

                _context.DailyIncome.Remove(dailyIncome);
                _context.SaveChanges();

                return Ok(dailyIncome);
            }
        }
    }
}