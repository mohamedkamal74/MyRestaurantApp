﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyRestaurant.Data;
using MyRestaurant.Models;
using MyRestaurant.ViewModels;
using System.Collections.Generic;
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



        private int PageSize = 2;
        [Authorize]
        public async Task<IActionResult> OrderHistory(int PageNumber=1)
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            //  List<OrderDetailsViewModel> orderDetailsViewModels = new List<OrderDetailsViewModel>();

            OrderListViewModel orderListVM = new OrderListViewModel
            {
                Orders=new List<OrderDetailsViewModel>()
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
                CurrentPage=PageNumber,
                RecordsPerPage=PageSize,
                TotalRecords=count,
                UrlParam= "/Customer/Orders/OrderHistory?PageNumber=:"
            };

            return View(orderListVM);


        }

        public async Task<IActionResult>GetOrderDetails(int id)
        {
            OrderDetailsViewModel orderDetailsVm = new OrderDetailsViewModel
            {
                OrderHeader=await _context.OrderHeaders.Include(m=>m.ApplicationUser).FirstOrDefaultAsync(m=>m.Id==id),
                OrderDetails=await _context.OrderDetails.Where(m=>m.OrderId==id).ToListAsync()
            };

            return PartialView("_IndividualOrderDetails", orderDetailsVm);


        }
    }
}
