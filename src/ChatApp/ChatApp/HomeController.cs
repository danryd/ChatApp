using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ChatApp
{
    [System.Web.Mvc.Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(Settings.CookieName);
            return RedirectToAction("Login");
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(string username)
        {

            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, username), new Claim(ClaimTypes.Name, username) }, Settings.CookieName);
            HttpContext.GetOwinContext().Authentication.SignIn(identity);
            return RedirectToAction("Index");
        }
    }
}