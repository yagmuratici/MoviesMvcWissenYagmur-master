﻿using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace _036_MoviesMvcWissen.Controllers
{
    public class LoginController : Controller
    {
        MoviesContext db = new MoviesContext();

        // GET: Login
        public ActionResult Index()
        {
            User user = new User();
            return View(user);
        }

        [HttpPost]

        public ActionResult Index(User user , string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (db.Users.Any(e => e.UserName == user.UserName && e.Password == user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    return returnUrl == null ? RedirectToAction("Index", "Movies") : (ActionResult)Redirect(returnUrl);
                }
                ViewBag.Message = "User Name or Password is incorrect!";
                return View(user); //user ı gönderiyoruz ki adamın girdiği şeyler gelsin
            }
            ViewBag.Message = "User Name or Password is invalid!";
            return View(user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Movies");
        }
    }
}