using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edura.WebUI.Entity;
using Edura.WebUI.Models;
using Edura.WebUI.Repository.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Edura.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private IUnitOfWork unitofWork;

        public AdminController(IUnitOfWork _unitofWork)
        {
            unitofWork = _unitofWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var entity = unitofWork.Categories.GetAll()
                .Include(x => x.ProductCategories)
                .ThenInclude(i => i.Product)
                .Where(x => x.CategoryId == id)
                .Select(x => new AdminEditCategoryModel()
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Products = x.ProductCategories.Select(a => new AdminEditCategoryProduct() {

                        ProductId = a.ProductId,
                        ProductName = a.Product.ProductName,
                        Image = a.Product.Image,
                        IsApproved = a.Product.IsApproved,
                        IsFeatured = a.Product.IsFeatured,
                        IsHome = a.Product.IsHome 

                    }).ToList()

                }).FirstOrDefault();

            return View(entity);
        }

        [HttpPost]
        public IActionResult EditCategory(Category entity)
        {
            if (ModelState.IsValid)
            {
                unitofWork.Categories.Edit(entity);
                unitofWork.SaveChanges();
                return RedirectToAction("CatalogList");
            }
            return View("Error");
        }


        public IActionResult CatalogList()
        {
            var model = new CatalogListModel()
            {
                Categories = unitofWork.Categories.GetAll().ToList(),
                Products = unitofWork.Products.GetAll().ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(Category entity)
        {
            if (ModelState.IsValid)
            {
                unitofWork.Categories.Add(entity);
                unitofWork.SaveChanges();
                return RedirectToAction("CatalogList");
                //return Ok(entity);
            }
            return BadRequest();
        }

        [HttpGet]
        public IActionResult  AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product entity,IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", file.FileName);
                    var path_tn = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products\\tn", file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        entity.Image = file.FileName;
                    }
                    using (var stream = new FileStream(path_tn, FileMode.Create))
                    {
                        await file.CopyToAsync(stream); 
                    }
                }

                entity.DateAdded = DateTime.Now;
                unitofWork.Products.Add(entity);
                unitofWork.SaveChanges();
                return  RedirectToAction("CatalogList");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCategory(int ProductId,int CategoryId)
        {
            if (ModelState.IsValid)
            {
                unitofWork.Categories.RemoveFromCategory(ProductId, CategoryId);
                unitofWork.SaveChanges();
                return RedirectToAction("EditCategory/"+CategoryId);
            }
            return BadRequest();
        }
    }
}