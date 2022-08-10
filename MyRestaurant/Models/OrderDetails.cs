using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRestaurant.Models
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Order")]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual OrderHeader OrderHeader { get; set; }
        public int MenuItemId { get; set; }
        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
