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
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            // BookingGroupId ke hisaab se group karo
            var groupedBookings = bookings
                .GroupBy(b => b.BookingGroupId)
                .Select(g => new BookingGroupViewModel
                {
                    BookingGroupId = g.Key,
                    PassengerName = g.First().PassengerName,
                    PassengerPhone = g.First().PassengerPhone,
                    SeatNumbers = g.Select(b => b.SeatNumber).OrderBy(s => s).ToList(),
                    Trip = g.First().Trip,
                    BoardingStop = g.First().BoardingStop,
                    DropStop = g.First().DropStop,
                    BookingStatus = g.First().BookingStatus,
                    SampleBookingId = g.First().BookingId
                })
                .ToList();

            return View(groupedBookings);
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

            // Check karo itni seats available hain ya nahi (jitni admin ne select ki hain)
            if (vm.SelectedSeats.Count > trip.AvailableSeats)
            {
                ModelState.AddModelError("", $"Sirf {trip.AvailableSeats} seats available hain, aapne {vm.SelectedSeats.Count} select ki hain.");
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
                ModelState.AddModelError("", "Drop stop, Boarding stop se aage hona chahiye.");
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

            // Check karo koi selected seat pehle se book to nahi
            var alreadyBookedSeats = await _context.Bookings
                .Where(b => b.TripId == vm.TripId &&
                            b.BookingStatus != "Cancelled" &&
                            vm.SelectedSeats.Contains(b.SeatNumber))
                .Select(b => b.SeatNumber)
                .ToListAsync();

            if (alreadyBookedSeats.Any())
            {
                ModelState.AddModelError("", $"Ye seats pehle se book hain: {string.Join(", ", alreadyBookedSeats)}");
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

            // Sab sahi hai — ek Group ID banao aur har seat ke liye alag Booking record banao
            Guid groupId = Guid.NewGuid();

            foreach (var seatNum in vm.SelectedSeats)
            {
                var booking = new Booking
                {
                    BookingGroupId = groupId,
                    TripId = vm.TripId,
                    BoardingStopId = vm.BoardingStopId,
                    DropStopId = vm.DropStopId,
                    PassengerName = vm.PassengerName,
                    PassengerPhone = vm.PassengerPhone,
                    SeatNumber = seatNum,
                    BookingStatus = "Booked"
                };

                _context.Bookings.Add(booking);
            }

            // Seats kam karo (jitni seats select hui, utni kam karo)
            trip.AvailableSeats -= vm.SelectedSeats.Count;
            if (trip.AvailableSeats <= 0)
            {
                trip.AvailableSeats = 0;
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
        // AJAX endpoint: Trip ki seat map (total seats + booked seats) laana
        [HttpGet]
        public async Task<IActionResult> GetSeatMapForTrip(int tripId)
        {
            var trip = await _context.Trips
                .Include(t => t.Bus)
                .FirstOrDefaultAsync(t => t.TripId == tripId);

            if (trip == null || trip.Bus == null)
            {
                return Json(new { totalSeats = 0, bookedSeats = new List<int>() });
            }

            var bookedSeats = await _context.Bookings
                .Where(b => b.TripId == tripId && b.BookingStatus != "Cancelled")
                .Select(b => b.SeatNumber)
                .ToListAsync();

            return Json(new { totalSeats = trip.Bus.TotalSeats, bookedSeats = bookedSeats });
        }
        // POST: /Booking/Cancel/5
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var referenceBooking = await _context.Bookings.FindAsync(id);
            if (referenceBooking == null)
            {
                return NotFound();
            }

            // Isi Group ke saare bookings nikalo (poori family/group)
            var groupBookings = await _context.Bookings
                .Include(b => b.Trip)
                .Where(b => b.BookingGroupId == referenceBooking.BookingGroupId && b.BookingStatus != "Cancelled")
                .ToListAsync();

            if (!groupBookings.Any())
            {
                TempData["Error"] = "Ye booking pehle se cancelled hai.";
                return RedirectToAction(nameof(Index));
            }

            Trip? trip = null;

            foreach (var booking in groupBookings)
            {
                booking.BookingStatus = "Cancelled";
                trip = booking.Trip;
            }

            // Trip ki seats wapas badhao (jitni seats is group mein thin, utni)
            if (trip != null)
            {
                trip.AvailableSeats += groupBookings.Count;

                if (trip.Status == "Full")
                {
                    trip.Status = "Scheduled";
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        // POST: /Booking/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var referenceBooking = await _context.Bookings.FindAsync(id);
            if (referenceBooking == null)
            {
                return NotFound();
            }

            // Isi Group ke saare bookings nikalo (poori family/group)
            var groupBookings = await _context.Bookings
                .Where(b => b.BookingGroupId == referenceBooking.BookingGroupId)
                .ToListAsync();

            _context.Bookings.RemoveRange(groupBookings);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}