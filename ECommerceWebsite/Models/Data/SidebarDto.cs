using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ECommerceWebsite.Models.Data
{
    [Table("Sidebar")]
    public class SidebarDto
    {
        [Key]
        public int Id { get; set; }
        public string Body { get; set; }

    }
}