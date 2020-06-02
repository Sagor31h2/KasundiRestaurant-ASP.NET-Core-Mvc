using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KasundiRestaurant.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KasundiRestaurant.Areas.Admin.Controllers
{[Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var subCategory = await _db.SubCategory.Include(m => m.Category).ToListAsync();
            return View(subCategory);
        }
    }
}