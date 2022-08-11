using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.Utility;
using MyRestaurant.ViewModels;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

            if (HttpContext.Session.GetString(SD.ssCopounCode) != null)
            {
                OrderDetailsCartVM.OrderHeader.CopounCode = HttpContext.Session.GetString(SD.ssCopounCode);

                var copounfromDB = _context.Copouns.FirstOrDefault(m => m.Name.ToLower() == OrderDetailsCartVM.OrderHeader.CopounCode.ToLower());

                OrderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(copounfromDB, OrderDetailsCartVM.OrderHeader.OrderTotalOriginal);
            }

            return View(OrderDetailsCartVM);
        }

        public IActionResult ApplyCopoun()
        {
            if(OrderDetailsCartVM.OrderHeader.CopounCode == null)
            {
                OrderDetailsCartVM.OrderHeader.CopounCode = "";
            }
            HttpContext.Session.SetString(SD.ssCopounCode, OrderDetailsCartVM.OrderHeader.CopounCode);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCopoun()
        {
            
            HttpContext.Session.SetString(SD.ssCopounCode, String.Empty);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int cardId)
        {
            var shoppingcart = await _context.ShoppingCarts.FindAsync(cardId);
            shoppingcart.Count += 1;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

    }
}
