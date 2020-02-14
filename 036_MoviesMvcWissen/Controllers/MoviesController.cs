using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using _036_MoviesMvcWissen.Models;
using _036_MoviesMvcWissen.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace _036_MoviesMvcWissen.Controllers
{
    public class MoviesController : Controller //Amacı veri tabanından filmleri çekip göstermek
    {
        MoviesContext db = new MoviesContext();

        // GET: Movies
        public ViewResult Index(MoviesIndexViewModel moviesIndexViewModel) //index aksiyonu verileri listelemek için kullanılır genelde
        {
            //var model = db.Movies.ToList();
            var model = GetList();
            ViewData["count"] = model.Count;

            var years = new List<SelectListItem>();
            years.Add(new SelectListItem()
            {
                Value ="",
                Text = "--All--"
            });

            for (int i = DateTime.Now.Year; i > 1950; i--)
            {
                years.Add(new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }

            if (moviesIndexViewModel == null)
                moviesIndexViewModel = new MoviesIndexViewModel();

            if (String.IsNullOrWhiteSpace(moviesIndexViewModel.YearId))
                moviesIndexViewModel.movies = db.Movies.ToList();
            else
            {
                moviesIndexViewModel.movies = db.Movies.Where(e => e.ProductionYear == moviesIndexViewModel.YearId).ToList();
            }

            //moviesIndexViewModel.movies = db.Movies.ToList();
            moviesIndexViewModel.Years = new SelectList(years ,"Value", "Text" , moviesIndexViewModel.YearId);

            //return View(model); //veri taşıyacaksak bunu view içinde yapmalıyız 
            return View(moviesIndexViewModel);

        }

        [OutputCache(Duration = 60, Location = OutputCacheLocation.ServerAndClient, NoStore = true, VaryByParam ="Name;Min;Max")] //1dk boyunca veriler burada dursun istek geldikçe buradan çeksin.
        public ActionResult List(MoviesIndexViewModel moviesIndexViewModel)
        {
            if (moviesIndexViewModel == null)
            {
                moviesIndexViewModel = new MoviesIndexViewModel();
            }
            var movies = db.Movies.AsQueryable();
            if(!string.IsNullOrWhiteSpace(moviesIndexViewModel.Name))
            {
                movies = movies.Where(e => e.Name.Contains(moviesIndexViewModel.Name));

            }

            moviesIndexViewModel.movies = movies.ToList();
            var years = new List<SelectListItem>();
            for (int i = DateTime.Now.Year; i > 1950; i--)
            {
                years.Add(new SelectListItem()
                {
                    Value = i.ToString(),
                    Text = i.ToString()
                });
            }
            if (!string.IsNullOrWhiteSpace(moviesIndexViewModel.YearId))
            {
                movies = movies.Where(e => e.ProductionYear == moviesIndexViewModel.YearId);
            }



            if (!string.IsNullOrWhiteSpace(moviesIndexViewModel.Min))
            {
                double minValue = 0;
                if(double.TryParse(moviesIndexViewModel.Min,out minValue))
                  movies = movies.Where(e => e.BoxOfficeReturn > minValue);
            }
            if (!string.IsNullOrWhiteSpace(moviesIndexViewModel.Min))
            {
                double maxValue = 0;
                if (double.TryParse(moviesIndexViewModel.Max, out maxValue))
                    movies = movies.Where(e => e.BoxOfficeReturn <= maxValue);
            }
            
            
            moviesIndexViewModel.movies = movies.ToList();
            moviesIndexViewModel.Years = new SelectList(years, "Value", "Text", moviesIndexViewModel.YearId);
            return View(moviesIndexViewModel);
        }

        public List<Movie> GetList(bool removeSession = true)
        {
            List<Movie> entities;
            if (Session["movies"] == null || removeSession)
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

        [HttpGet] // get tarayıca yazdıklarını gösteriyor güvenlik açığı ama postta öyle değil.
        public ActionResult Add() //form u almak için //get yapan add bu
        {
            ViewBag.Message = "Please enter movie information..";
            var directors = db.Directors.ToList().Select(e => new SelectListItem()
            {
                Value = e.Id.ToString(),
                Text = e.Name + " " + e.Surname
            }).ToList();
            ViewData["directors"] = new MultiSelectList(directors, "Value", "Text"); //tuttuğum değer value yani ıd göstereceğim değer text. 
            return View();
        }

        [HttpPost]
        public RedirectToRouteResult Add(string Name, string ProductionYear, string BoxOfficeReturn, List<int> Directors) //form u kayıt ediyor
        {
            var entity = new Movie()
            {
                Id = 0,
                Name = Name,
                ProductionYear = ProductionYear,
                BoxOfficeReturn = Convert.ToDouble(BoxOfficeReturn.Replace(",", "."), CultureInfo.InvariantCulture),
            };
            entity.MovieDirectors = Directors.Select(e => new MovieDirector()
            {
                MovieId = entity.Id,
                DirectorId = e
            }).ToList();

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
            ViewBag.Years = new SelectList(years, "Value", "Text", model.ProductionYear);
            var directors = db.Directors.Select(e => new SelectListItem()
            {
                Value = e.Id.ToString(),
                Text = e.Name + " " + e.Surname
            }).ToList();
            var directorIds = model.MovieDirectors.Select(e => e.DirectorId).ToList();
            ViewData["directors"] = new MultiSelectList(directors, "Value", "Text", directorIds);
            return View(model);
        }

        [HttpPost]

        public ActionResult Edit([Bind(Include = "Id, Name, ProductionYear")] Movie movie, string BoxOfficeReturn, List<int> directorIds)
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
            entity.MovieDirectors = new List<MovieDirector>(); //yeni liste 
            var movieDirectors = db.MovieDirectors.Where(e => e.MovieId == movie.Id).ToList();
            foreach (var movieDirector in movieDirectors)
            {
                //db.Entry(movieDirector).State = EntityState.Deleted; //önce eski kayıtları silmek gerekiyor.
                db.MovieDirectors.Remove(movieDirector);
            }
            foreach (var directorId in directorIds)
            {
                var movieDirector = new MovieDirector()
                {
                    MovieId = movie.Id,
                    DirectorId = directorId
                };
                entity.MovieDirectors.Add(movieDirector);
            }
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            TempData["Info"] = "Record successfully updated to database";
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
            TempData["Info"] = "Record successfully deleted from database";
            return RedirectToAction("Index");
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id is required!");
            var model = db.Movies.Find(id.Value);
            return View(model);

        }

        public ActionResult Welcome()
        {
            var result = "Welcome to Movies MVC";
            // return Content(result);
            return PartialView("_Welcome", result);

        }
    }
}