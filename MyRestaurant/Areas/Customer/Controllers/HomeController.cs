using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.ViewModels;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

       
    }
}
