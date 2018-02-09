using ECommerceWebsite.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceWebsite.Models.ViewModels.Shop
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
        }

        public ProductViewModel(ProductDto dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Slug = dto.Slug;
            Description = dto.Description;
            Price = dto.Price;
            CategoryName = dto.CategoryName;
            CategoryId = dto.CategoryId;
            ImageName = dto.ImageName;    
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public string CategoryName { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public string ImageName { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> Images { get; set; }
    }
}