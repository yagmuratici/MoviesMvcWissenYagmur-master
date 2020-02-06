using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
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
            //var model = db.Movies.ToList();
            var model = GetList();
            return View(model); //veri taşıyacaksak bunu view içinde yapmalıyız 
        }

        public List<Movie> GetList(bool removeSession = true)
        {
            List<Movie> entities;
            if(Session["movies"] == null || removeSession)
            {
                entities = db.Movies.ToList();
            }
            else
            {
                entities = Session["movies"] as List<Movie>;
            }
            return entities;
        }

        public ActionResult GetMoviesFromSession()
        {
            var model = GetList(false);
            return View("Index", model);
        }

        [HttpGet]
        public ActionResult Add() //form u almak için //get yapan add bu
        {
            ViewBag.Message = "Please enter movie information..";
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
            TempData["Info"] = "Record successfully saved to database";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var model = db.Movies.Find(id.Value);
            List<SelectListItem> years = new List<SelectListItem>();
            SelectListItem year;
            for (int i = DateTime.Now.Year; i >= 1890; i--)
            {
                year = new SelectListItem() { Value = i.ToString(), Text = i.ToString() };
                years.Add(year);
            }
            ViewBag.Years = new SelectList(years, "Value","Text",model.ProductionYear);
            return View(model);
        }

        [HttpPost]

        public ActionResult Edit([Bind(Include = "Id, Name, ProductionYear")] Movie movie, string BoxOfficeReturn)
        {
            var entity = db.Movies.SingleOrDefault(e => e.Id == movie.Id);
            entity.Name = movie.Name;
            entity.ProductionYear = movie.ProductionYear;
            if (BoxOfficeReturn != "")
                entity.BoxOfficeReturn = Convert.ToDouble(BoxOfficeReturn.Replace(",", "."), CultureInfo.InvariantCulture);
            else
            {
                TempData["Info"] = "Adam gibi birşey gir";
                return RedirectToRoute(new { Controller = "Movies", action = "Edit" });
            }
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToRoute(new { Controller = "Movies", action = "Index" });
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!");
            var model = db.Movies.FirstOrDefault(e => e.Id == id.Value);
            return View(model);
        }

        [ActionName("Delete")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var entity = db.Movies.Find(id);
            db.Movies.Remove(entity);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}