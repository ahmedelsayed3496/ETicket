using ETicket.Models;
using ETicket.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace ETicket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ActorController : Controller
    {

        private readonly IMovieRepository movieRepository;
        private readonly ICinemaRepository cinemaRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IActorRepository actorRepository;

        public ActorController(IMovieRepository movieRepository, ICinemaRepository cinemaRepository, ICategoryRepository categoryRepository, IActorRepository actorRepository)
        {
            this.movieRepository = movieRepository;
            this.cinemaRepository = cinemaRepository;
            this.categoryRepository = categoryRepository;
            this.actorRepository = actorRepository;
        }

        
        public IActionResult Index()
        {
            var actors = actorRepository.Get();
            return View(actors.ToList());
        }

        public IActionResult Works(int ActorId)
        {
            var actor = actorRepository.GetOne(e => e.Id == ActorId,includes: [a => a.ActorMovies]);

            if (actor == null)
            {
                return NotFound();
            }
            var movies = movieRepository.Get(filter: m => m.ActorMovies.Any(am => am.ActorId == ActorId), 
                includes: [m => m.Cinema, m => m.Category]).ToList();

            ViewBag.Movies = movies;
            return View(actor);
        }



    }
}
