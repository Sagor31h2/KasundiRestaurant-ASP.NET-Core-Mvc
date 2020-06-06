using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KasundiRestaurant.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Price should be greater Than ${1}")]
        public double Price { get; set; }

        public string Spicyness { get; set; }
        public enum Espicy
        {
            Na = 0,
            NotSpicy = 1,
            Spicy = 2,
            VerySpicy = 3
        }

        public string Image { get; set; }

        [Display(Name = "CategoryId")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")] public virtual Category Category { get; set; }


        [Display(Name = "SubCategoryId")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")] public virtual SubCategory SubCategory { get; set; }
    }
}
