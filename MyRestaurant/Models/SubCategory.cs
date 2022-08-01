﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyRestaurant.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Sub category Name")]
        public string Name { get; set; }
        [Required, Display(Name ="Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
