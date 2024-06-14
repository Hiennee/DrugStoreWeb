using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Authentication.Helper;
using Authentication.Models;
using Microsoft.AspNetCore.Authorization;

namespace PetStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly DrugStore_AuthenticationContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger, DrugStore_AuthenticationContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            //Lay cart tu session
            Cart cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            return View(cart);
        }

        public async Task<IActionResult> Add(int id, int pageIndex)
        {
            Product? p = _context.Products.Include(p => p.Category).First(p => p.Id == id);
               
            Item item = new Item
            {
                Id = p.Id,
                Category = p.Category.Name,
                Description = p.Description,
                Discount = p.Discount,
                Price = p.Price,
                Quantity = 1
            };
            //Lay cart tu session
            Cart cart = HttpContext.Session.Get<Cart>("cart")??new Cart();
            //Luu item vao cart
            cart.Add(item);
            //Luu cart vao session
            int route = 1;
            HttpContext.Session.Set<Cart>("cart", cart);
            //Quay ve /Home/Index de hien lai home page
            return RedirectToAction("Index", "Home", new {pageIndex = pageIndex});
        }

        public async Task<IActionResult> Remove(int id)
        {
            //Lay cart tu session
            Cart cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            //Remove item tu cart
            cart.Remove(id);
            //Luu cart vao session
            HttpContext.Session.Set<Cart>("cart", cart);
            //Quay ve thuc hien action /Cart/Index
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Empty()
        {
            //Lay cart tu session
            Cart cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            //Empty cart
            cart.Empty();
            //Luu cart vao session
            HttpContext.Session.Set<Cart>("cart", cart);
            //Quay ve thuc hien action /Cart/Index
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id, int quantity)
        {
            //Lay cart tu session
            Cart cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            //Empty cart
            cart.Update(id, quantity);
            //Luu cart vao session
            HttpContext.Session.Set<Cart>("cart", cart);
            //Quay ve thuc hien action /Cart/Index
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Purchase()
        {
            Cart cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            //AspNetOrder order = new AspNetOrder { Date:  }
            //_context.AspNetOrders.Add()
            return View(cart);
        }
        [HttpPost]
        public async Task<IActionResult> Purchase(int? date, int? month, int? year)
        {
            Cart cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            if (cart == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Console.WriteLine("HELLLLLLLLLLo", cart.TotalAmount);
            AspNetOrder order = new AspNetOrder
            {
                Date = new DateTime(year ?? DateTime.Now.Year, month ?? DateTime.Now.Month, date ?? DateTime.Now.Day),
                Status = "Successfully",
                CustomerId = User.Identity?.Name,
                Value = cart.TotalAmount,
            };
            _context.AspNetOrders.Add(order);
            await _context.SaveChangesAsync();
            foreach (var (index, item) in cart.List)
            {
                AspNetOrderDetail orderDetail = new AspNetOrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.Id,
                    Quantity = item.Quantity,
                    Price = item.TotalPrice,
                    Discount = item.Discount,
                };
                _context.AspNetOrderDetails.Add(orderDetail);
                await _context.SaveChangesAsync();
            }
            HttpContext.Session.Set<Cart>("cart", new Cart());
            return RedirectToAction("Index", "Home");
        }
    }
}
