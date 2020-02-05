using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _036_MoviesMvcWissen.Controllers
{
    public class MoviesController : Controller //Amacı veri tabanından filmleri çekip göstermek
    {
        MoviesContext db = new MoviesContext();

        // GET: Movies
        public ViewResult Index() //index aksiyonu verileri listelemek için kullanılır genelde
        {
            var model = db.Movies.ToList();
            return View(model); //veri taşıyacaksak bunu view içinde yapmalıyız 
        }
        [HttpGet]
        public ActionResult Add() //form u almak için
        {
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult Add(string Name, string ProductionYear, string BoxOfficeReturn) //form u kayıt ediyor
        {
            var entity = new Movie()
            {
                Name = Name,
                ProductionYear = ProductionYear,
                BoxOfficeReturn = Convert.ToDouble(BoxOfficeReturn.Replace(",","."), CultureInfo.InvariantCulture)
            };
            db.Movies.Add(entity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}