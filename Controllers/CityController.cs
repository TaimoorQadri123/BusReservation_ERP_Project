using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusReservationERP.Data;
using BusReservationERP.Models;

namespace BusReservationERP.Controllers
{
    [Authorize]
    public class CityController : Controller
    {
        private readonly AppDbContext _context;

        public CityController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /City
        public async Task<IActionResult> Index()
        {
            var cities = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
            return View(cities);
        }

        // GET: /City/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /City/Create
        [HttpPost]
        public async Task<IActionResult> Create(City city)
        {
            if (!ModelState.IsValid)
            {
                return View(city);
            }

            bool exists = await _context.Cities.AnyAsync(c => c.CityName == city.CityName);
            if (exists)
            {
                ModelState.AddModelError("CityName", "Ye city pehle se maujood hai.");
                return View(city);
            }

            _context.Cities.Add(city);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}