using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using CompleteLogin.Models;
using WebMatrix.WebData;

namespace CompleteLogin.Controllers
{
    public class HomeController : Controller
    {
        dbemployeeEntities obj = new dbemployeeEntities();
        [AllowAnonymous]
        public ActionResult Login()
        {
           
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(tblogin login)
        {
            var result = obj.tblogins.Where(m => m.Email == login.Email && m.Password == login.Password);
            if(result!=null)
            {
                ViewBag.msg = "<h1>successfully login</h1>";
                return View("Index");
            }
            else
            {
                ViewBag.msg = "<h1>login failed</h1>";
                return View();
            }
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(tblogin Register)
        {
            obj.tblogins.Add(Register);
            obj.SaveChanges();
            ViewBag.msg = "<h1>Registration Successfull</h1>";
            return View("Login");
        }
        [AllowAnonymous]
        public ActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgetPassword(String Email)
        {
            string To = Email;
            var user=obj.tblogins.Where(m => m.Email == Email);
                if (user == null)
                {
                    ViewBag.msg = "Email Does Not Exist";
                    return View();
                }
                else
                {
                int index = Email.IndexOf("@");
                if (index > 0)
                    Email = Encode(Email);
                var Time = Encode(DateTime.Now.ToString());
                var lnkHref = "<a href='" + Url.Action("ResetPassword", "Home", new { email = Email,time=Time}, "https") + "'>Reset Password</a>";
                    string subject = "Your changed password";
                    string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref;
                MailMessage mail = new MailMessage("anujkmr25299@gmail.com", To, subject, body);
                SmtpClient smtp = new SmtpClient();
                smtp.Send(mail);
                ViewBag.msg = "Email Has Been Sent";
                return View();
                }
        }
        [AllowAnonymous]
        public ActionResult ResetPassword(string Email,String time)
        {
            var start = DateTime.Now;
            var oldDate = DateTime.Parse(Decode(time));

            if (start.Subtract(oldDate) >= TimeSpan.FromMinutes(30))
            {
                ViewBag.msg = "Link Expired! Start Password Recovery Process again";
                Email = "";
                return View(RedirectToAction("Login"));
            }
            String N= Decode(Email);
            Response.Write(N);
            Email = N;
            ViewBag.Email = Email;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public  ActionResult ResetPassword(tblogin login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }
         var id = obj.tblogins.Where(m=>m.Email==login.Email);
            if (id == null)
            {
                // Don't reveal that the user does not exist +=0987td 
                ViewBag.msg = "User Does Not Exist";
                return View("Login");
            }
            tblogin loginid = obj.tblogins.First(f => f.Email == login.Email);
            loginid.Password = login.Password;
            obj.SaveChanges();
            ViewBag.msg = "Your Password Has Been Reset";
            return View("Login");
        }
        public string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }

        public static string Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }
    }
}