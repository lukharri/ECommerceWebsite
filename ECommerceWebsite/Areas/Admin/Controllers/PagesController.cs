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
        //private Db db = new Db();

        // GET: /Admin/Pages
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


        // GET: /Admin/Pages/CreatePage
        [HttpGet]
        public ActionResult CreatePage()
        {
            return View();
        }


        // POST: /Admin/Pages/CreatePage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePage(PageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using(Db db = new Db())
            {
                // declare slug
                string slug;

                // init PageDto
                PageDto dto = new PageDto();

                // dto title
                dto.Title = model.Title;

                // check for and set slug if need be
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }

                // ensure title and slug are unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "That title or slug already exists.");
                    return View(model);
                }

                // dto the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSideBar = model.HasSideBar;
                dto.Sorting = 100;

                // Save dto
                db.Pages.Add(dto);
                db.SaveChanges();

                // tempData: similar to ViewBag but persists after the next request
                // 
                TempData["SM"] = "You have successfully created a new page.";

                return RedirectToAction("CreatePage");

            }
        }
    }
}