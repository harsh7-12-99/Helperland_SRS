using Helperland.Data;
using Helperland.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Helperland.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HelperlandContext _auc;

        public AccountController(ILogger<HomeController> logger, HelperlandContext auc)
        {
            _logger = logger;
            _auc = auc;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateForCustomer(User uc)
        {
            var verify = _auc.Users.FirstOrDefault(x => x.Email.Equals(uc.Email));
            if (verify == null)
            {
                uc.UserTypeId = 0;
                _auc.Add(uc);
                _auc.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            else
            {
                string errormsg = "Email is already registered...login instead !";
                TempData["ErrorMessage"] = errormsg;
                return RedirectToAction("NewCustomerAccount", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateForProvider(User uc)
        {
            var verify = _auc.Users.FirstOrDefault(x => x.Email.Equals(uc.Email));
            if (verify == null)
            {
                uc.UserTypeId = 1;
                _auc.Add(uc);
                _auc.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                string errormsg = "Email is already registered...login instead !";
                TempData["ErrorMessage"] = errormsg ;
                return RedirectToAction("BecomeHelper", "Home");
            }
        }

        [HttpPost]
        public IActionResult Login (User uc)
        {
            var verify = _auc.Users.FirstOrDefault(x => x.Email.Equals(uc.Email) && x.Password.Equals(uc.Password));
            if (verify != null)
            {
                if (verify.UserTypeId == 0)
                {
                    HttpContext.Session.SetString("Username", verify.FirstName);
                    return RedirectToAction("Index","Home");
                }
                else if (verify.UserTypeId == 1)
                {
                    if (verify.IsApproved == true)
                    {
                        HttpContext.Session.SetString("Username", verify.FirstName);
                        return RedirectToAction("Index","Home");
                    }
                    else
                    {
                        return RedirectToAction("Index","Home");
                    }
                }
                else
                {
                    HttpContext.Session.SetString("username", verify.FirstName);
                    return RedirectToAction("Index","Home");
                }
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPasswordResetlink(User uc)
        {

                var verify = (from x in _auc.Users where x.Email == uc.Email select x).FirstOrDefault();
            
                String To = uc.Email;
                var ResetLink = "<a href='" + Url.Action("NewPassword","Account",null,"http") + "'> click here to reset your password </a>";
                String subject = "Helperland - Reset your password ";
                String Body = "Reset password" + " : " + ResetLink;
                MailMessage obj = new MailMessage();
                obj.To.Add(To);
                obj.Subject = subject;
                obj.Body = Body;
                obj.From = new MailAddress("harshitrajani1988@gmail.com");
                obj.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential("harshitrajani1988@gmail.com", "xxxx");
                smtp.Send(obj);
                ViewBag.message = "The email has been sent";
                return RedirectToAction("Index","Home");
          

        }
        [HttpGet]
        public ActionResult NewPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewPassword(User uc)
        {
            var verify = _auc.Users.FirstOrDefault(x => x.Email.Equals(uc.Email));
            if (verify != null)
            {
                verify.Email = uc.Email;
                verify.Password = uc.Password;
                verify.Confirmpassword = uc.Confirmpassword;
                _auc.Users.Update(verify);
                _auc.SaveChanges();
                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid email";
                return RedirectToAction("NewPassword"); 
            }
        }

    }
}
