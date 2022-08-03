using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.ViewModels;
using NToastNotify;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public MenuItemViewmodel MenuItemVM { get; set; }

        public MenuItemsController(ApplicationDbContext context, IToastNotification toastNotification,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _toastNotification = toastNotification;
            _webHostEnvironment = webHostEnvironment;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost()
        {
            if (ModelState.IsValid)
            {
                string imagepath = @"\Images\Default.png.png";
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    string webrootpath = _webHostEnvironment.WebRootPath;
                    string imagename = DateTime.Now.ToFileTime().ToString() + Path.GetExtension(files[0].FileName);
                    FileStream fileStream=new FileStream(Path.Combine(webrootpath,"Images",imagename), FileMode.Create);
                   await files[0].CopyToAsync(fileStream);
                    imagepath = @"\Images\" + imagename;
                }
                MenuItemVM.MenuItem.Image= imagepath;
                _context.MenuItems.Add(MenuItemVM.MenuItem);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("menu Item created Succesfully");
                return RedirectToAction(nameof(Index));

            }
            return View(MenuItemVM);
        }

        public async Task<IActionResult>Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var menuitem=_context.MenuItems.Include(m=>m.Category).Include(m=>m.SubCategory).FirstOrDefault(x=>x.Id==id);
            if (menuitem == null)
                return NotFound();
            MenuItemVM.MenuItem = menuitem;
            MenuItemVM.SubCategoriesList = await _context.SubCategories.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost()
        {
            if (ModelState.IsValid)
            {
               
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    string webrootpath = _webHostEnvironment.WebRootPath;
                    string imagename = DateTime.Now.ToFileTime().ToString() + Path.GetExtension(files[0].FileName);
                    FileStream fileStream = new FileStream(Path.Combine(webrootpath, "Images", imagename), FileMode.Create);
                    await files[0].CopyToAsync(fileStream);
                   string  imagepath = @"\Images\" + imagename;
                    MenuItemVM.MenuItem.Image = imagepath;
                }
               
                _context.MenuItems.Update(MenuItemVM.MenuItem);
                await _context.SaveChangesAsync();
                _toastNotification.AddInfoToastMessage("menu Item Updated Succesfully");
                return RedirectToAction(nameof(Index));

            }
            return View(MenuItemVM);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            var menuitem = _context.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefault(x => x.Id == id);
            if (menuitem == null)
                return NotFound();
            MenuItemVM.MenuItem = menuitem;
            MenuItemVM.SubCategoriesList = await _context.SubCategories.Where(m => m.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
            return View(MenuItemVM);
        }

    }
}
