using System.ComponentModel.DataAnnotations;

namespace MyRestaurant.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Category Name")]
        public string Name { get; set; }
    }
}
