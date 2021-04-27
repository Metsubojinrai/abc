using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Areas_Admin_Controllers
{
    //[Area("Admin")]
    //[Route("[controller]/[action]")]
    [Authorize(Roles ="Admin")]
    public class ProductController : Controller
    {
        private readonly BlogDbContext _context;

        public ProductController(BlogDbContext context)
        {
            _context = context;
        }

        //mảng chứa các CategoryID của Product
        [BindProperty]
        public int[] SelectedCategories { set; get; }

        public const int ITEMS_PER_PAGE = 4;

        // GET: Product
        public async Task<IActionResult> Index([Bind(Prefix = "page")] int page)
        {
            if (page == 0)
                page = 1;
            var listproduct = _context.Products
                .Include(p=>p.ProductCategories)
                .ThenInclude(c=>c.Category)
                .OrderByDescending(p => p.DateCreated);
            // Lấy tổng số dòng dữ liệu
            var totalItems = listproduct.Count();
            // Tính số trang hiện thị (mỗi trang hiện thị ITEMS_PER_PAGE mục)
            int totalPages = (int)Math.Ceiling((double)totalItems / ITEMS_PER_PAGE);

            if (page > totalPages)
                return RedirectToAction(nameof(ProductController.Index), new { page = totalPages });


            var products = await listproduct
                            .Skip(ITEMS_PER_PAGE * (page - 1))       // Bỏ qua các trang trước
                            .Take(ITEMS_PER_PAGE)                          // Lấy số phần tử của trang hiện tại
                            .ToListAsync();

            // return View (await listPosts.ToListAsync());
            ViewData["pageNumber"] = page;
            ViewData["totalPages"] = totalPages;

            return View(products.AsEnumerable());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Name", SelectedCategories);

            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,Description,Price,DateCreated,DateUpdated,ProductPicture")] Product product)
        {
            if (SelectedCategories.Length == 0)
            {
                ModelState.AddModelError(String.Empty, "Phải ít nhất một chuyên mục");
            }

            if (ModelState.IsValid)
            {
                var newproduct = new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    ProductPicture = product.ProductPicture
                };
                _context.Add(newproduct);
                await _context.SaveChangesAsync();

                foreach (var selectedCategory in SelectedCategories)
                {
                    _context.Add(new ProductCategory() { ProductID = newproduct.ProductId, CategoryID = selectedCategory });
                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Name", SelectedCategories);

            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Where(p => p.ProductId==id)
                .Include(p => p.ProductCategories)
                .ThenInclude(c => c.Category).FirstOrDefaultAsync();
            if (product == null)
            {
                return NotFound();
            }
            var selectedCates = product.ProductCategories.Select(c => c.CategoryID).ToArray();
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Name", selectedCates);
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Description,Price,DateCreated,DateUpdated,ProductPicture")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (SelectedCategories.Length == 0)
            {
                ModelState.AddModelError(String.Empty, "Phải ít nhất một loại sản phẩm");
            }

            if (ModelState.IsValid)
            {
                // Lấy nội dung từ DB
                var productUpdate = await _context.Products.Where(p => p.ProductId == id)
                    .Include(p => p.ProductCategories)
                    .ThenInclude(c => c.Category).FirstOrDefaultAsync();
                if (productUpdate == null)
                {
                    return NotFound();
                }

                productUpdate.Name = product.Name;
                productUpdate.Description = product.Description;
                productUpdate.Price = product.Price;
                productUpdate.ProductPicture = product.ProductPicture;
                productUpdate.DateUpdated = DateTime.Now;

                // Các danh mục không có trong selectedCategories
                var listcateremove = productUpdate.ProductCategories
                                               .Where(p => !SelectedCategories.Contains(p.CategoryID))
                                               .ToList();
                listcateremove.ForEach(c => productUpdate.ProductCategories.Remove(c));

                // Các ID category chưa có trong postUpdate.PostCategories
                var listCateAdd = SelectedCategories
                                    .Where(
                                        id => !productUpdate.ProductCategories.Where(c => c.CategoryID == id).Any()
                                    ).ToList();

                listCateAdd.ForEach(id => {
                    productUpdate.ProductCategories.Add(new ProductCategory()
                    {
                        ProductID = productUpdate.ProductId,
                        CategoryID = id
                    });
                });

                try
                {
                    _context.Update(productUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            var categories = await _context.Categories.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Name", SelectedCategories);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
