using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GeraFin.DAL.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using GeraFin.InterFaces.Factory;
using GeraFin.Models.SessionState;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using GeraFin.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using GeraFin.Models.ViewModels.GeraFinWeb;

namespace GeraFin.Controllers
{
    [AllowAnonymous]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        public int iBranchId;
        public int Admin;
        public const string SessionIsAdmin = "_IsAdmin";
        public const string SessionLoginKey = "_Name";
        public const string SessionBranchKey = "_BranchId";

        public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _context = context;
        }

        public IConfiguration Configuration { get; }

        [TempData] public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignedOut()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            UpdateSession(HttpContext.Session.Id);
            await HttpContext.SignOutAsync();
            _logger.LogInformation("User logged out.");
            
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    ApplicationUser byEmailAsync = await _userManager.FindByEmailAsync(model.Email);
                    if (byEmailAsync != null)
                    {
                        IdentityResult identityResult = await _userManager.UpdateSecurityStampAsync(byEmailAsync);
                    }

                    Admin = Convert.ToInt32((from adm in _context.UserProfile.Where(x => x.Email == model.Email) select adm.IsAdmin).FirstOrDefault());
                    iBranchId = _context.Branch.Where(x => x.RegionalEmail == model.Email).Select(x => x.BranchId).FirstOrDefault();

                    if (Admin == 1)
                    {
                        returnUrl = "/MyBranches/Index";
                    }
                    else if (Admin == 0)
                    {
                        returnUrl = "/GeraFinUser/Index";
                    }

                    if (Admin == 1 && iBranchId !=0)
                    {
                        HttpContext.Session.SetInt32("_BranchId", iBranchId);
                        HttpContext.Session.SetInt32("_IsAdmin", Admin);
                        HttpContext.Session.SetString("_Name", model.Email);
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("_BranchId", _context.Branch.Where(x => x.Email == model.Email).Select(x => x.BranchId).FirstOrDefault());
                        HttpContext.Session.SetInt32("_IsAdmin", Admin);
                        HttpContext.Session.SetString("_Name", model.Email);
                    }

                    LogSession(HttpContext.Session.Id, HttpContext.Session.GetString("_Name"), Convert.ToInt32(HttpContext.Session.GetInt32("_IsAdmin")), Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId")), true, Environment.MachineName, Environment.OSVersion.ToString());
                    
                    _logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public IActionResult ExternalLogin(string provider, string returnUrl = null, string AccountName = null)
        //{
        //    provider = "Google";
        //    returnUrl = "/GeraFinUser/Index";

        //    HttpContext.Session.SetString("Account", AccountName);

        //    new ChallengeResult(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
        //    {
        //        RedirectUri = Url.Action(nameof(ExternalLoginCallback), new { returnUrl })
        //    });

        //    var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

        //    return Challenge(properties, provider);
        //}

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null, string expectedXsrf = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync(_userManager.GetUserId(User));

            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }
            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, 
                info.ProviderKey, 
                isPersistent: false, 
                bypassTwoFactor: true);


            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            info = await _signInManager.GetExternalLoginInfoAsync(expectedXsrf);
            
            
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                ClaimsIdentity principalIdentity = info.Principal.Identity as ClaimsIdentity;
                string preferredUsername = principalIdentity.FindFirst("preferred_username").Value;
                ViewData["PreferredUsername"] = preferredUsername;
                return View("ExternalLogin", new ExternalLogin
                {
                    Email = preferredUsername
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLogin model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                if (await LoginIfExternalProviderAlreadyAssignedAsync(model.Email))
                {
                    return RedirectToLocal(returnUrl);
                }
                
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                  
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);

                await _emailSender.SendEmailAsync(model.Email, "Reset Password", $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPassword { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private void LogSession(string SessionId, string vcSessionLoginKey, int iSessionIsAdmin, int iSessionBranchKey, bool Active, string vcMachineName, string vcOSVersion)
        {
            iUserSession entity = new iUserSession();
            entity.SessionId = SessionId;
            entity.vcSessionLoginKey = vcSessionLoginKey;
            entity.iSessionIsAdmin = iSessionIsAdmin;
            entity.iSessionBranchKey = iSessionBranchKey;
            entity.Active = Active;
            entity.dtSessionStart = DateTime.Now.ToString();
            entity.dtSessionEnd = string.Empty;
            entity.vcMachineName = vcMachineName;
            entity.vcOSVersion = vcOSVersion;

            _context.iUserSession.Add(entity);
            _context.SaveChanges();
        }

        public void UpdateSession(string SessionID)
        {
            iUserSession entity = _context.iUserSession.Where(x => x.SessionId == SessionID && x.Active == true).FirstOrDefault();
            if (entity == null)
                return;
            entity.Active = false;
            entity.dtSessionEnd = DateTime.Now.ToString();
            _context.iUserSession.Update(entity);
            _context.SaveChanges();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private async Task<bool> LoginIfExternalProviderAlreadyAssignedAsync(string email = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return false;
            }

            email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return false;
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return true;
            }
            return false;
        }
    }
}
