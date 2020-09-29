using System;
using System.Linq;
using GeraFin.InterFaces.Factory;
using System.Threading.Tasks;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/WeeklySignOff")]
    public class WeeklySignOffController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INumberSequence _numberSequence;

        public WeeklySignOffController(ApplicationDbContext context, INumberSequence numberSequence)
        {
            _context = context;
            _numberSequence = numberSequence;
        }

        // GET: api/WeeklySignOff
        [HttpGet]
        public async Task<IActionResult> GetWeeklySignOff()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId != 0)
            {
                List<WeeklySignOff> Items = await _context.WeeklySignOff.Where(x => x.BranchId == BranchId).ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
            else
            {
                List<WeeklySignOff> Items = await _context.WeeklySignOff.ToListAsync();
                int Count = Items.Count();
                return Ok(new { Items, Count });
            }
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            WeeklySignOff result = await _context.WeeklySignOff.Where(x => x.WeeklySignOffId.Equals(id)).Include(x => x.WeeklySignOffLines).FirstOrDefaultAsync();
            return Ok(result);
        }

        private void UpdateWeeklySignOff(int WeeklySignOffId)
        {
            try
            {
                WeeklySignOff weeklySignOff = new WeeklySignOff();

                weeklySignOff = _context.WeeklySignOff.Where(x => x.WeeklySignOffId.Equals(WeeklySignOffId)).FirstOrDefault();
                weeklySignOff.CustomerRefNumber = (from Branch in _context.Branch.Where(x => x.BranchId == weeklySignOff.BranchId) select Branch.BranchName).FirstOrDefault();

                if (weeklySignOff == null)
                {
                    List<WeeklySignOffLine> lines = new List<WeeklySignOffLine>();
                    lines = _context.WeeklySignOffLine.Where(x => x.WeeklySignOffId.Equals(WeeklySignOffId)).ToList();

                    _context.Update(weeklySignOff);
                    _context.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<WeeklySignOff> payload)
        {
            if (ModelState.IsValid)
            {
                payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));
                payload.value.WeekEnd = Convert.ToDateTime(payload.value.WeekEnd.AddDays(1).ToString("yyyy-MM-dd"));

                int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

                if (BranchId != 0)
                {
                    payload.value.BranchId = BranchId;
                    payload.value.CustomerRefNumber = (from Branch in _context.Branch.Where(x => x.Email == User.Identity.Name) select Branch.BranchName).FirstOrDefault();

                    WeeklySignOff weeklySignOff = payload.value;
                    weeklySignOff.WeeklySignOffName = _numberSequence.GetNumberSequence("WSI");

                    _context.WeeklySignOff.Add(weeklySignOff);
                    _context.SaveChanges();

                    this.UpdateWeeklySignOff(weeklySignOff.WeeklySignOffId);

                    return Ok(weeklySignOff);
                }
                else
                {
                    payload.value.CustomerRefNumber = (from Branch in _context.Branch.Where(x => x.BranchId == payload.value.BranchId) select Branch.BranchName).FirstOrDefault();

                    WeeklySignOff weeklySignOff = payload.value;

                    weeklySignOff.WeeklySignOffName = _numberSequence.GetNumberSequence("WSI");

                    _context.WeeklySignOff.Add(weeklySignOff);
                    _context.SaveChanges();

                    this.UpdateWeeklySignOff(weeklySignOff.WeeklySignOffId);

                    return Ok(weeklySignOff);
                }
            }
            return null;
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<WeeklySignOff> payload)
        {
            payload.value.OrderDate = Convert.ToDateTime(payload.value.OrderDate.AddDays(1).ToString("yyyy-MM-dd"));
            payload.value.WeekEnd = Convert.ToDateTime(payload.value.WeekEnd.AddDays(1).ToString("yyyy-MM-dd"));

            WeeklySignOff weeklySignOff = payload.value;

            _context.WeeklySignOff.Update(weeklySignOff);
            _context.SaveChanges();

            return Ok(weeklySignOff);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<WeeklySignOff> payload)
        {
            WeeklySignOff weeklySignOff = _context.WeeklySignOff.Where(x => x.WeeklySignOffId == (int)payload.key).FirstOrDefault();

            _context.WeeklySignOff.Remove(weeklySignOff);
            _context.SaveChanges();

            return Ok(weeklySignOff);
        }
    }
}