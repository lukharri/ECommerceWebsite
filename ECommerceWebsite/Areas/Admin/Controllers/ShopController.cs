using ECommerceWebsite.Models;
using ECommerceWebsite.Models.Data;
using ECommerceWebsite.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ECommerceWebsite.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Index
        [HttpGet]
        public ActionResult Index()
        {
            List<CategoryViewModel> categories;

            using (Db db = new Db())
            {
                categories = db.Categories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryViewModel(x))
                    .ToList();
            }

            return View(categories);
        }


        //GET: Admin/Shop/Categories
        [HttpGet]
        public ActionResult Categories()
        {
            List<CategoryViewModel> categories;

            using (Db db = new Db())
            {
                categories = db.Categories
                    .ToArray()
                    .OrderBy(x => x.Sorting)
                    .Select(x => new CategoryViewModel(x))
                    .ToList();
            }

            return View(categories);
        }


        // POST: /admin/shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;

            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";
                
                CategoryDto dto = new CategoryDto();

                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;
                db.Categories.Add(dto);
                db.SaveChanges();

                //id = db.Categories.FirstOrDefault(x => x.Name == catName).ToString();
                id = dto.Id.ToString();
            }

            return id;

        }


        // POST: /Admin/Pages/ReorderCatgegories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                // set initial count
                int count = 1;

                CategoryDto dto;

                // set sorting for each Category
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }
        }


        // GET: /Admin/Pages/DeleteCategory/id
        //[HttpGet]
        //public ActionResult DeleteCategory(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    using (Db db = new Db())
        //    {
        //        CategoryDto category = db.Categories.Find(id);

        //        if (category == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        CategoryViewModel model = new CategoryViewModel(category);

        //        return View(model);
        //    }
        //}


        // POST: /Admin/Shop/DeleteCategory/id
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteCategory(int id)
        //{
        //    using (Db db = new Db())
        //    {
        //        CategoryDto category = db.Categories.Find(id);
        //        db.Categories.Remove(category);
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("Categories");

        //}


        // GET: /admin/shop/DeleteCategory/id
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                CategoryDto category = db.Categories.Find(id);
                db.Categories.Remove(category);
                db.SaveChanges();
            }

            return RedirectToAction("Categories");

        }


        // POST: /Admin/Shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using(Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                CategoryDto dto = db.Categories.Find(id);
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower(); 
                     
                db.SaveChanges();
            }

            return "ok";
        }


        // GET: /Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            ProductViewModel model = new ProductViewModel();

            using(Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

            }
            return View(model);
        }


        //POST: /Admin/Shop/AddProduct
       [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult AddProduct(ProductViewModel model, HttpPostedFileBase file)
        {
            ProductDto dto = new ProductDto();
            dto.Id = model.Id;
            dto.Name = model.Name;
            dto.Slug = model.Slug;
            dto.Description = model.Description;
            dto.Price = model.Price;

            using (Db db = new Db())
            {
                db.Products.Add(dto);
                db.SaveChanges();
            }
            return View();
        }
    }
}