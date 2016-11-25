using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CollegeConnected.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CollegeConnected.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly CollegeConnectedDbContext db = new CollegeConnectedDbContext();


        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
            ;
        }
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = db.Users.Find(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Username or Password incorrect");
                return View();
            }
            var email = user.UserID;
            var bytes = Encoding.UTF8.GetBytes(model.Password);

            var sha = new SHA256Managed();
            var hashBytes = sha.ComputeHash(bytes);

            var hash = Convert.ToBase64String(hashBytes);

            if (hash == user.Password)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Name,
                        ClaimTypes.Email, user.UserID)
                };


                var id = new ClaimsIdentity(claims, "ApplicationCookie");

                AuthenticationManager.SignIn(id);
                return RedirectToAction("Admin", "Home");
            }
            ModelState.AddModelError("", "Username or Password incorrect");
            return View();
        }

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login");
        }


        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }
    }
}