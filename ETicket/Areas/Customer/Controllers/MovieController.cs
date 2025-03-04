using ETicket.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace ETicket.Areas.Customer.Controllers
{
    [Area("Customer")]
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
        public IActionResult Details(int MovieId)
        {
            var movie = movieRepository.GetOne(e => e.Id == MovieId,includes: [m => m.Category, m => m.Cinema]);

            var actors = actorRepository.Get(e => e.ActorMovies.Any(am => am.MovieId == MovieId),includes: [a => a.ActorMovies]).ToList();

            ViewData["actors"] = actors;

            ViewBag.RelatedMovies = movieRepository.Get(includes: [m => m.Actors, m => m.Category, m => m.Cinema])
            .Where(m => m.CategoryId == movie.CategoryId && m.Id != movie.Id)
            .GroupBy(m => m.Id)
            .Select(g => g.First())
            .Take(3) 
            .ToList();



            return View(movie);
        }

        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index"); 
            }

            var movies = movieRepository.Get(
                m => m.Name.ToLower().Contains(query.ToLower()),
                includes: [m => m.Cinema, m => m.Category] 
            ).ToList();

            ViewData["SearchQuery"] = query; 
            return View(movies);
        }





    }
}
