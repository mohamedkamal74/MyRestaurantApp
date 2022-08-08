using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using MyRestaurant.Utility;
using NToastNotify;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =(SD.ManagerUser))]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public UsersController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
           _toastNotification = toastNotification;
        }
        public async Task< IActionResult> Index()
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;
            return View(await _context.ApplicationUsers.Where(m=>m.Id!=userId).ToListAsync());
        }

        public async Task<IActionResult>LockUnLock(string? id)
        {
            if (id == null)
                return NotFound();
            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null)
                return NotFound();
            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(100);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;

            }
           await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
