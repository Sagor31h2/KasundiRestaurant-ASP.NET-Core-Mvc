using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KasundiRestaurant.Data;
using KasundiRestaurant.Models;
using KasundiRestaurant.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    }
}
