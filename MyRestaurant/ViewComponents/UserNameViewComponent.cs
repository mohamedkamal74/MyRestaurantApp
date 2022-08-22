using Microsoft.AspNetCore.Mvc;
using MyRestaurant.Data;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyRestaurant.ViewComponents
{
    public class UserNameViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public UserNameViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsidentity=(ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb=await _context.ApplicationUsers.FindAsync(claim.Value);
            return View(userFromDb);
        }
    }
}
