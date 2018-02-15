﻿using ECommerceWebsite.Models;
using ECommerceWebsite.Models.Data;
using ECommerceWebsite.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
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
            if(!ModelState.IsValid)
            {
                using(Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Ensure product name is unique
            using (Db db = new Db())
            {
                if(db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "That product name already exists.");
                    return View(model);
                }
            }

            int id;

            // Save product
            using (Db db = new Db())
            {
                ProductDto product = new ProductDto();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                // Assign category name to product based on the category id  
                CategoryDto category = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = category.Name;

                db.Products.Add(product);
                db.SaveChanges();

                id = product.Id;
            }

            TempData["SM"] = "Product added successfully.";

            #region Upload Image

            // Create directories for storing product images
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            // Check if file was uploaded
            if(file != null && file.ContentLength > 0)
            {
                // Get file extension
                string ext = file.ContentType.ToLower();

                // Verify file extension
                if(ext != "image/jpg" && 
                   ext != "image/jpeg" && 
                   ext != "image/pjpeg" && 
                   ext != "image/gif" && 
                   ext != "image/x-png" &&
                   ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension.");
                        return View(model);
                    }

                }

                string imageName = file.FileName;

                using(Db db = new Db())
                {
                    ProductDto dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                // Set original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                file.SaveAs(path);

                // Create and save thumb of the original
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);

            }

            #endregion

            return RedirectToAction("AddProduct");
        }
    }
}