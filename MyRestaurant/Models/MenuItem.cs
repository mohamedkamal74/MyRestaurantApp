using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRestaurant.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name  { get; set; }
        public string Description  { get; set; }
        public double Price  { get; set; }
        public string Image  { get; set; }
        public string Spicyness  { get; set; }
        public enum Espicy  { NA=0,NotSpicy=1,Spicy=2,VerySpicy=3 }
        [Display(Name="Category")]
        public int CategoryId  { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category  { get; set; }
        [Display(Name ="Sub Category")]
        public int SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public SubCategory SubCategory { get; set; }

    }
}
