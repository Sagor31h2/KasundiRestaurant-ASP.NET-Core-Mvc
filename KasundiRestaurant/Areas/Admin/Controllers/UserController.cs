using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KasundiRestaurant.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KasundiRestaurant.Areas.Admin.Controllers
{[Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity) this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(await _db.ApplicationUser.Where(c=>c.Id!=claim.Value).ToListAsync());
        }
    }
}
