using ECommerceWebsite.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        [AllowHtml]
        public string Body { get; set; }

    }
}