using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _036_MoviesMvcWissen.Contexts;
using _036_MoviesMvcWissen.Entities;
using _036_MoviesMvcWissen.Models.ViewModels;

namespace _036_MoviesMvcWissen.Controllers
{
    public class DirectorsController : Controller
    {
        private MoviesContext db = new MoviesContext();

        // GET: Directors
        public ActionResult Index()
        {
            var model = new DirectorIndexViewModel()
            {
                Directors = db.Directors.ToList()
            };
            return View(model);
        }

        // GET: Directors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Director director = db.Directors.Find(id);
            if (director == null)
            {
                return HttpNotFound();
            }
            return View(director);
        }

        // GET: Directors/Create
        public ActionResult Create()
        {
            var movies = db.Movies.ToList().Select(e => new SelectListItem() //directors listesini aldık ve listbox da kullanabilmek için multiselectliste çevirdik
            {
                Value = e.Id.ToString(),
                Text = e.Name
            }).ToList();
            ViewData["movies"] = new MultiSelectList(movies, "Value", "Text");
            return View();
        }

        // POST: Directors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,Name,Surname,Retired")] Director director) //default
        //{
        [ActionName("Create")]
        //public ActionResult CreateNew()//1. yol
        public ActionResult CreateNew(FormCollection formCollection)
        {
            var director = new Director()
            {
                //Name = Request.Form["Name"],//1. yol için kullanıyorum
                //Surname =Request.Form["Surname"]//1. yol için kullanıyorum
                Name = formCollection["Name"],
                Surname = formCollection["Surname"]
            };

            //var retired = (Request.Form["Retired"]);//1. yol için kullanıyorum
            var retired = formCollection["Retired"];
            var movieIds = formCollection["movieIds"].Split(',');
            director.Retired = true;
            if (retired.Equals("false"))
                director.Retired = false;
            if (String.IsNullOrWhiteSpace(director.Name))
                ModelState.AddModelError("Name", "Director Name is Required!");
            if (String.IsNullOrWhiteSpace(director.Surname))
                ModelState.AddModelError("Surname", "Director Surname is Required!");
            if (director.Name.Length > 100)
                ModelState.AddModelError("Name", "Director name must be maximum 100 characters!");
            if (director.Surname.Length > 100)
                ModelState.AddModelError("Surname", "Director surname must be maximum 100 characters!");
            if (ModelState.IsValid)
            {
                director.MovieDirectors = movieIds.Select(e => new MovieDirector()
                {
                    MovieId = Convert.ToInt32(e),
                    DirectorId = director.Id
                }).ToList();
                db.Directors.Add(director);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(director);
        }

        #region Edit Yol :1
        // GET: Directors/Edit/5
        //  public ActionResult Edit(int? id) yol 1
        // { 
        //yol 1:
        //if (id == null)
        //{
        //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //}
        //Director director = db.Directors.Find(id);
        //if (director == null)
        //{
        //    return HttpNotFound();
        //}

        //var movies = db.Movies.Select(e => new SelectListItem()
        //{
        //    Value = e.Id.ToString(),
        //    Text = e.Name
        //}).ToList();

        //var model = db.Directors.Find(id.Value);
        //var movieIds = model.MovieDirectors.Select(e => e.MovieId).ToList();
        //ViewData["movies"] = new MultiSelectList(movies, "Value", "Text", movieIds);
        //return View(director);
        //  }

        #endregion


        public ActionResult Edit(int? id) //yol 2
        {

            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var movies = db.Movies.Select(e => new SelectListItem()
            {
                Value = e.Id.ToString(),
                Text = e.Name
            }).ToList();

            var director = db.Directors.Find(id.Value);
            List<int> _movieIds = director.MovieDirectors.Select(e => e.MovieId).ToList();
            DirectorsEditViewModel model = new DirectorsEditViewModel();
            model.Director = director;
            model.movieIds = _movieIds;
            model.Movies = new MultiSelectList(movies, "Value", "Text", model.movieIds);
            //2.yol
            //DirectorsEditViewModel model = new DirectorsEditViewModel() //model oluşturduk
            //{
            //    Director = director,
            //    Movies = new MultiSelectList(movies, _movieIds), //movieIds i yazılınca seçili olanlar gidecek
            //    //edit olduğu için adamın daha önce seçtiği ıd leride dönmemiz gerek
            //    movieId = _movieIds

            //};
            return View("EditNew", model);
        }

        // POST: Directors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        #region Edit Post Yol 1
        //public ActionResult Edit([Bind(Include = "Id,Name,Surname,Retired")] Director director, List<int> movieIds)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = db.Directors.SingleOrDefault(e => e.Id == director.Id);
        //        entity.Name = director.Name;
        //        entity.Retired = director.Retired;
        //        entity.Surname = director.Surname;

        //        entity.MovieDirectors = new List<MovieDirector>(); //yeni liste 

        //        var movieDirectors = db.MovieDirectors.Where(e => e.DirectorId == director.Id).ToList();
        //        foreach (var movieDirector in movieDirectors)
        //        {
        //            //db.Entry(movieDirector).State = EntityState.Deleted; //önce eski kayıtları silmek gerekiyor.
        //            db.Entry(movieDirector).State = EntityState.Deleted;
        //        }

        //        foreach (var movieId in movieIds)
        //        {
        //            var movieDirector = new MovieDirector()
        //            {
        //                DirectorId = director.Id,
        //                MovieId = movieId
        //            };
        //            entity.MovieDirectors.Add(movieDirector);
        //        }
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //        TempData["Info"] = "Record successfully updated to database";
        //        return RedirectToRoute(new { Controller = "Directors", action = "Index" });
        //    }
        //    return View(director);

        //}
        #endregion

        public ActionResult Edit(DirectorsEditViewModel directorEditViewModel)
        {
            if (ModelState.IsValid)
            {

                var director = db.Directors.Find(directorEditViewModel.Director.Id); //director çekiyoruz db contexten aldık
                director.Name = directorEditViewModel.Director.Name; //güncellemeler için veri tabanından kayıt çekmek.
                director.Surname = directorEditViewModel.Director.Surname;
                director.Retired = directorEditViewModel.Director.Retired;

                //veri tabınında eskileri temizlemek gerek
                //var movieDirectors = db.MovieDirectors.Where(e => e.DirectorId == director.Id).ToList(); //1.yol
                var movieDirectors = director.MovieDirectors;
                foreach (var movieDirector in movieDirectors.ToList())
                {
                    db.MovieDirectors.Remove(movieDirector);
                }
                //var movieDirectors = db.MovieDirectors.Where(e => e.DirectorId == director.Id).ToList(); //1.yol
                //foreach (var movieDirector in movieDirectors)
                //{
                //    db.MovieDirectors.Remove(movieDirector);
                //}

                //sildikten sonra gelen idlerden moviedirector yapıyor
                director.MovieDirectors = directorEditViewModel.movieIds.Select(e => new MovieDirector()
                {
                    DirectorId = director.Id,
                    MovieId = e
                }).ToList();

                db.Entry(director).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(directorEditViewModel);
        }


        // GET: Directors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Director director = db.Directors.Find(id);
            if (director == null)
            {
                return HttpNotFound();
            }
            return View(director);
        }

        // POST: Directors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Director director = db.Directors.Find(id);
            db.Directors.Remove(director);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
