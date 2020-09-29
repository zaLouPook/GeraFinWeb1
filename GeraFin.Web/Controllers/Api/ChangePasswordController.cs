using System.Linq;
using GeraFin.DAL.DataAccess;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using GeraFin.Models.ViewModels.Manage;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Syncfusion;
using GeraFin.Models.ViewModels.GeraFinWeb;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/ChangePassword")]
    public class ChangePasswordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ChangePasswordController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public RoleManager<IdentityRole> RoleManager => _roleManager;

        // GET: api/ChangePassword
        [HttpGet]
        public IActionResult GetChangePassword()
        {
            _ = new List<ApplicationUser>();
            List<ApplicationUser> Items = _context.Users.ToList();
            int Count = Items.Count();
            return Ok(new { Items, Count });
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Update([FromBody]Crud<ChangePassword> payload)
        {
            ChangePassword changePasswordViewModel = payload.value;
            var user = _context.Users.SingleOrDefault(x => x.Id.Equals(changePasswordViewModel.Id));

            if (user != null && changePasswordViewModel.NewPassword.Equals(changePasswordViewModel.ConfirmPassword))
            {
                await _userManager.ChangePasswordAsync(user, changePasswordViewModel.OldPassword, changePasswordViewModel.NewPassword);
            }
            return Ok();
        }

    }
}