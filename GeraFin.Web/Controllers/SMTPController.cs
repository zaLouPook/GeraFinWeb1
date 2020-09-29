//using System.Linq;
//using OpenPop.Mime;
//using OpenPop.Pop3;
//using GeraFin.DAL.DataAccess;
//using GeraFin.Models.WebClient;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using GeraFin.Models.ViewModels.Gmail;

//namespace GeraFin.Web.Controllers
//{
//    public class SMTPController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public SMTPController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public int iUserId = 0;
//        public int iBranchId = GeraFin.Controllers.AccountController.iBanchId;

//        // GET: SMTP
//        public ActionResult Index()
//        {
//            iUserId = _context.UserProfile.Where(x => x.Email == User.Identity.Name).Select(x => x.UserProfileId).SingleOrDefault();
//            ViewBag.Categories = _context.UserConfig.Where(x => x.UserId == iUserId);
//            // var model = new Email();
//            return View();
//        }

//        [HttpGet]
//        public ActionResult Add()
//        {
//            return View();
//        }

//        [HttpPost]
//        public ActionResult Add(SMTPemail smtpe)
//        {
//            //Save SMTP
//            UserConfig SMTP = new UserConfig();
//            SMTP.UserId = iUserId;
//            SMTP.Branchid = iBranchId;
//            SMTP.Email = smtpe.Email;
//            SMTP.Password = smtpe.Password;
//            SMTP.SMTPHostServer = smtpe.ServerName;
//            SMTP.POP3HostServer = smtpe.PServerName;
//            SMTP.SMTPPort = smtpe.Port;
//            SMTP.POP3Port = smtpe.PPort;
//            SMTP.SMTPEnableSSL = smtpe.SSL;
//            SMTP.POP3EnableSSL = smtpe.PSSL;

//            _context.Add(SMTP);
//            _context.SaveChanges();

//            return RedirectToAction("Index", "Dashboards");
//        }

//        public ActionResult ReadMail()
//        {
//            var PEmails = _context.UserConfig.Where(x => x.UserId == iUserId).FirstOrDefault();

//            Pop3Client pop3Client;
//            pop3Client = new Pop3Client();
            
//            pop3Client.Connect(PEmails.POP3HostServer, PEmails.POP3Port, PEmails.POP3EnableSSL);
//            pop3Client.Authenticate(PEmails.Email, PEmails.Password);
            
//            pop3Client.Connect(PEmails.POP3HostServer, PEmails.POP3Port, PEmails.POP3EnableSSL);
//            pop3Client.Authenticate(PEmails.Email, PEmails.Password);

//            int count = pop3Client.GetMessageCount();
//            var Emails = new List<POPEmail>();
//            int counter = 0;

//            for (int i = count; i >= 1; i--)
//            {
//                Message message = pop3Client.GetMessage(i);
//                POPEmail email = new POPEmail()
//                {
//                    MessageNumber = i,
//                    Subject = message.Headers.Subject,
//                    DateSent = message.Headers.DateSent,
//                    From = string.Format("<a href = 'mailto:{1}'>{0}</a>", message.Headers.From.DisplayName, message.Headers.From.Address),
//                };

//                MessagePart body = message.FindFirstHtmlVersion();
//                if (body != null)
//                {
//                    email.Body = body.GetBodyAsText();
//                }
//                else
//                {
//                    body = message.FindFirstPlainTextVersion();
//                    if (body != null)
//                    {
//                        email.Body = body.GetBodyAsText();
//                    }
//                }
                
//                List<MessagePart> attachments = message.FindAllAttachments();

//                foreach (MessagePart attachment in attachments)
//                {
//                    email.Attachments.Add(new Attachment
//                    {
//                        FileName = attachment.FileName,
//                        ContentType = attachment.ContentType.MediaType,
//                        Content = attachment.Body
//                    });
//                }
//                Emails.Add(email);
//                counter++;
//                if (counter > 2)
//                {
//                    break;
//                }
//            }
//            var emails = Emails;
//            return View(emails);
//        }
//    }
//}