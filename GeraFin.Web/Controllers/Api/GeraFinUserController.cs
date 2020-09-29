using System;
using SendGrid;
using System.Linq;
using SendGrid.Helpers.Mail;
using GeraFin.DAL.DataAccess;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using GeraFin.Models.ViewModels.Dashboard;
using GeraFin.Models.ViewModels.GeraFinWeb;
using Microsoft.AspNetCore.Http;

namespace GeraFin.Controllers.Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("[Controller]/[Action]")]

    public class GeraFinUserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GeraFinUserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            ConString = configuration.GetConnectionString("DevEnvR");
            SendGridKey = configuration.GetValue<string>("SendGridOptions:SendGridKey");
        }

        public string ConString = "";
        public string SendGridKey = "";
        public static string ShowBlock = "";
        public static string LoadModel = "false";
        public static string BranchName = "";
        public static string TransQTY = "";
        public static string TransferItem = "";
        

        [HttpGet]
        public IActionResult Index()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));
            Stock stock = _context.Stock.Where(x => x.BranchId == BranchId && x.Notify == 1 && x.TransferToBranchId == BranchId).FirstOrDefault();

            if(stock !=null)
            {
                LoadModel = "true";
                BranchName = _context.Branch.Where(x => x.BranchId == stock.TransferFromBranchId).Select(x => x.BranchName).FirstOrDefault();
                TransferItem = stock.ProductName;
                TransQTY = stock.Quantity.ToString();
            }
            else
            {
                LoadModel = "false";
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePopUpData()
        {
            int BranchId = Convert.ToInt32(HttpContext.Session.GetInt32("_BranchId"));

            if (BranchId != 0)
            {
                Stock stock = _context.Stock.Where(x => x.BranchId == BranchId && x.Notify == 1).FirstOrDefault();

                if (stock !=null)
                {
                    stock.Notify = 0;
                    _context.Update(stock);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index", "GeraFinUser");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult NewToDo(UserTasks userTasks)
        {
            userTasks.DueDate = DateTime.Now.AddDays(3);

            _context.UserTasks.Add(userTasks);
            _context.SaveChanges();

            return RedirectToAction("Index", "GeraFinUser");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult CompleteToDo(UserTasks userTasks)
        {
            UserTasks iuserTasks = _context.UserTasks.Where(x => x.TaskId == userTasks.TaskId).FirstOrDefault();
            iuserTasks.MarkCompleted = true;
            iuserTasks.CompletionDate = DateTime.Now;

            _context.UserTasks.Update(iuserTasks);
            _context.SaveChanges();

            return RedirectToAction("Index", "GeraFinUser");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickEmail(QuickEmail Mail)
        {
            try
            {
                SendMail QuickEmail = new SendMail();

                Response Res = await QuickEmail.Execute(SendGridKey, Mail.From, Mail.EmailTo, Mail.Subject, Mail.Body, Mail.Body);

                if (Res.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    ShowBlock = "Sucsess";
                }
                _context.QuickEmail.Add(Mail); 
                _context.SaveChanges();
            }
            catch(Exception ex)
            {
                ShowBlock = "Failure";
                //Loggers.Logging LoginInternal = new Loggers.Logging();
                //LoginInternal.RequestLog(1, ex.Source, "await QuickEmail.Execute(SendGridKey, Mail.From, Mail.EmailTo, Mail.Subject, Mail.Body, Mail.Body);");
            }
            return RedirectToAction("Index", "GeraFinUser");
        }
    }
    
    internal class SendMail
    {
        public async Task<Response> Execute(string SendGridKey, string From, string To, string subject, string plainTextContent, string htmlContent)
        {
            var apiKey = SendGridKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(From);
            var to = new EmailAddress(To);
            htmlContent = "<strong>" + htmlContent + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            Response Res = await client.SendEmailAsync(msg);

            return Res;
        }
    }
}
