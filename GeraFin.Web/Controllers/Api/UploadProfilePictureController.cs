using GeraFin.DAL.DataAccess;
using GeraFin.InterFaces.Factory;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeraFin.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/UploadProfilePicture")]
    [Authorize]
    public class UploadProfilePictureController : Controller
    {
        private readonly IFunctional _functionalService;
        [Obsolete]
        private readonly IHostingEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        [Obsolete]
        public UploadProfilePictureController(IFunctional functionalService, IHostingEnvironment env, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _functionalService = functionalService;
            _env = env;
            _userManager = userManager;
            _context = context;
        }

        [HttpPost]
        [RequestSizeLimit(5000000)]
        [Obsolete]
        public async Task<IActionResult> PostUploadProfilePicture(List<IFormFile> UploadDefault)
        {
            try
            {
                var folderUpload = "upload";
                var fileName = await _functionalService.UploadFile(UploadDefault, _env, folderUpload);

                ApplicationUser appUser = await _userManager.GetUserAsync(User);
                if (appUser != null)
                {
                    UserProfile profile = _context.UserProfile.SingleOrDefault(x => x.ApplicationUserId.Equals(appUser.Id));
                    if (profile != null)
                    {
                        profile.ProfilePicture = "/" + folderUpload + "/" + fileName;
                        _context.UserProfile.Update(profile);
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok(fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}