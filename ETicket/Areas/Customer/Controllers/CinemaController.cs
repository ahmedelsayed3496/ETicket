using ETicket.Repositories;
using ETicket.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace ETicket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CinemaController : Controller
    {
        private readonly IMovieRepository movieRepository;
        private readonly ICinemaRepository cinemaRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IActorRepository actorRepository;

        public CinemaController(IMovieRepository movieRepository, ICinemaRepository cinemaRepository, ICategoryRepository categoryRepository, IActorRepository actorRepository)
        {
            this.movieRepository = movieRepository;
            this.cinemaRepository = cinemaRepository;
            this.categoryRepository = categoryRepository;
            this.actorRepository = actorRepository;
        }
        public IActionResult Index()
        {
            var cinemas = cinemaRepository.Get();
            return View(cinemas.ToList());
        }

        public IActionResult Movies(int CinemaId)
        {
            var cinema = cinemaRepository.GetOne(c => c.Id == CinemaId);

            if (cinema == null)
            {
                return NotFound();
            }

            var movies = movieRepository.Get(filter: m => m.CinemaId == CinemaId,includes: [m => m.Category, m => m.Actors]).ToList();

            ViewBag.Cinema = cinema;
            return View(movies);
        }

    }
}
