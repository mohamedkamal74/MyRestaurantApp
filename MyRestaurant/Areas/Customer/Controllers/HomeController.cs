using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.Utility;
using MyRestaurant.ViewModels;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyRestaurant.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toastNotification;

        public HomeController(ApplicationDbContext context, IToastNotification toastNotification)
        {
            _context = context;
            _toastNotification = toastNotification;
        }
       
        public async Task< IActionResult> Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel()

            {
                Categories = await _context.Categories.ToListAsync(),
                Copouns = await _context.Copouns.Where(m=>m.IsActive).ToListAsync(),
                MenuItems=await _context.MenuItems.Include(m=>m.Category).Include(m=>m.SubCategory).ToListAsync()
            };
            return View(homeViewModel);
        }
        [Authorize]
        public async Task<IActionResult>Details(int itemid)
        {
            var menuitem= await _context.MenuItems.Include(m=>m.Category).Include(m=>m.SubCategory).FirstOrDefaultAsync(m=>m.Id == itemid);

            var shoppingcart = new ShoppingCart
            {
                MenuItem=menuitem,
                MenuItemId=menuitem.Id
            };
            return View(shoppingcart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Details(ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;

                var shoppingcartitem = await _context.ShoppingCarts.Where(m => m.ApplicationUserId == shoppingCart.ApplicationUserId
                && m.MenuItemId == shoppingCart.MenuItemId).FirstOrDefaultAsync();

                if (shoppingcartitem == null)
                {
                    _context.ShoppingCarts.Add(shoppingCart);
                }
                else
                {
                    shoppingcartitem.Count += shoppingCart.Count;
                }
                await _context.SaveChangesAsync();
                var count = _context.ShoppingCarts.Where(m => m.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count;

              //  HttpContext.Session.SetInt32(SD.ShoppingCartCount, count);
                _toastNotification.AddSuccessToastMessage("product added to your cart");
                return RedirectToAction(nameof(Index));

            }
            var menuitem = await _context.MenuItems.Include(m => m.Category).Include(m => m.SubCategory).FirstOrDefaultAsync(m => m.Id == shoppingCart.MenuItemId);

            var shoppingcart = new ShoppingCart
            {
                MenuItem = menuitem,
                MenuItemId = menuitem.Id
            };
            return View(shoppingcart);

        }

       
    }
}
