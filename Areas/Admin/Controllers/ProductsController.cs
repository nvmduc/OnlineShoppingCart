﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Models;
using X.PagedList;
using System.IO;
using OnlineShoppingCart.Models.BusinessModels;

namespace OnlineShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly OnlineShoppingCartContext _context;

        public ProductsController(OnlineShoppingCartContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public IActionResult Index(string Search, int page = 1)
        {
            int limit = 5;
            var list = _context.Products.Include(p => p.Discounts).Include(p => p.Categories).OrderByDescending(p => p.ProductID).ToPagedList(page, limit);
            if (!string.IsNullOrEmpty(Search))
            {
                list = _context.Products.Include(p => p.Discounts).Include(p => p.Categories).Where(p => p.ProductName.Contains(Search)).OrderByDescending(p => p.ProductID).ToPagedList(page, limit);
            }
            ViewData["Category"] = new SelectList(_context.Discounts, "CategoryId", "CategoryName");
            ViewData["Discount"] = new SelectList(_context.Discounts, "DiscountId", "DiscountName");
            return View(list);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Discounts)
                .FirstOrDefaultAsync(m => m.ProductID.Equals(id));
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["Discount"] = new SelectList(_context.Discounts, "DiscountId", "DiscountName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Image,Price,Quantity,CategoryId,DiscountId,Status,CreatedAt,ModifiedAt,DeleteAt")] Products products)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0 && files[0].Length > 0)
                {
                    var file = files[0];
                    var fileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ImgProducts", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        products.Image = fileName;
                    }
                    products.Created_at = DateTime.Now;
           

                }


                _context.Products.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", products.CategoryID);

            ViewData["Discount"] = new SelectList(_context.Discounts, "DiscountId", "DiscountName", products.DiscountID);

            return View(products);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", products.CategoryID);
            ViewData["Discount"] = new SelectList(_context.Discounts, "DiscountId", "DiscountId", products.DiscountID);
            return View(products);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProductId,ProductName,Image,Price,Quantity,CategoryId,DiscountId,Status,CreatedAt,ModifiedAt,DeleteAt")] Products products, [Bind("oldImg")]string oldImg)
        {
            
            if (id != products.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    if (files.Count() > 0 && files[0].Length > 0)
                    {
                        var file = files[0];
                        var fileName = file.FileName;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ImgProducts", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            products.Image = fileName;
                        }

                    }
                    else
                    {
                        products.Image = oldImg;
                    }

                    _context.Products.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ProductID))
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
            ViewData["Category"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", products.CategoryID);
            ViewData["Discount"] = new SelectList(_context.Discounts, "DiscountId", "DiscountId", products.DiscountID);
            return View(products);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Discounts)
                .FirstOrDefaultAsync(m => m.ProductID.Equals(id));
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var products = await _context.Products.FindAsync(id);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(Guid id)
        {
            return _context.Products.Any(e => e.ProductID.Equals(id));
        }
    }
}
