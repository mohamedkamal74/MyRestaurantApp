using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.ViewModels;
using NToastNotify;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoriesController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var subcategory = await _context.SubCategories.Include(c => c.Category).OrderBy(x => x.Name).ToListAsync();
            return View(subcategory);
        }
        public async Task<IActionResult> Create()
        {
            var categoryandcategoryVm = new CategoryAndSubcategoryViewModel
            {
                CategoriesList = await _context.Categories.ToListAsync(),
                SubCategory = new SubCategory(),

            };
            return View(categoryandcategoryVm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAndSubcategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existSubcategory = await _context.SubCategories.Include(m => m.Category).Where(m => m.Category.Id == model.SubCategory.CategoryId && m.Name == model.SubCategory.Name).ToArrayAsync();
                if (existSubcategory.Any())

                {
                    StatusMessage = "Error : this sub category exist under " + existSubcategory.FirstOrDefault().Category.Name + "Category";

                }
                else
                {
                    _context.SubCategories.Add(model.SubCategory);
                    await _context.SaveChangesAsync();
                    _toastNotification.AddSuccessToastMessage("Sub Category Created Succesfully");
                    return RedirectToAction(nameof(Index));
                }

            }
            var categoryandcategoryVm = new CategoryAndSubcategoryViewModel
            {
                CategoriesList = await _context.Categories.ToListAsync(),
                SubCategory = model.SubCategory,
                StatusMessage = StatusMessage

            };
            return View(categoryandcategoryVm);

        }

        [HttpGet]
        public async Task<IActionResult>GetSubCategories(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();
            subCategories = await _context.SubCategories.Where(x => x.CategoryId == id).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }
    }
}
