using MyRestaurant.Models;
using System.Collections.Generic;

namespace MyRestaurant.ViewModels
{
    public class OrderListViewModel
    {
        public List<OrderDetailsViewModel> Orders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
