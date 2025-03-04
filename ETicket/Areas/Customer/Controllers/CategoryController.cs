using ETicket.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace ETicket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CategoryController : Controller
    {

        private readonly IMovieRepository movieRepository;
        private readonly ICinemaRepository cinemaRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IActorRepository actorRepository;

        public CategoryController(IMovieRepository movieRepository, ICinemaRepository cinemaRepository, ICategoryRepository categoryRepository, IActorRepository actorRepository)
        {
            this.movieRepository = movieRepository;
            this.cinemaRepository = cinemaRepository;
            this.categoryRepository = categoryRepository;
            this.actorRepository = actorRepository;
        }
        public IActionResult Index()
        {
            var categories = categoryRepository.Get();
            return View(categories.ToList());
        }

        public IActionResult Movies(int CategoryId)
        {
            var category = categoryRepository.GetOne(c => c.Id == CategoryId);

            if (category == null)
            {
                return NotFound();
            }

            var movies = movieRepository.Get(filter: m => m.CategoryId == CategoryId,includes: [m => m.Cinema, m => m.Actors]).ToList();

            ViewBag.Category = category;
            return View(movies);
        }

    }
}
