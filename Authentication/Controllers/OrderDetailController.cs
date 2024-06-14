using Authentication.Helper;
using Authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Controllers
{
    public class OrderDetailController : Controller
    {
        public DrugStore_AuthenticationContext _context;
        public ILogger<OrderDetailController> _logger;

        public OrderDetailController(DrugStore_AuthenticationContext context, ILogger<OrderDetailController> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public IActionResult Index(int orderId)
        {
            var orderDetail = _context.AspNetOrderDetails.Where(o => o.OrderId == orderId).Include(o => o.Product);
            ViewBag.orderId = orderId;
            return View(orderDetail);
        }
    }
}
