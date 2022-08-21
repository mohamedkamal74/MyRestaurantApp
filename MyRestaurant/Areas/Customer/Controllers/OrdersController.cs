using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.Utility;
using MyRestaurant.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            var orderdetailsVM = new ViewModels.OrderDetailsViewModel
            {
                OrderDetails = await _context.OrderDetails.Where(x => x.OrderId == id).ToListAsync(),
                OrderHeader = await _context.OrderHeaders.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.Id == id && x.UserId == claim.Value)
            };
            return View(orderdetailsVM);
        }



        private int PageSize = 2;
        [Authorize]
        public async Task<IActionResult> OrderHistory(int PageNumber = 1)
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            //  List<OrderDetailsViewModel> orderDetailsViewModels = new List<OrderDetailsViewModel>();

            OrderListViewModel orderListVM = new OrderListViewModel
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            List<OrderHeader> orderHeadersList = await _context.OrderHeaders.Include(m => m.ApplicationUser).Where(m => m.UserId == claim.Value).ToListAsync();

            foreach (var orderHeader in orderHeadersList)
            {
                OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel
                {
                    OrderHeader = orderHeader,
                    OrderDetails = await _context.OrderDetails.Where(m => m.OrderId == orderHeader.Id).ToListAsync()
                };
                orderListVM.Orders.Add(orderDetailsVM);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(x => x.OrderHeader.Id).Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = PageNumber,
                RecordsPerPage = PageSize,
                TotalRecords = count,
                UrlParam = "/Customer/Orders/OrderHistory?PageNumber=:"
            };

            return View(orderListVM);


        }


        [Authorize(Roles = SD.ManagerUser + "," + SD.StatusInProcess)]
        public async Task<IActionResult> ManageOrder()
        {

            List<OrderDetailsViewModel> orderDetailsViewModels = new List<OrderDetailsViewModel>();

            List<OrderHeader> orderHeadersList = await _context.OrderHeaders.Where(m => m.Status == SD.StatusInProcess || m.Status == SD.StatusSubmited).ToListAsync();

            foreach (var orderHeader in orderHeadersList)
            {
                OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel
                {
                    OrderHeader = orderHeader,
                    OrderDetails = await _context.OrderDetails.Where(m => m.OrderId == orderHeader.Id).ToListAsync()
                };
                orderDetailsViewModels.Add(orderDetailsVM);
            }

            return View(orderDetailsViewModels.OrderBy(o => o.OrderHeader.PickUpTime).ToList());


        }

        public async Task<IActionResult> GetOrderDetails(int id)
        {
            OrderDetailsViewModel orderDetailsVm = new OrderDetailsViewModel
            {
                OrderHeader = await _context.OrderHeaders.Include(m => m.ApplicationUser).FirstOrDefaultAsync(m => m.Id == id),
                OrderDetails = await _context.OrderDetails.Where(m => m.OrderId == id).ToListAsync()
            };

            return PartialView("_IndividualOrderDetails", orderDetailsVm);


        }

        public async Task<IActionResult> GetOrderStatus(int id)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            return PartialView("_OrderStatus", orderHeader.Status);

        }


        [Authorize(Roles = SD.ManagerUser + "," + SD.StatusInProcess)]
        public async Task<IActionResult> OrderPrepare(int orderId)
        {
            var orderheader = await _context.OrderHeaders.FindAsync(orderId);
            orderheader.Status = SD.StatusInProcess;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.StatusInProcess)]
        public async Task<IActionResult> OrderReady(int orderId)
        {
            var orderheader = await _context.OrderHeaders.FindAsync(orderId);
            orderheader.Status = SD.StatusReady;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }


        [Authorize(Roles = SD.ManagerUser + "," + SD.StatusInProcess)]
        public async Task<IActionResult> OrderCancel(int orderId)
        {
            var orderheader = await _context.OrderHeaders.FindAsync(orderId);
            orderheader.Status = SD.StatusCancelled;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }


        [Authorize(Roles =SD.ManagerUser +","+SD.FrontDeskUser)]
        public async Task<IActionResult> OrderPickup(int PageNumber = 1,string searchName=null, string searchPhone = null, string searchEmail = null)
        {
           
            OrderListViewModel orderListVM = new OrderListViewModel
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            StringBuilder param=new StringBuilder();
            param.Append("/Customer/Orders/OrderPickup?PageNumber=:");

            param.Append("&searchName=");
            if(searchName != null)
            {
                param.Append(searchName);
            }
            else
            {
                searchName= string.Empty;
            }

            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }
            else
            {
                searchPhone = string.Empty;
            }

            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            else
            {
                searchEmail = string.Empty;
            }

            List<OrderHeader> orderHeadersList = await _context.OrderHeaders.Include(m => m.ApplicationUser)
                .OrderByDescending(m=>m.OrderDate).Where(m => m.Status==SD.StatusReady&&m.PickUpName.ToLower().Contains(searchName)
                &&m.PhoneNumber.Contains(searchPhone)&&m.ApplicationUser.Email.Contains(searchEmail)).ToListAsync();

            foreach (var orderHeader in orderHeadersList)
            {
                OrderDetailsViewModel orderDetailsVM = new OrderDetailsViewModel
                {
                    OrderHeader = orderHeader,
                    OrderDetails = await _context.OrderDetails.Where(m => m.OrderId == orderHeader.Id).ToListAsync()
                };
                orderListVM.Orders.Add(orderDetailsVM);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(x => x.OrderHeader.Id).Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = PageNumber,
                RecordsPerPage = PageSize,
                TotalRecords = count,
                UrlParam = param.ToString()
            };

            return View(orderListVM);


        }


        [Authorize(Roles = SD.ManagerUser + "," + SD.FrontDeskUser)]
        [HttpPost]
        public async Task<IActionResult> OrderPickup(int orderId)
        {
            var orderheader = await _context.OrderHeaders.FindAsync(orderId);
            orderheader.Status = SD.StatusCompleted;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageOrder));
        }

    }
}
