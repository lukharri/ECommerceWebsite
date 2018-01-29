using ECommerceWebsite.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerceWebsite.Models
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {

        }

        public CategoryViewModel(CategoryDto dto)
        {
            Id = dto.Id;
            Name = dto.Name;
            Slug = dto.Slug;
            Sorting = dto.Sorting;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public int Sorting { get; set; }

    }
}