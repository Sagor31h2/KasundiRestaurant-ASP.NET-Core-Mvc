using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KasundiRestaurant.Data;
using KasundiRestaurant.Models;
using KasundiRestaurant.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KasundiRestaurant.Areas.Admin.Controllers
{[Area("Admin")]
    public class SubCategoryController : Controller
    { 
        private readonly ApplicationDbContext _db;

        [TempData] public string StatusMessageSubCategory { get; set; }

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
        //Post-CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryAndSubCategoryViewModel model)
        {
            var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category)
                .Where(p => p.Name==model.SubCategory.Name && p.Category.Id == model.SubCategory.CategoryId);
            if (doesSubCategoryExists.Count()>0)
            {
                //Error
                StatusMessageSubCategory = "Error:SubCategory Exists Under " +
                                doesSubCategoryExists.First().Category.Name +
                                " category.Please use another name";
            }
            else
            {
                _db.SubCategory.Add(model.SubCategory);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var modelVM = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).
                    Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessageSubCategory
            };
            return View(modelVM);
        }

        //GET-SUBCATEGORY
        [ActionName("GetSubcategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            var subCategories=new List<SubCategory>();
            subCategories = await _db.SubCategory.Where(c => c.CategoryId == id).ToListAsync();

            return Json(new SelectList(subCategories,"Id","Name"));
        }

        //GET-EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(c => c.Id == id);
            if (subCategory==null)
            {
                return NotFound();
            }
            var model = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).
                    Select(p => p.Name).Distinct().ToListAsync()
            };
            return View(model);

        }


        //POST-EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryAndSubCategoryViewModel model)
        {
            var doesSubCategoryExists = _db.SubCategory.Include(s => s.Category)
                .Where(p => p.Name == model.SubCategory.Name && p.Category.Id == model.SubCategory.CategoryId);
            if (doesSubCategoryExists.Count() > 0)
            {
                //Error
                StatusMessageSubCategory = "Error:SubCategory Exists Under " +
                                           doesSubCategoryExists.First().Category.Name +
                                           " category.Please use another name";
            }
            else
            {

                var subCategoryFromDb = await _db.SubCategory.FindAsync(model.SubCategory.Id);
                subCategoryFromDb.Name = model.SubCategory.Name;

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var modelVM = new CategoryAndSubCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).
                    Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessageSubCategory
            };
            return View(modelVM);
        }

        //GET-DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

    }
}