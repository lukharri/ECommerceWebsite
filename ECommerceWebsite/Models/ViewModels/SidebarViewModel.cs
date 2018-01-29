using ECommerceWebsite.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECommerceWebsite.Models.ViewModels
{
    public class SidebarViewModel
    {
        public SidebarViewModel()
        {

        }


        public SidebarViewModel(SidebarDto dto)
        {
            Id = dto.Id;
            Body = dto.Body;
        }


        public int Id { get; set; }
        public string Body { get; set; }

    }
}