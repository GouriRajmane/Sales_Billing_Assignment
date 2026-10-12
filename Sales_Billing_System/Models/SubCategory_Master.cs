using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sales_Billing_System.Models
{
    public class SubCategory_Master
    {
        [Key]
        public int SubCategoryId { get; set; }

        // Foreign key to Category_Master
        [Required]
        public int CategoryId { get; set; }

        // Many subcategories belong to one category
        public virtual Category_Master Category { get; set; }

        [Required]
        [StringLength(100)]
        public string SubCategoryName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        // One subcategory can have many products
        public virtual ICollection<Product_Master> Products { get; set; }

        public SubCategory_Master()
        {
            IsActive = true;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Products = new List<Product_Master>();
        }
    }
}