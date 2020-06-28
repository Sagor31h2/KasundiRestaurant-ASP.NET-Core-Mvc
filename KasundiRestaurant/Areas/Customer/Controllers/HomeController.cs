using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using KasundiRestaurant.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KasundiRestaurant.Models;
using KasundiRestaurant.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace KasundiRestaurant.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var IndexVM = new IndexViewModel()
            {
                MenuItems = await _db.MenuItem.Include(c => c.Category).Include(c => c.SubCategory).ToListAsync(),
                Categories = await _db.Category.ToListAsync(),
                Coupons = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };
            return View(IndexVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
