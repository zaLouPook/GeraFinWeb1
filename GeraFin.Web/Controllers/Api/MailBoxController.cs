//using System;
//using System.Linq;
//using OpenPop.Mime;
//using OpenPop.Pop3;
//using GeraFin.DAL.DataAccess;
//using GeraFin.Models.WebClient;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using GeraFin.Models.ViewModels.Gmail;
//using Microsoft.AspNetCore.Authorization;

//namespace GeraFin.Controllers.Api
//{
//    [Authorize]
//    [Produces("application/json")]
//    [Route("api/MailBox")]
//    public class MailBoxController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public MailBoxController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public int iUserId = 0;
//        public int iBranchId = AccountController.iBanchId;

//        [HttpGet]
//        public IActionResult Inbox()
//        {
//            Pop3Client pop3Client;

//            pop3Client = new Pop3Client();

//            iUserId = _context.UserProfile.Where(x => x.Email == User.Identity.Name).Select(x => x.UserProfileId).SingleOrDefault();
//            var PEmails = _context.UserConfig.Where(x => x.UserId == iUserId).FirstOrDefault();

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
//                    email.Attachments.Add(new Models.ViewModels.Gmail.Attachment
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

//        [HttpPost]
//        public ActionResult Add(string Email, string Password, int iUserId)
//        {
//            if (String.IsNullOrEmpty(Email) || String.IsNullOrEmpty(Password) || iUserId == 0)
//            {
//                return View();
//            }
//            //Save SMTP
//            UserConfig SMTP = new UserConfig
//            {
//                UserId = iUserId,
//                Branchid = iBranchId,
//                Email = Email,
//                Password = Password,
//                SMTPHostServer = "smtp.gmail,.com",
//                POP3HostServer = "pop3.gmail.com",
//                SMTPPort = 587,
//                POP3Port = 995,
//                SMTPEnableSSL = true,
//                POP3EnableSSL = true
//            };

//            _context.Add(SMTP);
//            _context.SaveChanges();

//            return RedirectToAction("Inbox");
//        }

//        public Pop3Client PopClientConnect(Pop3Client pop3Client)
//        {
//            try
//            {
//                pop3Client = new Pop3Client();

//                iUserId = _context.UserProfile.Where(x => x.Email == User.Identity.Name).Select(x => x.UserProfileId).SingleOrDefault();
//                var PEmails = _context.UserConfig.Where(x => x.UserId == iUserId).FirstOrDefault();

//                pop3Client.Connect(PEmails.POP3HostServer, PEmails.POP3Port, PEmails.POP3EnableSSL);
//                pop3Client.Authenticate(PEmails.Email, PEmails.Password);

//                //pop3Client.Connect(PEmails.POP3HostServer, PEmails.POP3Port, PEmails.POP3EnableSSL);
//                //pop3Client.Authenticate(PEmails.Email, PEmails.Password);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//            return pop3Client;
//        }

//        //[HttpPost]
//        //public ActionResult SendEmail(Models.ViewModels.Gmail.MailMessage msg, System.Net.Mail.Attachment attachment)
//        //{
//        //    iUserId = _context.UserProfile.Where(x => x.Email == User.Identity.Name).Select(x => x.UserProfileId).SingleOrDefault();
//        //    var PEmails = _context.UserConfig.Where(x => x.UserId == iUserId).FirstOrDefault();

//        //    if(PEmails != null)
//        //    {
//        //        SmtpClient client = new SmtpClient(PEmails.SMTPHostServer);
//        //        client.UseDefaultCredentials = false;
//        //        client.Credentials = new NetworkCredential(PEmails.Email, PEmails.Password);

//        //        System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();

//        //        mailMessage.From = new MailAddress(msg.FromEmail);
//        //        mailMessage.To.Add(msg.ToEmail);
//        //        mailMessage.Body = msg.EmailBody;
//        //        mailMessage.Subject = msg.EmailSubject;
//        //        mailMessage.IsBodyHtml = true;

//        //        if (attachment != null)
//        //        {
//        //            mailMessage.Attachments.Add(attachment);
//        //        }
//        //        client.Send(mailMessage);

//        //        return View();
//        //    }
//        //    else
//        //    {
//        //        return View();
//        //    }
//        //}
//    }
//}
    