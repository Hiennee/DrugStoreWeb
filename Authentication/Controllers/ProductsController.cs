using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Authentication.Models;
using Authentication.Helper;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly DrugStore_AuthenticationContext _context;

        public ProductsController(DrugStore_AuthenticationContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(int? pageIndex, string? orderBy, string? orderType, string? op,
            int? categoryId, string? description, double? fromPrice, double? toPrice)
        {
            var products = (IQueryable<Product>)_context.Products.Include(p => p.Category);
            products = Sort(products, orderBy, orderType, op);
            products = Filter(products, categoryId, description, fromPrice, toPrice, op);
            //return View(await drugStore_AuthenticationContext.ToListAsync());
            return View(await PaginatedList<Product>.CreateAsync(products, pageIndex ?? 1, 5));
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Description, Price, Discount, CategoryId, Available")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Price,Discount,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.Available = 0;
                _context.Products.Update(product);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
            //var product = await _context.Products.FindAsync(id);
            //if (product != null)
            //{
            //    _context.Products.Remove(product);
            //}

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        private IQueryable<Product> Filter(IQueryable<Product> products, int? categoryId, string? description, double? fromPrice, double? toPrice, string? op)
        {
            FilterInfo filterInfo = new FilterInfo();
            switch (op)
            {
                case "search-do":
                    filterInfo = new FilterInfo
                    {
                        CategoryId = categoryId,
                        Description = description,
                        FromPrice = fromPrice,
                        ToPrice = toPrice,
                    };
                    break;
                case "search-clear":
                    break;
                default:
                    filterInfo = HttpContext.Session.Get<FilterInfo>("filterInfo") ?? filterInfo;
                    break;
            }
            HttpContext.Session.Set<FilterInfo>("filterInfo", filterInfo);
            ViewBag.CategoryList = new SelectList(_context.Categories, "Id", "Name", filterInfo.CategoryId);
            ViewBag.FilterInfo = filterInfo;
            if (filterInfo.CategoryId != null && filterInfo.CategoryId != -1)
            {
                products = products.Where(p => p.CategoryId == filterInfo.CategoryId);
            }
            if (!string.IsNullOrEmpty(filterInfo.Description))
            {
                products = products.Where(p => p.Description.Contains(filterInfo.Description));
            }
            if (filterInfo.FromPrice != null)
            {
                products = products.Where(p => p.Price >= filterInfo.FromPrice);
            }
            if (filterInfo.ToPrice != null)
            {
                products = products.Where(p => p.Price <= filterInfo.ToPrice);
            }
            return products;
        }
        private IQueryable<Product> Sort(IQueryable<Product> products, string? orderBy, string? orderType, string? op)
        {
            //Must run: Install-Package System.Linq.Dynamic.Core
            SortInfo sortInfo = new SortInfo { orderBy = "Id", orderType = "ASC" };
            switch (op)
            {
                case "sort-do":
                    sortInfo = new SortInfo { orderBy = orderBy, orderType = orderType };
                    break;
                case "sort-clear":
                    break;
                default:
                    sortInfo = HttpContext.Session.Get<SortInfo>("sortInfo") ?? sortInfo;
                    break;
            }
            HttpContext.Session.Set<SortInfo>("sortInfo", sortInfo);
            List<string> fieldList = new List<string> { "Id", "Description", "Discount", "Price", "Category" };
            ViewBag.FieldList = new SelectList(fieldList, sortInfo.orderBy);
            ViewBag.SortInfo = sortInfo;
            return products.OrderBy($"{sortInfo.orderBy} {sortInfo.orderType}");
        }
    }
}
