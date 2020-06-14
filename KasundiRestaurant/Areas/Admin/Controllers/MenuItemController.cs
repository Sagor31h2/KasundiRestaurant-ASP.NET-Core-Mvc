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


    }
}
