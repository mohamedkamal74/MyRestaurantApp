using MyRestaurant.Models;
using System.Collections.Generic;

namespace MyRestaurant.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<MenuItem> MenuItems { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Copoun> Copouns { get; set; }
    }
}
