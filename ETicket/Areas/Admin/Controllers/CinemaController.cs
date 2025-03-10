using ETicket.Data;
using ETicket.Models;
using ETicket.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETicket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ICinemaRepository cinemaRepository;

        public CinemaController(ICinemaRepository cinemaRepository)
        {
            this.cinemaRepository = cinemaRepository;
        }
    

        public IActionResult Index()
        {
            var cinemas = cinemaRepository.Get().ToList();
            return View(cinemas);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }

            if (ImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cinemas", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                cinema.CinemaLogo = fileName;
            }

            cinemaRepository.Create(cinema);
            cinemaRepository.Commit();

            TempData["SuccessMessage"] = $"Cinema '{cinema.Name}' created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cinema = cinemaRepository.GetOne(c => c.Id == id);
            if (cinema == null) return NotFound();

            return View(cinema);
        }

        [HttpPost]
        public IActionResult Edit(Cinema cinema, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }

            var cinemaInDb = cinemaRepository.GetOne(c => c.Id == cinema.Id, tracked: false);
            if (cinemaInDb == null) return NotFound();

            if (ImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cinemas", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(stream);
                }

                if (!string.IsNullOrEmpty(cinemaInDb.CinemaLogo))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cinemas", cinemaInDb.CinemaLogo);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                cinema.CinemaLogo = fileName; 
            }
            else
            {
                cinema.CinemaLogo = cinemaInDb.CinemaLogo; 
            }

            cinemaInDb.Name = cinema.Name;
            cinemaInDb.Description = cinema.Description;
            cinemaInDb.Address = cinema.Address;
            cinemaInDb.CinemaLogo = cinema.CinemaLogo;

            cinemaRepository.Edit(cinemaInDb);
            cinemaRepository.Commit();

            TempData["SuccessMessage"] = $"Cinema '{cinema.Name}' updated successfully!";
            return RedirectToAction("Index");
        }



        public IActionResult Delete(int id)
        {
            var cinema = cinemaRepository.GetOne(c => c.Id == id);
            if (cinema == null) return NotFound();

            if (!string.IsNullOrEmpty(cinema.CinemaLogo))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cinemas", cinema.CinemaLogo);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            cinemaRepository.Delete(cinema);
            cinemaRepository.Commit();

            TempData["SuccessMessage"] = $"Cinema '{cinema.Name}' deleted successfully!";
            return RedirectToAction("Index");
        }


    }
}
