using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusReservationERP.Data;
using BusReservationERP.Models;
using BusReservationERP.ViewModels;

namespace BusReservationERP.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Booking
        public async Task<IActionResult> Index()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Trip)
                    .ThenInclude(t => t!.Bus)
                .Include(b => b.BoardingStop)
                .Include(b => b.DropStop)
                .ToListAsync();

            return View(bookings);
        }

        // GET: /Booking/Create
        public async Task<IActionResult> Create()
        {
            var vm = new BookingViewModel
            {
                TripList = await _context.Trips
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.OriginCity)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.DestinationCity)
                    .Where(t => t.Status == "Scheduled")
                    .ToListAsync()
            };

            return View(vm);
        }

        // POST: /Booking/Create
        [HttpPost]
        public async Task<IActionResult> Create(BookingViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.TripList = await _context.Trips
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.OriginCity)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.DestinationCity)
                    .Where(t => t.Status == "Scheduled")
                    .ToListAsync();
                return View(vm);
            }

            var trip = await _context.Trips.FindAsync(vm.TripId);
            if (trip == null || trip.Status != "Scheduled")
            {
                ModelState.AddModelError("", "The Trip was not available for booking.");
                vm.TripList = await _context.Trips
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.OriginCity)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.DestinationCity)
                    .Where(t => t.Status == "Scheduled")
                    .ToListAsync();
                return View(vm);
            }

            if (trip.AvailableSeats <= 0)
            {
                ModelState.AddModelError("", "The Seats are full.");
                vm.TripList = await _context.Trips
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.OriginCity)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.DestinationCity)
                    .Where(t => t.Status == "Scheduled")
                    .ToListAsync();
                return View(vm);
            }

            // Boarding aur Drop stops nikalo, unka StopOrder check karo
            var boardingStop = await _context.RouteStops.FindAsync(vm.BoardingStopId);
            var dropStop = await _context.RouteStops.FindAsync(vm.DropStopId);

            if (boardingStop == null || dropStop == null)
            {
                ModelState.AddModelError("", "Boarding or Drop stop Not Found.");
                vm.TripList = await _context.Trips
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.OriginCity)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.DestinationCity)
                    .Where(t => t.Status == "Scheduled")
                    .ToListAsync();
                return View(vm);
            }

            if (boardingStop.StopOrder >= dropStop.StopOrder)
            {
                ModelState.AddModelError("", "Drop stop,SHould be Ahead From Boarding stop .");
                vm.TripList = await _context.Trips
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.OriginCity)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.DestinationCity)
                    .Where(t => t.Status == "Scheduled")
                    .ToListAsync();
                return View(vm);
            }

            // Seat already li hui to nahi, isi Trip mein
            bool seatTaken = await _context.Bookings.AnyAsync(b =>
                b.TripId == vm.TripId &&
                b.SeatNumber == vm.SeatNumber &&
                b.BookingStatus != "Cancelled");

            if (seatTaken)
            {
                ModelState.AddModelError("", "This seat Number is already booked.");
                vm.TripList = await _context.Trips
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.OriginCity)
                    .Include(t => t.Route)
                        .ThenInclude(r => r!.DestinationCity)
                    .Where(t => t.Status == "Scheduled")
                    .ToListAsync();
                return View(vm);
            }

            var booking = new Booking
            {
                TripId = vm.TripId,
                BoardingStopId = vm.BoardingStopId,
                DropStopId = vm.DropStopId,
                PassengerName = vm.PassengerName,
                PassengerPhone = vm.PassengerPhone,
                SeatNumber = vm.SeatNumber,
                BookingStatus = "Booked"
            };

            _context.Bookings.Add(booking);

            // Seat kam karo aur Full check karo
            trip.AvailableSeats -= 1;
            if (trip.AvailableSeats == 0)
            {
                trip.Status = "Full";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // AJAX endpoint: Trip select hone pe uske route ke stops laana
        [HttpGet]
        public async Task<IActionResult> GetStopsForTrip(int tripId)
        {
            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null)
            {
                return Json(new List<object>());
            }

            var stops = await _context.RouteStops
                .Where(s => s.RouteId == trip.RouteId)
                .OrderBy(s => s.StopOrder)
                .Select(s => new { s.RouteStopId, s.StopName, s.StopOrder })
                .ToListAsync();

            return Json(stops);
        }
        // AJAX endpoint: Trip ki available seats laana
        [HttpGet]
        public async Task<IActionResult> GetAvailableSeats(int tripId)
        {
            var trip = await _context.Trips
                .Include(t => t.Bus)
                .FirstOrDefaultAsync(t => t.TripId == tripId);

            if (trip == null || trip.Bus == null)
            {
                return Json(new List<int>());
            }

            // Bus ki total seats ka poora range banao (1 se TotalSeats tak)
            var allSeatNumbers = Enumerable.Range(1, trip.Bus.TotalSeats).ToList();

            // Jo seats already book ho chuki hain (cancelled ko chhod kar)
            var bookedSeats = await _context.Bookings
                .Where(b => b.TripId == tripId && b.BookingStatus != "Cancelled")
                .Select(b => b.SeatNumber)
                .ToListAsync();

            // Available = Total - Booked
            var availableSeats = allSeatNumbers.Except(bookedSeats).OrderBy(s => s).ToList();

            return Json(availableSeats);
        }
        // POST: /Booking/Cancel/5
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Trip)
                .FirstOrDefaultAsync(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.BookingStatus == "Cancelled")
            {
                TempData["Error"] = "Ye booking pehle se cancelled hai.";
                return RedirectToAction(nameof(Index));
            }

            booking.BookingStatus = "Cancelled";

            // Trip ki seat wapas badhao
            if (booking.Trip != null)
            {
                booking.Trip.AvailableSeats += 1;

                // Agar Trip Full thi, ab wapas Scheduled kar do (kyunki seat khali hui)
                if (booking.Trip.Status == "Full")
                {
                    booking.Trip.Status = "Scheduled";
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}