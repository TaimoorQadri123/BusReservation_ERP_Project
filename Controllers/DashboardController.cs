using BusReservationERP.Data;
using BusReservationERP.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        //var today = DateTime.Today;
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        //var tomorrow = today.AddDays(1);

        var todayTrips = await _context.Trips
            .Include(t => t.Bus)
            .Include(t => t.Route)
                .ThenInclude(r => r!.OriginCity)
            .Include(t => t.Route)
                .ThenInclude(r => r!.DestinationCity)
              //.Where(t => t.DepartureDate >= today &&
              //            t.DepartureDate < tomorrow)
              .Where(t => t.DepartureDate >= today &&
            t.DepartureDate < tomorrow)
            .ToListAsync();
      

        var todayBookings = await _context.Bookings
            .Where(b => b.CreatedAt >= today &&
                        b.CreatedAt < tomorrow &&
                        b.BookingStatus != "Cancelled")
            .ToListAsync();

        var todayPayments = await _context.Payments
            .Where(p => p.PaidAt.HasValue &&
                        p.PaidAt.Value >= today &&
                        p.PaidAt.Value < tomorrow &&
                        (p.Status == "Paid" || p.Status == "Partial"))
            .ToListAsync();

        var allPayments = await _context.Payments.ToListAsync();
        var allBuses = await _context.Buses.ToListAsync();

        var totalTripsCount = await _context.Trips.CountAsync();

        var vm = new DashboardViewModel
        {
            TotalTripsCount = totalTripsCount,
            TodayTripsCount = todayTrips.Count,
            TodayBookingsCount = todayBookings.Count,

            TodayRevenue = todayPayments.Sum(p => p.Amount),

            AvailableSeatsToday = todayTrips.Sum(t => t.AvailableSeats),

            TodayTrips = todayTrips
                .OrderBy(t => t.DepartureTime)
                .Select(t => new DashboardTripRow
                {
                    TripId = t.TripId,
                    BusNumber = t.Bus?.BusNumber ?? "",
                    Route =
                        $"{t.Route?.OriginCity?.CityName} → " +
                        $"{t.Route?.DestinationCity?.CityName}",
                    DepartureTime = t.DepartureTime.ToString(@"hh\:mm"),
                    BookedSeats =
                        (t.Bus?.TotalSeats ?? 0) - t.AvailableSeats,
                    TotalSeats = t.Bus?.TotalSeats ?? 0,
                    Status = t.Status
                })
                .ToList(),

            PaidCount = allPayments.Count(p => p.Status == "Paid"),
            PaidAmount = allPayments
                .Where(p => p.Status == "Paid")
                .Sum(p => p.Amount),

            PendingCount = allPayments.Count(p => p.Status == "Pending"),
            PendingAmount = allPayments
                .Where(p => p.Status == "Pending")
                .Sum(p => p.Amount),

            PartialCount = allPayments.Count(p => p.Status == "Partial"),
            PartialAmount = allPayments
                .Where(p => p.Status == "Partial")
                .Sum(p => p.Amount),

            ActiveBuses = allBuses.Count(b => b.Status == "Active"),
            MaintenanceBuses = allBuses.Count(b => b.Status == "Maintenance"),
            InactiveBuses = allBuses.Count(b => b.Status == "Inactive"),
            TotalBuses = allBuses.Count
        };

        var recentBookingsRaw = await _context.Bookings
            .Include(b => b.Trip)
                .ThenInclude(t => t!.Bus)
            .Include(b => b.Trip)
                .ThenInclude(t => t!.Route)
                    .ThenInclude(r => r!.OriginCity)
            .Include(b => b.Trip)
                .ThenInclude(t => t!.Route)
                    .ThenInclude(r => r!.DestinationCity)
            .Where(b => b.BookingStatus != "Cancelled")
            .ToListAsync();

        vm.RecentBookings = recentBookingsRaw
            .GroupBy(b => b.BookingGroupId)
            .Select(g => new DashboardRecentBooking
            {
                PassengerName = g.First().PassengerName,
                SeatCount = g.Count(),
                Route =
                    $"{g.First().Trip?.Route?.OriginCity?.CityName} → " +
                    $"{g.First().Trip?.Route?.DestinationCity?.CityName}",
                BusNumber = g.First().Trip?.Bus?.BusNumber ?? "",
                CreatedAt = g.Max(b => b.CreatedAt)
            })
            .OrderByDescending(x => x.CreatedAt)
            .Take(6)
            .ToList();

        return View(vm);
    }
}