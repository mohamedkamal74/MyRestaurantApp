using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.ViewModels;
using NToastNotify;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public SubCategoriesController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult>Index()
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
                SubCategoriesList = await _context.SubCategories.OrderBy(m => m.Name).Select(x => x.Name).Distinct().ToListAsync()

            };
            return View(categoryandcategoryVm);
        }
    }
}
