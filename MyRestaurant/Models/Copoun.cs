using System.ComponentModel.DataAnnotations;

namespace MyRestaurant.Models
{
    public class Copoun
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]

        public string CopounType { get; set; }
        public enum ECopounType { Percent = 0, Dollar = 1 }
        [Required]

        public double  Discount { get; set; }
        [Required]
        [Display(Name = "Minimum Amount")]

        public double MinimumAmount { get; set; }
        public byte[] Picture { get; set; }
        [Display(Name = "Is Active")]

        public bool IsActive { get; set; }

    }
}
