using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusReservationERP.Data;
using BusReservationERP.Models;
using BusReservationERP.ViewModels;

namespace BusReservationERP.Controllers
{
    [Authorize]
    public class TripController : Controller
    {
        private readonly AppDbContext _context;

        public TripController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Trip
        public async Task<IActionResult> Index()
        {
            var trips = await _context.Trips
                .Include(t => t.Bus)
                .Include(t => t.Route)
                    .ThenInclude(r => r!.OriginCity)
                .Include(t => t.Route)
                    .ThenInclude(r => r!.DestinationCity)
                .ToListAsync();

            return View(trips);
        }

        // GET: /Trip/Create
        public async Task<IActionResult> Create()
        {
            var vm = new TripViewModel
            {
                BusList = await _context.Buses.Where(b => b.Status == "Active").ToListAsync(),
                RouteList = await _context.Routes
                    .Include(r => r.OriginCity)
                    .Include(r => r.DestinationCity)
                    .ToListAsync()
            };

            return View(vm);
        }

        // POST: /Trip/Create
        [HttpPost]
        public async Task<IActionResult> Create(TripViewModel vm)
        {
            // Agar validation fail ho, to dropdowns dobara load karke form wapas dikhao
            if (!ModelState.IsValid)
            {
                vm.BusList = await _context.Buses.Where(b => b.Status == "Active").ToListAsync();
                vm.RouteList = await _context.Routes
                    .Include(r => r.OriginCity)
                    .Include(r => r.DestinationCity)
                    .ToListAsync();
                return View(vm);
            }

            // Naye trip ka poora Date+Time ek jagah banao
            DateTime newTripDateTime = vm.DepartureDate.Date + vm.DepartureTime;

            // Business Rule: Same Bus ke existing trips dhoondo (Route ka farq nahi padta)
            var conflictingTrips = await _context.Trips
                .Where(t => t.BusId == vm.BusId && t.Status != "Cancelled")
                .ToListAsync();

            foreach (var existingTrip in conflictingTrips)
            {
                DateTime existingTripDateTime = existingTrip.DepartureDate.Date + existingTrip.DepartureTime;

                double hoursDifference = Math.Abs((newTripDateTime - existingTripDateTime).TotalHours);

                if (hoursDifference < 24)
                {
                    ModelState.AddModelError("",
                        "This Bus is not available on this date.");

                    vm.BusList = await _context.Buses.Where(b => b.Status == "Active").ToListAsync();
                    vm.RouteList = await _context.Routes
                        .Include(r => r.OriginCity)
                        .Include(r => r.DestinationCity)
                        .ToListAsync();
                    return View(vm);
                }
            }

            // Bus ki TotalSeats nikalo taake AvailableSeats set ho sake
            var selectedBus = await _context.Buses.FindAsync(vm.BusId);
            if (selectedBus == null)
            {
                ModelState.AddModelError("", "Selected bus is null.");
                vm.BusList = await _context.Buses.Where(b => b.Status == "Active").ToListAsync();
                vm.RouteList = await _context.Routes
                    .Include(r => r.OriginCity)
                    .Include(r => r.DestinationCity)
                    .ToListAsync();
                return View(vm);
            }

            var trip = new Trip
            {
                BusId = vm.BusId,
                RouteId = vm.RouteId,
                DepartureDate = vm.DepartureDate,
                DepartureTime = vm.DepartureTime,
                Fare = vm.Fare,
                AvailableSeats = selectedBus.TotalSeats,
                Status = "Scheduled"
            };

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Trip/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            var vm = new TripViewModel
            {
                TripId = trip.TripId,
                BusId = trip.BusId,
                RouteId = trip.RouteId,
                DepartureDate = trip.DepartureDate,
                DepartureTime = trip.DepartureTime,
                Fare = trip.Fare,
                BusList = await _context.Buses.Where(b => b.Status == "Active").ToListAsync(),
                RouteList = await _context.Routes
                    .Include(r => r.OriginCity)
                    .Include(r => r.DestinationCity)
                    .ToListAsync()
            };

            return View(vm);
        }

        // POST: /Trip/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TripViewModel vm)
        {
            if (id != vm.TripId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                vm.BusList = await _context.Buses.Where(b => b.Status == "Active").ToListAsync();
                vm.RouteList = await _context.Routes
                    .Include(r => r.OriginCity)
                    .Include(r => r.DestinationCity)
                    .ToListAsync();
                return View(vm);
            }

            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            DateTime newTripDateTime = vm.DepartureDate.Date + vm.DepartureTime;

            // Same Bus check karo, lekin isi trip ko exclude karke (khud se conflict na ho)
            var conflictingTrips = await _context.Trips
                .Where(t => t.BusId == vm.BusId && t.Status != "Cancelled" && t.TripId != id)
                .ToListAsync();

            foreach (var existingTrip in conflictingTrips)
            {
                DateTime existingTripDateTime = existingTrip.DepartureDate.Date + existingTrip.DepartureTime;
                double hoursDifference = Math.Abs((newTripDateTime - existingTripDateTime).TotalHours);

                if (hoursDifference < 24)
                {
                    ModelState.AddModelError("",
                        "This Bus is already registered on another trip within 24 hours.");

                    vm.BusList = await _context.Buses.Where(b => b.Status == "Active").ToListAsync();
                    vm.RouteList = await _context.Routes
                        .Include(r => r.OriginCity)
                        .Include(r => r.DestinationCity)
                        .ToListAsync();
                    return View(vm);
                }
            }

            trip.BusId = vm.BusId;
            trip.RouteId = vm.RouteId;
            trip.DepartureDate = vm.DepartureDate;
            trip.DepartureTime = vm.DepartureTime;
            trip.Fare = vm.Fare;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /Trip/Cancel/5
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            trip.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: /Trip/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            // Check karo is trip pe koi booking to nahi hui
            bool hasBookings = await _context.Bookings.AnyAsync(b => b.TripId == id);

            if (hasBookings)
            {
                TempData["Error"] = "Ye trip delete nahi ho sakti kyunki ismein bookings maujood hain. Isay Cancel karein.";
                return RedirectToAction(nameof(Index));
            }

            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}