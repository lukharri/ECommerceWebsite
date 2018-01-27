using ECommerceWebsite.Models.Data;
using ECommerceWebsite.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerceWebsite.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageViewModel> pages;

            using (Db db = new Db())
            {
                pages = db.Pages
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new PageViewModel(x))
                    .ToList();
            }

            return View(pages);
        }
    }
}