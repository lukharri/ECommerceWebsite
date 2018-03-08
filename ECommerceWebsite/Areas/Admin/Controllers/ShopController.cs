using ECommerceWebsite.Models;
using ECommerceWebsite.Models.Data;
using ECommerceWebsite.Models.ViewModels.Shop;
using PagedList;
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


        // GET: /Admin/Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            List<ProductViewModel> products = new List<ProductViewModel>();

            var pageNumber = page ?? 1;

            using(Db db = new Db())
            {
                products = db.Products.ToArray()
                     .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                     .Select(x => new ProductViewModel(x))
                     .ToList();

                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                ViewBag.SelectedCategory = catId.ToString();
            }

            var onePageOfProducts = products.ToPagedList(pageNumber, 5);

            ViewBag.OnePageOfProducts = onePageOfProducts;

            return View(products);
        }


        // GET: /Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            ProductViewModel model;

            using (Db db = new Db())
            {
                ProductDto dto = db.Products.Find(id);

                if (dto == null)
                {
                    return Content("That product doesn't exist.");
                }

                model = new ProductViewModel(dto);

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                model.Images = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fileName => Path.GetFileName(fileName));
            }

            return View(model);
        }


        // POST: /Admin/Shop/EditProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProduct(ProductViewModel model, HttpPostedFileBase file)
        {
            // get product id
            int id = model.Id;

            // populate categories select list and images
            using (Db db = new Models.Data.Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            model.Images = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fileName => Path.GetFileName(fileName));

            // check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // make sure product name is unique
            using(Db db = new Db())
            {
                if(db.Products.Where(p => p.Id != id).Any(p => p.Name == model.Name))
                {
                    ModelState.AddModelError("", "Product name already exists.");
                    return View(model);
                }
            }

            // update product
            using(Db db = new Db())
            {
                ProductDto dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name.Replace(" ", "-").ToLower();
                dto.Price = model.Price;
                dto.Description = model.Description;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDto catDto = db.Categories.FirstOrDefault(c => c.Id == model.CategoryId);
                dto.CategoryName = catDto.Name;

                db.SaveChanges();
            }

            // set TempData msg
            TempData["SM"] = "Product edited successfully";

            #region Image Upload

            // check for file upload
            if (file != null && file.ContentLength > 0)
            {

                // get file extension
                string ext = file.ContentType.ToLower();

                // verify file extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension.");
                        return View(model);
                    }

                }

                // set upload directory paths
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // delete files from directories
                DirectoryInfo directoryInfo1 = new DirectoryInfo(pathString1);
                DirectoryInfo directoryInfo2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in directoryInfo1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (FileInfo file3 in directoryInfo2.GetFiles())
                {
                    file3.Delete();
                }

                // save image name
                string imageName = file.FileName;

                using(Db db = new Db())
                {
                    ProductDto dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }

                // save original and thumb images
                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                file.SaveAs(path);

                // Create and save thumb of the original
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);


            }
            // redirect

            #endregion

            // redirect
            return RedirectToAction("EditProduct");
        }


        // GET: /Admin/Shop/DeleteProduct/id
        [HttpGet]
        public ActionResult DeleteProduct(int? id)
        {
            ProductViewModel model;

            using(Db db = new Db())
            {
                ProductDto dto = db.Products.Find(id);
                model = new ProductViewModel(dto);
            }

            return View(model);
        }


        // POST: /Admin/Shop/DeletProduct/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProduct(int id)
        {
            using(Db db = new Db())
            {
                ProductDto product = db.Products.FirstOrDefault(p => p.Id == id);
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return RedirectToAction("Products", "Shop");
        }
    }
}