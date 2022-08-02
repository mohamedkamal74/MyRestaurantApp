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
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        [BindProperty]
        public MenuItemViewmodel MenuItemVM { get; set; }

        public MenuItemsController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
            MenuItemVM = new MenuItemViewmodel()
            {
                MenuItem=new MenuItem(),
                CategoriesList=_context.Categories.ToList()
            };
        }
        public async Task<IActionResult> Index()
        {
            var menuitems = await _context.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuitems);
        }

        public IActionResult Create()
        {
            return View(MenuItemVM);
        }
    }
}
