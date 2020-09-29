using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/TransactionType")]
    public class TransactionTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/TransactionType
        [HttpGet]
        public async Task<IActionResult> GetTransactionType()
        {
            List<TransactionType> Items = await _context.TransactionType.ToListAsync();
            int Count = Items.Count();
            return Ok(new { Items, Count });
        }



        [HttpPost("[action]")]
        public IActionResult Insert([FromBody]Crud<TransactionType> payload)
        {
            TransactionType productType = payload.value;
            _context.TransactionType.Add(productType);
            _context.SaveChanges();
            return Ok(productType);
        }

        [HttpPost("[action]")]
        public IActionResult Update([FromBody]Crud<TransactionType> payload)
        {
            TransactionType productType = payload.value;
            _context.TransactionType.Update(productType);
            _context.SaveChanges();
            return Ok(productType);
        }

        [HttpPost("[action]")]
        public IActionResult Remove([FromBody]Crud<TransactionType> payload)
        {
            TransactionType productType = _context.TransactionType
                .Where(x => x.TransactionTypeId == (int)payload.key)
                .FirstOrDefault();
            _context.TransactionType.Remove(productType);
            _context.SaveChanges();
            return Ok(productType);

        }
    }
}