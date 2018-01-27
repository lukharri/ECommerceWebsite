using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ECommerceWebsite.Models.Data
{
    public class Db : DbContext
    {
        public DbSet<PageDto> Pages { get; set; }
    }
}