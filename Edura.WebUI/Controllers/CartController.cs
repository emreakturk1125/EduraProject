using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edura.WebUI.Entity;
using Edura.WebUI.Infrastructure;
using Edura.WebUI.Models;
using Edura.WebUI.Repository.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edura.WebUI.Controllers
{
    public class CartController : Controller
    {
        private IUnitOfWork unitOfWork; 

        public CartController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork; 
        }

        public IActionResult Index()
        {
            return View(GetCart());
        }

        public IActionResult AddToCart(int ProductId, int quantity = 1)
        {
            var product = unitOfWork.Products.Get(ProductId);
            if (product != null)
            {
                var cart = GetCart();
                cart.AddProduct(product,quantity);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int ProductId)
        {
            var product = unitOfWork.Products.Get(ProductId);
            if (product != null)
            {
                var cart = GetCart();
                cart.RemoveProduct(product);
                SaveCart(cart);

            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Checkout(OrderDetails model)
        {
            var cart = GetCart();

            if (cart.Products.Count == 0)
            {
                ModelState.AddModelError("UrunYokModel", "Sepetinizde ürün bulunmamaktadır.");
            }

            if (ModelState.IsValid)
            {
                SaveOrder(cart, model);
                cart.ClearAll();
                SaveCart(cart);
                return View("Completed");
            }

            return View(model);
        }

        private void SaveOrder(Cart cart, OrderDetails details)
        {
            var order = new Order();

            order.OrderNumber = "A" + (new Random()).Next(11111, 99999).ToString();
            order.Total = cart.TotalPrice();
            order.OrderDate = DateTime.Now;
            order.OrderState = EnumOrderState.Waiting;
            order.Username = User.Identity.Name;

            order.AdresTanimi = details.AdresTanimi;
            order.Adres = details.Adres;
            order.Sehir = details.Sehir;
            order.Semt = details.Semt;
            order.Telefon = details.Telefon;

            foreach (var product in cart.Products)
            {
                var orderline = new OrderLine();
                orderline.Quantity = product.Quantity;
                orderline.Price = product.Product.Price;
                orderline.ProductId = product.Product.ProductId;

                order.OrderLines.Add(orderline);
            }

            unitOfWork.Orders.Add(order);
            unitOfWork.SaveChanges();
        }

        private void SaveCart(Cart cart)
        {
            HttpContext.Session.SetJson("Cart", cart);
        }

        private Cart GetCart()
        {
            return HttpContext.Session.GetJson<Cart>("Cart") ?? new Cart();
        }
    }
}