using ETicket.Models;
using ETicket.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace ETicket.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            return View(actors);
        }

      
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Actor()); 
        }


        [HttpPost]
        public IActionResult Create(Actor actor, IFormFile? ProfilePicture)
        {
            if (!ModelState.IsValid)
            {
                return View(actor); 
            }

  
            if (ProfilePicture != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cast", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePicture.CopyTo(stream);  
                }

                actor.ProfilePicture = fileName; 
            }

            actorRepository.Create(actor);  
            actorRepository.Commit();  

            TempData["SuccessMessage"] = $"Actor '{actor.FirstName} {actor.LastName}' created successfully!";  
            return RedirectToAction("Index");  
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var actor = actorRepository.GetOne(a => a.Id == id);  
            if (actor == null)
                return NotFound(); 

            return View(actor);  
        }

       
        [HttpPost]
        public IActionResult Edit(Actor actor, IFormFile? ProfilePicture)
        {
            if (!ModelState.IsValid)
            {
                return View(actor);  
            }

            var actorInDb = actorRepository.GetOne(a => a.Id == actor.Id, tracked: false);  
            if (actorInDb == null)
                return NotFound();  

            
            if (ProfilePicture != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cast", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePicture.CopyTo(stream);  
                }

                if (!string.IsNullOrEmpty(actorInDb.ProfilePicture))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cast", actorInDb.ProfilePicture);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath); 
                    }
                }

                actor.ProfilePicture = fileName;  
            }
            else
            {
                actor.ProfilePicture = actorInDb.ProfilePicture; 
            }

          
            actorInDb.FirstName = actor.FirstName;
            actorInDb.LastName = actor.LastName;
            actorInDb.Bio = actor.Bio;
            actorInDb.News = actor.News;
            actorInDb.ProfilePicture = actor.ProfilePicture;

            actorRepository.Edit(actorInDb);  
            actorRepository.Commit(); 

            TempData["SuccessMessage"] = $"Actor '{actor.FirstName} {actor.LastName}' updated successfully!";  
            return RedirectToAction("Index"); 
        }

       
        public IActionResult Delete(int id)
        {
            var actor = actorRepository.GetOne(a => a.Id == id);  
            if (actor == null)
                return NotFound(); 

            
            if (!string.IsNullOrEmpty(actor.ProfilePicture))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cast", actor.ProfilePicture);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);  
                }
            }

            actorRepository.Delete(actor);  
            actorRepository.Commit(); 

            TempData["SuccessMessage"] = $"Actor '{actor.FirstName} {actor.LastName}' deleted successfully!";  
            return RedirectToAction("Index");  
        }

    }
}
