using MyRestaurant.Models;
using System.Collections.Generic;

namespace MyRestaurant.ViewModels
{
    public class MenuItemViewmodel
    {
        public MenuItem MenuItem { get; set; }
        public IEnumerable<Category> CategoriesList { get; set; }
        public IEnumerable<SubCategory> SubCategoriesList { get; set; }
    }
}
