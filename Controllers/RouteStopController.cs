using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusReservationERP.Data;
using BusReservationERP.Models;

namespace BusReservationERP.Controllers
{
    [Authorize]
    public class RouteStopController : Controller
    {
        private readonly AppDbContext _context;

        public RouteStopController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /RouteStop/Manage/5   (5 = RouteId)
        public async Task<IActionResult> Manage(int routeId)
        {
            var route = await _context.Routes
                .Include(r => r.Stops)
                .Include(r => r.OriginCity)
                .Include(r => r.DestinationCity)
                .FirstOrDefaultAsync(r => r.RouteId == routeId);

            if (route == null)
            {
                return NotFound();
            }

            route.Stops = route.Stops?.OrderBy(s => s.StopOrder).ToList();

            ViewBag.RouteId = routeId;
            ViewBag.RouteName = $"{route.OriginCity?.CityName} → {route.DestinationCity?.CityName}";

            return View(route);
        }

        // POST: /RouteStop/Create
        [HttpPost]
        public async Task<IActionResult> Create(int routeId, string stopName, int stopOrder)
        {
            if (string.IsNullOrWhiteSpace(stopName))
            {
                TempData["Error"] = "Stop ka naam likhna zaroori hai.";
                return RedirectToAction(nameof(Manage), new { routeId });
            }

            // Business rule: Same route mein same StopOrder duplicate na ho
            bool orderExists = await _context.RouteStops
                .AnyAsync(s => s.RouteId == routeId && s.StopOrder == stopOrder);

            if (orderExists)
            {
                TempData["Error"] = "Ye Stop Order pehle se is route mein maujood hai.";
                return RedirectToAction(nameof(Manage), new { routeId });
            }

            var stop = new RouteStop
            {
                RouteId = routeId,
                StopName = stopName,
                StopOrder = stopOrder
            };

            _context.RouteStops.Add(stop);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage), new { routeId });
        }

        // POST: /RouteStop/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id, int routeId)
        {
            var stop = await _context.RouteStops.FindAsync(id);
            if (stop != null)
            {
                _context.RouteStops.Remove(stop);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Manage), new { routeId });
        }
    }
}