using MyRestaurant.Models;
using System.Collections.Generic;

namespace MyRestaurant.ViewModels
{
    public class CategoryAndSubcategoryViewModel
    {
        public IEnumerable<Category> CategoriesList { get; set; }
        public SubCategory SubCategory { get; set; }
        public List<string> SubCategoriesList { get; set; }
        public string StatusMessage { get; set; }


    }
}
