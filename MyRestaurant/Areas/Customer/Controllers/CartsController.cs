using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                OrderHeader = new OrderHeader()
            };
            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingcarts = _context.ShoppingCarts.Where(m => m.ApplicationUserId == claim.Value);
            if (shoppingcarts != null)
            {
                OrderDetailsCartVM.ShoppingCartsList = shoppingcarts.ToList();
            }

            foreach (var item in OrderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = _context.MenuItems.FirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetailsCartVM.OrderHeader.OrderTotal += item.MenuItem.Price * item.Count;
                OrderDetailsCartVM.OrderHeader.OrderTotal = Math.Round(OrderDetailsCartVM.OrderHeader.OrderTotal, 2);

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


        public IActionResult Summary()
        {
            OrderDetailsCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new OrderHeader()
            };
            OrderDetailsCartVM.OrderHeader.OrderTotal = 0;

            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            var AppUser = _context.ApplicationUsers.Find(claim.Value);

            OrderDetailsCartVM.OrderHeader.PickUpName = AppUser.Name;
            OrderDetailsCartVM.OrderHeader.PhoneNumber = AppUser.PhoneNumber;
            OrderDetailsCartVM.OrderHeader.PickUpDate = DateTime.Now;

            var shoppingcarts = _context.ShoppingCarts.Where(m => m.ApplicationUserId == claim.Value);
            if (shoppingcarts != null)
            {
                OrderDetailsCartVM.ShoppingCartsList = shoppingcarts.ToList();
            }

            foreach (var item in OrderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = _context.MenuItems.FirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetailsCartVM.OrderHeader.OrderTotal += item.MenuItem.Price * item.Count;
                OrderDetailsCartVM.OrderHeader.OrderTotal = Math.Round(OrderDetailsCartVM.OrderHeader.OrderTotal, 2);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task< IActionResult> SummaryPost()
        {
           
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsCartVM.ShoppingCartsList=await  _context.ShoppingCarts.Where(m => m.ApplicationUserId == claim.Value).ToListAsync();

            OrderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.OrderDate = DateTime.Now;
            OrderDetailsCartVM.OrderHeader.UserId = claim.Value;
            OrderDetailsCartVM.OrderHeader.Status = SD.PaymentStatusPending;
            OrderDetailsCartVM.OrderHeader.PickUpTime = Convert.ToDateTime(OrderDetailsCartVM.OrderHeader.PickUpDate.ToShortDateString() + "" +
               OrderDetailsCartVM.OrderHeader.PickUpTime.ToShortDateString());
            OrderDetailsCartVM.OrderHeader.OrderTotalOriginal = 0;

            _context.OrderHeaders.Add(OrderDetailsCartVM.OrderHeader);
            await _context.SaveChangesAsync();


            foreach (var item in OrderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = _context.MenuItems.FirstOrDefault(m => m.Id == item.MenuItemId);

                OrderDetails orderDetails = new OrderDetails()
                {
                    
                    MenuItemId=item.MenuItemId,
                    OrderId=OrderDetailsCartVM.OrderHeader.Id,
                    Description=item.MenuItem.Description,
                    Name = item.MenuItem.Name ,
                    Price=item.MenuItem.Price,
                    Count=item.Count
                };

                OrderDetailsCartVM.OrderHeader.OrderTotal += item.MenuItem.Price * item.Count;

                _context.OrderDetails.Add(orderDetails);
                await _context.SaveChangesAsync();
                OrderDetailsCartVM.OrderHeader.OrderTotal = Math.Round(OrderDetailsCartVM.OrderHeader.OrderTotal, 2);

            }


            if (HttpContext.Session.GetString(SD.ssCopounCode) != null)
            {
                OrderDetailsCartVM.OrderHeader.CopounCode = HttpContext.Session.GetString(SD.ssCopounCode);

                var copounfromDB = _context.Copouns.FirstOrDefault(m => m.Name.ToLower() == OrderDetailsCartVM.OrderHeader.CopounCode.ToLower());

                OrderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(copounfromDB, OrderDetailsCartVM.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                OrderDetailsCartVM.OrderHeader.OrderTotal = OrderDetailsCartVM.OrderHeader.OrderTotalOriginal;
            }

            return View(OrderDetailsCartVM);
        }
        public  IActionResult ApplyCopoun()
        {
            if (OrderDetailsCartVM.OrderHeader.CopounCode == null)
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

        public async Task<IActionResult> Minus(int cardId)
        {
            var shoppingcart = await _context.ShoppingCarts.FindAsync(cardId);

            if (shoppingcart.Count > 1)
            {
                shoppingcart.Count -= 1;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Remove(int cardId)
        {
            var shoppingcart = await _context.ShoppingCarts.FindAsync(cardId);
            _context.ShoppingCarts.Remove(shoppingcart);
            await _context.SaveChangesAsync();

            var count = _context.ShoppingCarts.Where(m => m.ApplicationUserId == shoppingcart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ShoppingCartCount, count);

            return RedirectToAction(nameof(Index));

        }

    }
}
