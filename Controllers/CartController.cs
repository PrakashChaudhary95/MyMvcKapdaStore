using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using KapdaStore.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace KapdaStore.Controllers
{
    public class CartController : Controller
    {
        private const string SessionCartKey = "CartSession";
        private readonly IAntiforgery _antiforgery;

        public CartController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            try
            {
                await _antiforgery.ValidateRequestAsync(HttpContext);
            }
            catch
            {
                return Unauthorized("Invalid CSRF token.");
            }

            var cart = GetCartFromSession();

            var item = cart.FirstOrDefault(i => i.Product.Id == request.ProductId);
            if (item != null)
            {
                item.Quantity += request.Change;
                if (item.Quantity < 1)
                    item.Quantity = 1;
            }

            SaveCartToSession(cart);

            return Json(cart.Select(i => new
            {
                product = new { id = i.Product.Id, price = i.Product.Price },
                quantity = i.Quantity
            }));
        }

        // ======= New Action: BuyNow =======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult BuyNow(int productId)
        {
            var product = GetProductById(productId);
            if (product == null)
            {
                return RedirectToAction("Cart");
            }

            var cart = GetCartFromSession();
            var item = cart.FirstOrDefault(i => i.Product.Id == productId);
            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Add(new CartItem { Product = product, Quantity = 1 });
            }
            SaveCartToSession(cart);

            // ✅ Redirect to Checkout page after adding to cart
            return RedirectToAction("Checkout");
        }


        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCartFromSession();
            if (cart == null || !cart.Any())
            {
                TempData["Error"] = "Cart is empty!";
                return RedirectToAction("Cart");
            }

            return View(new CheckoutViewModel());
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

            TempData["Success"] = "🎉 Order placed successfully!";
            HttpContext.Session.Remove(SessionCartKey);
            return RedirectToAction("OrderSuccess");
        }

        [HttpGet]
        public IActionResult OrderSuccess()
        {
            return View();
        }

        public IActionResult Cart()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }

        // ======= Session Helper Methods =======
        private List<CartItem> GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString(SessionCartKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<CartItem>();
            }
            return JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        private void SaveCartToSession(List<CartItem> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(SessionCartKey, cartJson);
        }

        // ======= Render Partial View as String =======
        private string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using var sw = new StringWriter();

            var viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            var viewResult = viewEngine.FindView(ControllerContext, viewName, false);

            if (!viewResult.Success)
                throw new FileNotFoundException($"View '{viewName}' not found.");

            var viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                ViewData,
                TempData,
                sw,
                new HtmlHelperOptions()
            );

            viewResult.View.RenderAsync(viewContext).Wait();
            return sw.ToString();
        }


        // ======= Dummy method to simulate product fetching - replace with your DB logic =======
        private Product GetProductById(int productId)
        {
            // Example: Replace with your actual DB call
            // return _dbContext.Products.Find(productId);

            // Dummy product for demo
            return new Product
            {
                Id = productId,
                Name = "Sample Product",
                Price = 999
            };
        }
    }

    public class UpdateQuantityRequest
    {
        public int ProductId { get; set; }
        public int Change { get; set; }
    }
}
