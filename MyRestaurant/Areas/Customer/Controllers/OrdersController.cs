using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyRestaurant.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult>Confirm(int id)
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            var orderdetailsVM = new ViewModels.OrderDetailsViewModel
            {
                OrderDetails =await _context.OrderDetails.Where(x=>x.OrderId==id).ToListAsync(),
                OrderHeader=await _context.OrderHeaders.Include(x=>x.ApplicationUser).FirstOrDefaultAsync(x=>x.Id==id&&x.UserId==claim.Value)
            };
            return View(orderdetailsVM);
        }
    }
}
