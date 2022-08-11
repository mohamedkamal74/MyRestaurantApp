using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;

namespace MyRestaurant.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
           _context = context;
        }
        [BindProperty]
        public OrderDetailsCartViewModel OrderDetailsCartVM { get; set; }
        public IActionResult Index()
        {
            OrderDetailsCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader=new OrderHeader()
            };
            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingcarts = _context.ShoppingCarts.Where(m => m.ApplicationUserId == claim.Value);
            if(shoppingcarts != null)
            {
                OrderDetailsCartVM.ShoppingCartsList = shoppingcarts.ToList();
            }

            foreach (var item in OrderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = _context.MenuItems.FirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetailsCartVM.OrderHeader.OrderTotal += item.MenuItem.Price * item.Count;
                OrderDetailsCartVM.OrderHeader.OrderTotal=Math.Round(OrderDetailsCartVM.OrderHeader.OrderTotal,2);

            }

            OrderDetailsCartVM.OrderHeader.OrderTotalOriginal = OrderDetailsCartVM.OrderHeader.OrderTotal;

            return View(OrderDetailsCartVM);
        }
        
    }
}
