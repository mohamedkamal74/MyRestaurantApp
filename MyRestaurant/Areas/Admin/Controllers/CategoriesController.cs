using Microsoft.AspNetCore.Mvc;
using MyRestaurant.Data;

namespace MyRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
           _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
