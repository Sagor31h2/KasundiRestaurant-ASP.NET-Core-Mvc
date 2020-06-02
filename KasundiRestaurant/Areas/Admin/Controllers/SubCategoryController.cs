using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KasundiRestaurant.Data;
using KasundiRestaurant.Models.ViewModels;
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
        //GET-INDEX
        public async Task<IActionResult> Index()
        {
            var subCategory = await _db.SubCategory.Include(m => m.Category).ToListAsync();
            return View(subCategory);
        }
        //GET-CREATE
        public async Task<IActionResult> Create()
        {
            var model = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).
                Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(model);

        }
    }
}