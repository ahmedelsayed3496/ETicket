using ETicket.Data.Enums;
using ETicket.Models;
using ETicket.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace ETicket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository movieRepository;
        private readonly ICinemaRepository cinemaRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IActorRepository actorRepository;

        public MovieController(IMovieRepository movieRepository, ICinemaRepository cinemaRepository, ICategoryRepository categoryRepository, IActorRepository actorRepository)
        {
            this.movieRepository = movieRepository;
            this.cinemaRepository = cinemaRepository;
            this.categoryRepository = categoryRepository;
            this.actorRepository = actorRepository;
        }
        public IActionResult Index()
        {
            var movies = movieRepository.Get(includes: [m => m.Category, m => m.Cinema]);

            return View(movies.ToList());
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Cinemas = cinemaRepository.Get().ToList();
            ViewBag.Categories = categoryRepository.Get().ToList();
            ViewBag.Actors = actorRepository.Get().ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(Movie movie, int[] ActorIds, IFormFile? ImageFile, IFormFile? TrailerFile)
        {
            var today = DateTime.Now;
            if (movie.StartDate > today)
                movie.MovieStatus = ETicket.Data.Enums.MovieStatus.Upcoming;
            else if (movie.EndDate < today)
                movie.MovieStatus = ETicket.Data.Enums.MovieStatus.Expired;
            else
                movie.MovieStatus = ETicket.Data.Enums.MovieStatus.Available;

            if (ImageFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images/movies", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create)) { ImageFile.CopyTo(stream); }
                movie.ImgUrl = fileName;
            }

            if (TrailerFile != null)
            {
                var trailerName = Guid.NewGuid() + Path.GetExtension(TrailerFile.FileName);
                var trailerPath = Path.Combine("wwwroot/videos", trailerName);
                using (var stream = new FileStream(trailerPath, FileMode.Create)) { TrailerFile.CopyTo(stream); }
                movie.TrailerUrl = trailerName;
            }

            
            movie.ActorMovies = ActorIds.Select(id => new ActorMovie { ActorId = id }).ToList();

            
            movieRepository.Create(movie);
            movieRepository.Commit();

           
            TempData["SuccessMessage"] = $"Movie '{movie.Name}' created successfully!";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var movie = movieRepository.GetOne(m => m.Id == id, includes: [m => m.Category,m => m.Cinema, m => m.ActorMovies]);

            if (movie == null)
            {
                return NotFound();
            }

            ViewBag.Cinemas = cinemaRepository.Get().ToList();
            ViewBag.Categories = categoryRepository.Get().ToList();
            ViewBag.Actors = actorRepository.Get().ToList();

            
            ViewBag.SelectedActors = movie.ActorMovies.Select(am => am.ActorId).ToList();

            return View(movie);
        }

        [HttpPost]
        public IActionResult Edit(Movie movie, IFormFile? ImageFile, IFormFile? TrailerFile, int[] ActorIds)
        {
            var movieInDb = movieRepository.GetOne(m => m.Id == movie.Id, includes:[m => m.ActorMovies]);

            if (movieInDb == null)
            {
                return NotFound();
            }


            movieInDb.Name = movie.Name;
            movieInDb.Description = movie.Description;
            movieInDb.Price = movie.Price;
            movieInDb.StartDate = movie.StartDate;
            movieInDb.EndDate = movie.EndDate;
            movieInDb.CinemaId = movie.CinemaId;
            movieInDb.CategoryId = movie.CategoryId;


            var today = DateTime.Now;
            if (movie.StartDate > today)
                movieInDb.MovieStatus = ETicket.Data.Enums.MovieStatus.Upcoming;
            else if (movie.EndDate < today)
                movieInDb.MovieStatus = ETicket.Data.Enums.MovieStatus.Expired;
            else
                movieInDb.MovieStatus = ETicket.Data.Enums.MovieStatus.Available;


            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine("wwwroot/images/movies", fileName);
                using (var stream = System.IO.File.Create(filePath)) { ImageFile.CopyTo(stream); }


                var oldImagePath = Path.Combine("wwwroot/images/movies", movieInDb.ImgUrl);
                if (System.IO.File.Exists(oldImagePath)) { System.IO.File.Delete(oldImagePath); }

                movieInDb.ImgUrl = fileName;
            }

            if (TrailerFile != null && TrailerFile.Length > 0)
            {
                var trailerName = Guid.NewGuid() + Path.GetExtension(TrailerFile.FileName);
                var trailerPath = Path.Combine("wwwroot/videos", trailerName);
                using (var stream = System.IO.File.Create(trailerPath)) { TrailerFile.CopyTo(stream); }

                var oldTrailerPath = Path.Combine("wwwroot/videos", movieInDb.TrailerUrl);
                if (System.IO.File.Exists(oldTrailerPath)) { System.IO.File.Delete(oldTrailerPath); }

                movieInDb.TrailerUrl = trailerName;
            }


            var existingActorIds = movieInDb.ActorMovies.Select(am => am.ActorId).ToList();


            foreach (var actor in movieInDb.ActorMovies.ToList())
            {
                if (!ActorIds.Contains(actor.ActorId))
                {
                    movieInDb.ActorMovies.Remove(actor);
                }
            }


            foreach (var actorId in ActorIds)
            {
                if (!existingActorIds.Contains(actorId))
                {
                    movieInDb.ActorMovies.Add(new ActorMovie { ActorId = actorId, MovieId = movieInDb.Id });
                }
            }


            movieRepository.Edit(movieInDb);
            movieRepository.Commit();

            TempData["SuccessMessage"] = $"Movie '{movie.Name}' updated successfully!";

            return RedirectToAction("Index");
        }


        public IActionResult Delete(int movieId)
        {
            var movie = movieRepository.GetOne(m => m.Id == movieId);

            if (movie == null)
            {
                return NotFound(); 
            }

           
            if (!string.IsNullOrEmpty(movie.ImgUrl))
            {
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies", movie.ImgUrl);
                if (System.IO.File.Exists(imgPath))
                {
                    System.IO.File.Delete(imgPath);
                }
            }

           
            if (!string.IsNullOrEmpty(movie.TrailerUrl))
            {
                var trailerPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/videos", movie.TrailerUrl);
                if (System.IO.File.Exists(trailerPath))
                {
                    System.IO.File.Delete(trailerPath);
                }
            }

           
            movieRepository.Delete(movie);
            movieRepository.Commit();

            
            TempData["SuccessMessage"] = $"Movie '{movie.Name}' deleted successfully!";

            return RedirectToAction("Index");
        }




    }
}
