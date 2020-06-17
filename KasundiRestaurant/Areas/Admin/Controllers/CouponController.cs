using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KasundiRestaurant.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KasundiRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var couponList = await _db.Coupon.ToListAsync();

            return View(couponList);
        }
    }
}
