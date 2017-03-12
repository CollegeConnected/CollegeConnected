﻿using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CollegeConnected.Models;
using Microsoft.Owin.Security;

namespace CollegeConnected.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly CollegeConnectedDbContext db = new CollegeConnectedDbContext();


        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }


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
                return View(model);
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
                FormsAuthentication.SetAuthCookie(user.UserID, false);
                return RedirectToAction("Admin", "Home");
            }
            ModelState.AddModelError("", "Username or Password incorrect");
            return View();
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Response.Cookies.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}