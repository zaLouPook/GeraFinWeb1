using System;
using System.Linq;
using GeraFin.InterFaces.Factory;
using GeraFin.DAL.DataAccess;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/DailyIssues")]
    public class DailyIssuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INumberSequence _numberSequence;

        public DailyIssuesController(ApplicationDbContext context, INumberSequence numberSequence)
        {
            _context = context;
            _numberSequence = numberSequence;
        }

        // GET: api/DailyIssues
        [HttpGet]
        public async Task<IActionResult> GetDailyIssues()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId != 0)
            {
                List<DailyIssues> Items = await _context.DailyIssues.Where(x => x.BranchId == BranchId).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            else
            {
                List<DailyIssues> Items = await _context.DailyIssues.ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            DailyIssues result = await _context.DailyIssues.Where(x => x.DailyIssuesId.Equals(id)).Include(x => x.DailyIssuesLines).FirstOrDefaultAsync();

            return Ok(result);
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<DailyIssues> payload)
        {
            if (ModelState.IsValid)
            {
                int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

                payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));

                if (BranchId != 0)
                {
                    payload.value.IssueComplete = false;
                    payload.value.CustomerRefNumber = (from Branch in _context.Branch.Where(x => x.BranchId == BranchId) select Branch.BranchName).FirstOrDefault();

                    DailyIssues dailyIssues = payload.value;
                    dailyIssues.DailyIssuesName = _numberSequence.GetNumberSequence("SI");

                    _context.DailyIssues.Add(dailyIssues);
                    _context.SaveChanges();

                    return Ok(dailyIssues);
                }
                else
                {
                    payload.value.IssueComplete = false;
                    payload.value.CustomerRefNumber = (from Branch in _context.Branch.Where(x => x.BranchId == BranchId) select Branch.BranchName).FirstOrDefault();

                    DailyIssues dailyIssues = payload.value;
                    dailyIssues.DailyIssuesName = _numberSequence.GetNumberSequence("SI");

                    _context.DailyIssues.Add(dailyIssues);
                    _context.SaveChanges();

                    return Ok(dailyIssues);
                }
            }
            else
            {
                return null;
            }
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<DailyIssues> payload)
        {
            payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));

            DailyIssues dailyIssues = payload.value;

            List<DailyIssuesLine> lines = new List<DailyIssuesLine>();
            lines = _context.DailyIssuesLine.Where(x => x.DailyIssuesId.Equals(payload.value.DailyIssuesId)).ToList();

            dailyIssues.Amount = lines.Sum(x => x.Amount);
            dailyIssues.SubTotal = lines.Sum(x => x.SubTotal);
            dailyIssues.Total = lines.Sum(x => x.Total);

            _context.DailyIssues.Update(dailyIssues);
            _context.SaveChanges();

            if (payload.value.IssueComplete == true)
            {
                DailyIssuesLineController dailyissueslinecontroller = new DailyIssuesLineController(_context);

                DailyIssues dailyIssue = _context.DailyIssues.Where(x => x.DailyIssuesId == payload.value.DailyIssuesId).FirstOrDefault();

                foreach (var item in lines)
                {
                    dailyissueslinecontroller.UpdateStock(item.ProductId, item.Quantity, dailyIssue.BranchId);
                }
                dailyissueslinecontroller.Dispose();
            }
            return Ok(dailyIssues);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<DailyIssues> payload)
        {
            List<DailyIssuesLine> lines = _context.DailyIssuesLine.Where(x => x.DailyIssuesId == (int)payload.key).ToList();

            if (lines.Count == 0)
            {
                DailyIssues dailyIssues = _context.DailyIssues.Where(x => x.DailyIssuesId == (int)payload.key).FirstOrDefault();

                _context.DailyIssues.Remove(dailyIssues);
                _context.SaveChanges();

                return Ok(dailyIssues);
            }
            else
            {
                foreach (var line in lines)
                {
                    _context.DailyIssuesLine.Remove(line);
                    _context.SaveChanges();
                }
                DailyIssues dailyIssues = _context.DailyIssues.Where(x => x.DailyIssuesId == (int)payload.key).FirstOrDefault();

                _context.DailyIssues.Remove(dailyIssues);
                _context.SaveChanges();

                return Ok(dailyIssues);
            }
        }
    }
}