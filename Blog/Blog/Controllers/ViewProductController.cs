using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    [Route("/products")]
    public class ViewProductController : Controller
    {
        private readonly ILogger<ViewProductController> _logger;

        private readonly BlogDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMemoryCache _cache;
        public const int ITEMS_PER_PAGE = 6;
        public ViewProductController(ILogger<ViewProductController> logger, BlogDbContext context,
            UserManager<User> userManager, SignInManager<User> signInManager, IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _cache = cache;
        }

        // Hiện thị danh sách sản phẩm, có nút chọn đưa vào giỏ hàng
        [Route("{category?}", Name = "listproduct")]
        public async Task<IActionResult> Index([FromQuery] int page,
                [FromRoute(Name = "category")] string category,string sortOrder)
        {
            List<string> listsort = new() { "name desc","date","date desc"};
            ViewData["SortOrders"] = new SelectList(listsort);
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name desc" : "";
            var categories = GetCategories();
            Category cate = null;
            if (category != null)
            {
                cate = FindCategoryBySlug(categories, category);
            }
            
            ViewData["categories"] = categories;
            ViewData["productCategory"] = category;
            ViewData["CurrentCategory"] = cate;

            if (page == 0)
                page = 1;

            // ........................................
            // Truy vấn lấy các product
            var listproduct = _context.Products.AsQueryable();

            if (cate != null)
            {
                var ids = new List<int>
                {
                    cate.Id
                };
                listproduct = listproduct
                    .Where(p => p.ProductCategories.Where(c => ids.Contains(c.CategoryID)).Any());
            }

            switch (sortOrder)
            {
                case "name desc":
                    listproduct = listproduct.OrderByDescending(s => s.Name);
                    break;
                case "date desc":
                    listproduct = listproduct.OrderByDescending(s => s.DateCreated);
                    break;
                case "date":
                    listproduct = listproduct.OrderBy(s => s.DateCreated);
                    break;
                default:
                    listproduct = listproduct.OrderBy(s => s.Name);
                    break;
            }

            // Lấy tổng số dòng dữ liệu
            var totalItems = listproduct.Count();
            // Tính số trang hiện thị (mỗi trang hiện thị ITEMS_PER_PAGE mục)
            int totalPages = (int)Math.Ceiling((double)totalItems / ITEMS_PER_PAGE);

            if (page > totalPages)
                return RedirectToAction(nameof(ViewProductController.Index), new { page = totalPages });

            var products = await listproduct
                            .Skip(ITEMS_PER_PAGE * (page - 1))       // Bỏ qua các trang trước
                            .Take(ITEMS_PER_PAGE)                    // Lấy số phần tử của trang hiện tại
                            .ToListAsync();

            ViewData["pageNumber"] = page;
            ViewData["totalPages"] = totalPages;

            return View(products.AsEnumerable());
        }

        // Thêm sản phẩm vào cart
        [Route("/addcart/{productid:int}",Name ="addcart")]
        public IActionResult AddToCart([FromRoute] int productid)
        {
            var product = _context.Products.Where(p => p.ProductId == productid).FirstOrDefault();
            if (product == null)
                return NotFound("Không có sản phẩm");
            // Xử lý đưa vào Cart ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if(cartitem != null)
            {
                // Đã tồn tại, tăng thêm 1
                cartitem.quantity++;
            }
            else
            {
                // Thêm mới
                cart.Add(new CartItem() { quantity = 1, product = product });
            }

            // Lưu cart vào session
            SaveCartSession(cart);
         
            return RedirectToAction(nameof(Index));
        }

        // Xóa item trong cart
        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            // Xử lý xóa một mục của Cart ...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
            {
                cart.Remove(cartitem);
            }
            SaveCartSession(cart);
            return RedirectToAction(nameof(Cart));
        }

        // Cập nhật
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm]int productid, [FromForm]int quantity)
        {
            //Cập nhật Cart thay đổi số lượng quantity...
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.product.ProductId == productid);
            if (cartitem != null)
                cartitem.quantity = quantity;
            SaveCartSession(cart);
            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return Ok();
        }

        // Hiện thị giỏ hàng
        [Route("/cart", Name = "cart")]
        public IActionResult Cart()
        {
            return View(GetCartItems());
        }

        [Route("/checkout", Name = "checkout")]
        public async Task<IActionResult> Checkout(Order order)
        {
            if(!_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCartItems();
            if (cart.Count == 0)
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn trống, hãy thêm một số mặt hàng trước");
                return RedirectToAction(nameof(Index));
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            order.OrderTotal = cart.Sum(item => item.quantity * item.product.Price);
            order.OrderPlaced = DateTime.Now;
            order.UserId = Convert.ToInt32(userId);
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail()
                {
                    Quantity = item.quantity,
                    ProductId = item.product.ProductId,
                    OrderId = order.Id,
                    Price = item.product.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }
            await _context.SaveChangesAsync();
            ClearCart();

            return RedirectToAction("CheckoutComplete");
        }

        [Route("/checkoutcomplete")]
        public IActionResult CheckoutComplete()
        {
            ViewBag.CheckoutCompleteMessage = "Cảm ơn đơn đặt hàng của bạn.";
            return View();
        }

        // Key lưu chuỗi json của Cart
        public const string CARTKEY = "cart";
        // Lấy cart từ Session (danh sách CartItem)
        List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart);
            return new List<CartItem>();
        }

        // Xóa cart khỏi session
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }

        [NonAction]
        List<Category> GetCategories()
        {
            string keycacheCategories = "_listallcategories";

            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(keycacheCategories, out List<Category> categories))
            {

                categories = _context.Categories
                    .AsEnumerable()
                    .ToList();

                // Thiết lập cache - lưu vào cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set("_GetCategories", categories, cacheEntryOptions);
            }

            return categories;
        }

        [NonAction]
        Category FindCategoryBySlug(List<Category> categories, string category)
        {

            foreach (var c in categories)
            {
                if (c.Name.ToLower() == category) return c;
            }

            return null;
        }
    }
}
