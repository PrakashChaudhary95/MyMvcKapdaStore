using KapdaStore.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Http;
using KapdaStore.Helpers;
using KapdaStore.Data;

namespace KapdaStore.Controllers
{
    public class PrakashController : Controller  // <-- Corrected class name here
    {
        private List<Product> GetProductList()
        {
            return new List<Product>
            {
                new Product { Id=1, Name="Cotton Shirt", Description="Comfortable cotton shirt", Price=899, Size="M", ImageUrl="/images/shirt1.jpg" },
                new Product { Id=2, Name="Denim Jeans", Description="Stylish blue jeans", Price=1499, Size="L", ImageUrl="/images/jeans1.jpg" },
                new Product { Id=3, Name="Formal Trousers", Description="Perfect for office", Price=1299, Size="M", ImageUrl="/images/trousers1.jpg" },
                new Product { Id=4, Name="Summer T-Shirt", Description="Light and airy", Price=499, Size="S", ImageUrl="/images/tshirt1.jpg" },
                new Product { Id=5, Name="Jacket", Description="Warm and trendy", Price=1999, Size="XL", ImageUrl="/images/jacket1.jpg" },
                new Product { Id=6, Name="Casual Shorts", Description="Comfortable for summer", Price=599, Size="L", ImageUrl="/images/shorts1.jpg" },
                new Product { Id=7, Name="Women Saree", Description="Light and Stylish", Price=999, Size="l", ImageUrl="/images/saree.jpg" },
                new Product { Id=8, Name="Women Top", Description="Stylish and Comfortable", Price=1299, Size="M", ImageUrl="/images/tops1.jpg" },
                new Product { Id=9, Name="Women Kurti", Description="Comfortable for Women", Price=799, Size="XL", ImageUrl="/images/kurtis1.jpg" },


            };
        }

        public IActionResult Index(string search)
        {
            var products = GetProductList();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                products = products
                    .Where(p => p.Name.ToLower().Contains(search)
                             || p.Description.ToLower().Contains(search)
                             || p.Size.ToLower().Contains(search))
                    .ToList();
            }

            ViewData["SearchValue"] = search ?? "";  // Ye line add ki gayi hai

            return View(products);
        }


        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                TempData["Error"] = "⚠ Please login to add items to cart.";
                return RedirectToAction("Login", "Account");
            }

            var product = GetProductList().FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();

            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            var existing = cart.FirstOrDefault(c => c.Product.Id == id);
            if (existing != null)
                existing.Quantity++;
            else
                cart.Add(new CartItem { Product = product, Quantity = 1 });

            HttpContext.Session.SetObject("Cart", cart);
            return RedirectToAction("Cart");
        }

        public IActionResult Cart()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                TempData["Error"] = "⚠ Please login to view your cart.";
                return RedirectToAction("Login", "Account");
            }

            var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
            return View(cart);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }
    }
}
