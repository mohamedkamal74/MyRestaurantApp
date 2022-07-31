using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using MyRestaurant.Models;
using NToastNotify;
using System.Threading.Tasks;

namespace MyRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public CategoriesController(ApplicationDbContext context, IToastNotification toastNotification)
        {
           _context = context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _context.Categories.Add(model);
           await _context.SaveChangesAsync();
            _toastNotification.AddSuccessToastMessage("Category Created Succesfully");

            return RedirectToAction(nameof(Index));
        }
    }
}
