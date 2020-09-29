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
    [Route("api/Specials")]
    public class SpecialsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpecialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetSpecials()
        {
            List<Specials> Items = await _context.Specials.ToListAsync();
            int Count = Items.Count();
            return Ok(new { Items, Count });
        }

        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<Specials> payload)
        {
            Specials specials = payload.value;
            _context.Specials.Add(specials);
            _context.SaveChanges();
            return Ok(specials);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<Specials> payload)
        {
            Specials specials = payload.value;
            _context.Specials.Update(specials);
            _context.SaveChanges();
            return Ok(specials);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<Specials> payload)
        {
            Specials specials = _context.Specials.Where(x => x.SpecialId == (int)payload.key).FirstOrDefault();

            _context.Specials.Remove(specials);
            _context.SaveChanges();
            return Ok(specials);

        }
    }
}