using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KasundiRestaurant.Data;
using KasundiRestaurant.Models;
using KasundiRestaurant.Models.ViewModels;
using KasundiRestaurant.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KasundiRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        [BindProperty] public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemController(ApplicationDbContext db,IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
            MenuItemVM=new MenuItemViewModel()
            {
                Categories = _db.Category,
                MenuItem =new MenuItem()
            };
        }
        public async Task<IActionResult> Index()
        {
            var menuItems = await _db.MenuItem.
                Include(c=>c.Category).
                Include(c=>c.SubCategory).ToListAsync();
            return View(menuItems);
        }

        //GET-CREATE
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        //POST-CREATE
        [ActionName("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            //image saving
            string webRootPath = _hostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;


            var menuItemFormDb = await _db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count>0)
            {
               //file uploaded 
               var uploads = Path.Combine(webRootPath, "Images");
               var extension = Path.GetExtension(files[0].FileName);

               using (var fileStream=new FileStream(Path.Combine(uploads,MenuItemVM.MenuItem.Id+extension),FileMode.Create))
               {
                   files[0].CopyTo(fileStream);

               }

               menuItemFormDb.Image = @"\Images\" + MenuItemVM.MenuItem.Id + extension;
            }

            else
            {
                //no file uploaded
                var uploads = Path.Combine(webRootPath, @"Images\" + StaticDetails.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\Images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFormDb.Image = @"\Images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //GET-EDIT 
        public async Task<IActionResult> Edit(int? id)
        {
            if (id==null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(c => c.Category).Include(c => c.SubCategory)
                .SingleOrDefaultAsync(c => c.Id == id);

            MenuItemVM.SubCategories = await _db.SubCategory.Where(c => c.CategoryId == MenuItemVM.MenuItem.CategoryId)
                .ToListAsync();
            if (MenuItemVM.MenuItem==null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //POST-EDIT
        [ActionName("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id==null)
            {
                NotFound();
            }

            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategories = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }

            //upload new image
            string webRootPath = _hostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;


            var menuItemFormDb = await _db.MenuItem.FindAsync(id);

            if (files.Count > 0)
            {
                //new file uploaded 
                var uploads = Path.Combine(webRootPath, "Images");
                var new_Extension = Path.GetExtension(files[0].FileName);

                //Delete the original file
                var imagePath = Path.Combine(webRootPath, menuItemFormDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (var fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + new_Extension), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);

                }

                menuItemFormDb.Image = @"\Images\" + MenuItemVM.MenuItem.Id + new_Extension;
            }

            menuItemFormDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFormDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFormDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFormDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
            menuItemFormDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFormDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET-DETAILS 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(c => c.Category).Include(c => c.SubCategory)
                .SingleOrDefaultAsync(c => c.Id == id);

            MenuItemVM.SubCategories = await _db.SubCategory.Where(c => c.CategoryId == MenuItemVM.MenuItem.CategoryId)
                .ToListAsync();
            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //GET-DELETE 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(c => c.Category).Include(c => c.SubCategory)
                .SingleOrDefaultAsync(c => c.Id == id);

            MenuItemVM.SubCategories = await _db.SubCategory.Where(c => c.CategoryId == MenuItemVM.MenuItem.CategoryId)
                .ToListAsync();
            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }
        //POST-DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
            {
                NotFound();
            }
            string webRootPath = _hostEnvironment.WebRootPath;

            var menuItem = await _db.MenuItem.FindAsync(id);

            var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _db.MenuItem.Remove(menuItem);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
