using MyRestaurant.Models;
using System.Collections.Generic;

namespace MyRestaurant.ViewModels
{
    public class OrderDetailsCartViewModel
    {
        public List<ShoppingCart> ShoppingCartsList { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
