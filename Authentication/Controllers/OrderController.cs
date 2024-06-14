using Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        public DrugStore_AuthenticationContext _context;
        public ILogger<OrderController> _logger;
        public OrderController(DrugStore_AuthenticationContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index(int? month, string? customer)
        {
            IQueryable<AspNetOrder> orders;
            if (User.IsInRole("Admin"))
            {
                orders = _context.AspNetOrders;
                if (customer != null)
                {
                    orders = orders.Where(o => o.CustomerId == customer);
                }
            }
            else
                orders = _context.AspNetOrders.Where(o => o.CustomerId == User.Identity.Name);
            if (month != null)
            {
                orders = orders.Where(o => o.Date.Month == month);
            }
            
            //var orders = (IQueryable<AspNetOrder>) order;
            return View(orders);
        }
    }
}
