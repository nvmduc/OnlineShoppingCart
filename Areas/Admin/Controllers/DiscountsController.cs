﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Models;
using OnlineShoppingCart.Models.BusinessModels;
using X.PagedList;

namespace EProjects_III.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiscountsController : Controller
    {
        private readonly OnlineShoppingCartContext _context;

        public DiscountsController(OnlineShoppingCartContext context)
        {
            _context = context;
        }

        public IActionResult Index(string Search, int page = 1)
        {
            int limit = 5;
            var list = _context.Discounts.OrderBy(d => d.DiscountID).ToPagedList(page, limit);
            if (!string.IsNullOrEmpty(Search))
            {
                list = _context.Discounts.Where(d => d.DiscountName.Contains(Search)).OrderBy(d => d.DiscountID).ToPagedList(page, limit);

            }
            return View(list);
        }

       

        // GET: Admin/Discounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discounts
                .FirstOrDefaultAsync(m => m.DiscountID == id);
            if (discount == null)
            {
                return NotFound();
            }

            return View(discount);
        }

        // GET: Admin/Discounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Discounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiscountId,DiscountName,Description,DiscountPercent,Active,CreatedAt,ModifiedAt,DeleteAt")] Discounts discount)
        {
            if (ModelState.IsValid)
            {
                discount.Created_at = DateTime.Now;
                _context.Discounts.Add(discount);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");

                
            }
            return View(discount);
        }

        // GET: Admin/Discounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                return NotFound();
            }
            return View(discount);
        }

        // POST: Admin/Discounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiscountId,DiscountName,Description,DiscountPercent,Active,CreatedAt,ModifiedAt,DeleteAt")] Discounts discount)
        {
            if (id != discount.DiscountID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    discount.Created_at = DateTime.Now;
                    _context.Update(discount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscountExists(discount.DiscountID))
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
            return View(discount);
        }

        // GET: Admin/Discounts/Delete/5
        public IActionResult Delete(int id)
        {
            var checkProduct = _context.Products.FirstOrDefault(p => p.DiscountID == id);
            if (checkProduct != null)
            {
                TempData["eror"] = "Discount code exists product cannot be deleted!";
                return RedirectToAction("Index");
            }
            else
            {
                var discount = _context.Discounts.FirstOrDefault(b => b.DiscountID == id);
                if (discount != null)
                {
                    _context.Discounts.Remove(discount);
                    _context.SaveChanges();
                    TempData["success"] = "Xóa thành công";
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
        }

        //// POST: Admin/Discounts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    //var discount = await _context.Discount.FindAsync(id);
        //    //_context.Discount.Remove(discount);
        //    //await _context.SaveChangesAsync();
        //    //return RedirectToAction(nameof(Index));
        //    var checkProduct = _context.Products.FirstOrDefault(p => p.DiscountId == id);
        //    if (checkProduct != null)
        //    {
        //        TempData["eror"] = "Nhãn hàng tồn tại sản phẩm không thể xóa!";
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        var discount = _context.Discount.FirstOrDefault(b => b.DiscountId == id);
        //        if (discount != null)
        //        {
        //            _context.Discount.Remove(discount);
        //            _context.SaveChanges();
        //            TempData["success"] = "Xóa thành công";
        //            return RedirectToAction("Index");
        //        }
        //        return RedirectToAction("Index");
        //    }



        //}

        private bool DiscountExists(int id)
        {
            return _context.Discounts.Any(e => e.DiscountID == id);
        }
    }
}
