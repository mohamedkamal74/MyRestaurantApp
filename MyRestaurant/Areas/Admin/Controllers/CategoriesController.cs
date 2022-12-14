using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.Utility;
using NToastNotify;
using System.Threading.Tasks;

namespace MyRestaurant.Areas.Admin.Controllers
{
    [Authorize(Roles =(SD.ManagerUser))]
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
            if (ModelState.IsValid)
            {
                _context.Categories.Add(model);
                await _context.SaveChangesAsync();
                 _toastNotification.AddSuccessToastMessage("Category Created Succesfully");

                return RedirectToAction(nameof(Index));
            }
          return View(model);
        }

        public async Task<IActionResult>Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(model);
                await _context.SaveChangesAsync();
                _toastNotification.AddInfoToastMessage("Category Updated Succesufully");
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult>Delete(Category model)
        {
            if(model != null)
            {
                _context.Categories.Remove(model);
              await  _context.SaveChangesAsync();
                _toastNotification.AddErrorToastMessage("category Deleted");
                return RedirectToAction(nameof(Index));
            }
            return View();
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();
            return View(category);
        }

    }
}
