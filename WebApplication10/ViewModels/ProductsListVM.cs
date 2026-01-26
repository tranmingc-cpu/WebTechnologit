
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebApplication10.Models;

namespace WebApplication10.ViewModels
{

    public class ProductListVM
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        //demo
        public string CategoryName { get; set; }
        public int? BrandId { get; set; }
        public string BrandName { get; set; }

        public string   Description { get; set; }
        public decimal? Discount { get; set; }

        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int CategoryId { get; set; }

        public string Status { get; set; }
    }
}