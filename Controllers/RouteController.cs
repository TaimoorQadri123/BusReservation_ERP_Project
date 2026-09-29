using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusReservationERP.Data;
using BusReservationERP.Models;

namespace BusReservationERP.Controllers
{
    [Authorize]
    public class RouteController : Controller
    {
        private readonly AppDbContext _context;

        public RouteController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Route
        public async Task<IActionResult> Index()
        {
            var routes = await _context.Routes
                .Include(r => r.OriginCity)
                .Include(r => r.DestinationCity)
                .ToListAsync();

            return View(routes);
        }

        // GET: /Route/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.CityList = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
            return View();
        }

        // POST: /Route/Create
        [HttpPost]
        public async Task<IActionResult> Create(BusRoute route)
        {
            // Origin aur Destination same nahi ho sakte
            if (route.OriginCityId == route.DestinationCityId)
            {
                ModelState.AddModelError("", "Origin & Destination city Should be different.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CityList = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
                return View(route);
            }

            bool exists = await _context.Routes.AnyAsync(r =>
                r.OriginCityId == route.OriginCityId &&
                r.DestinationCityId == route.DestinationCityId);

            if (exists)
            {
                ModelState.AddModelError("", "This route has already inserted.");
                ViewBag.CityList = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
                return View(route);
            }

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Route/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var route = await _context.Routes.FindAsync(id);
            if (route == null)
            {
                return NotFound();
            }

            ViewBag.CityList = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
            return View(route);
        }

        // POST: /Route/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, BusRoute route)
        {
            if (id != route.RouteId)
            {
                return NotFound();
            }

            if (route.OriginCityId == route.DestinationCityId)
            {
                ModelState.AddModelError("", "Origin & Destination city Should be different .");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CityList = await _context.Cities.OrderBy(c => c.CityName).ToListAsync();
                return View(route);
            }

            _context.Routes.Update(route);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}