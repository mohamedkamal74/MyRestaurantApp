using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using MyRestaurant.Models;
using NToastNotify;
using System.IO;
using System.Threading.Tasks;

namespace MyRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CopounsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public CopounsController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
        public async Task< IActionResult> Index()
        {
            return View(await _context.Copouns.ToListAsync());
        }

        public  IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(Copoun model)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files != null)
                {
                    var filestream = files[0].OpenReadStream();
                    var memorystream = new MemoryStream();
                    filestream.CopyTo(memorystream);
                    model.Picture = memorystream.ToArray();
                }
                _context.Copouns.Add(model);
                await _context.SaveChangesAsync();
                _toastNotification.AddSuccessToastMessage("Copoun Created Succesfully");
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task< IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var copoun =await _context.Copouns.FindAsync(id);
            if (copoun == null)
                return NotFound();
            return View(copoun);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Copoun model)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files .Count>0)
                {
                    var filestream = files[0].OpenReadStream();
                    var memorystream = new MemoryStream();
                    filestream.CopyTo(memorystream);
                    model.Picture = memorystream.ToArray();
                }
                _context.Copouns.Update(model);
                await _context.SaveChangesAsync();
                _toastNotification.AddAlertToastMessage("Copoun updated Succesfully");
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}
