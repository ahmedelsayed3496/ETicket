using ETicket.Models;
using ETicket.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace ETicket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly ICategoryRepository categoryRepository;


        public CategoryController( ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            var categories = categoryRepository.Get();
            return View(categories.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            categoryRepository.Create(category);
            categoryRepository.Commit();

            TempData["SuccessMessage"] = $"Category '{category.Name}' created successfully!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = categoryRepository.GetOne(c => c.Id == id);

            if (category == null)
            {
                return RedirectToAction("Index");
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            categoryRepository.Edit(category);
            categoryRepository.Commit();

            TempData["SuccessMessage"] = $"Category '{category.Name}' updated successfully!";
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id)
        {
            var category = categoryRepository.GetOne(c => c.Id == id);

            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction("Index");
            }

            categoryRepository.Delete(category);
            categoryRepository.Commit();

            TempData["SuccessMessage"] = $"Category '{category.Name}' deleted successfully!";
            return RedirectToAction("Index");
        }

    }
}

