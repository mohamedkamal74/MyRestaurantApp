using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using NToastNotify;
using System.Threading.Tasks;

namespace MyRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public MenuItemsController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public async Task<IActionResult> Index()
        {
            var menuitems = await _context.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuitems);
        }
    }
}
