using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication10.Models;

namespace WebApplication10.ViewModels
{
    public class AdminProductsFormVM
    {

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ProductDescription
        {
            get;
            set;
        }
        public int ProductId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int? BrandId { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal? Discount { get; set; }
        public int? Quantity { get; set; }

        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public string Status { get; set; } = "Available";

        // dropdown
        public IEnumerable<Categories> Categories { get; set; }
        public IEnumerable<Brands> Brands { get; set; }

    }
}